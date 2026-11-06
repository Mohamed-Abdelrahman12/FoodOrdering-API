
using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Auth.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FoodOrdering.Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResultDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;  
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,               
        ITokenService tokenService,
        IUnitOfWork uow,
        IMapper mapper,
        IConfiguration config,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _uow = uow;
        _mapper = mapper;
        _config = config;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResultDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await EnsureAdminExistsIfAdminCredentialsAsync(request.Email, request.Password);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
                return ApiResponse<AuthResultDto>.Fail("Invalid credentials.", 401);

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
                return ApiResponse<AuthResultDto>.Fail("Invalid credentials.", 401);

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var expiryDays = ParseConfigInt(_config["Jwt:RefreshTokenExpiryDays"], 7);

            await _uow.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
            }, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);
            var roles = await _userManager.GetRolesAsync(user);
            var dto = _mapper.Map<UserDto>(user);
            dto.Roles = roles.ToList();

            var expiryMinutes = ParseConfigInt(_config["Jwt:AccessTokenExpiryMinutes"], 60);

            return ApiResponse<AuthResultDto>.Ok(new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(expiryMinutes),
                User = dto
            }, "Login successful.");
        }
        catch (InvalidOperationException)
        {
          
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for {Email}", request.Email);
            return ApiResponse<AuthResultDto>.Fail(
                "An unexpected error occurred. Please try again.", 500);
        }
    }

    private async Task EnsureAdminExistsIfAdminCredentialsAsync(string email, string password)
    {
        var adminEmail = _config["AdminAccount:Email"] ?? string.Empty;
        var adminPassword = _config["AdminAccount:Password"] ?? string.Empty;

       
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            _logger.LogWarning(
                "AdminAccount:Email or AdminAccount:Password is not set in configuration. " +
                "Admin auto-provisioning is disabled.");
            return;
        }
        if (!string.Equals(email, adminEmail, StringComparison.OrdinalIgnoreCase))
            return;

        if (password != adminPassword)
            return;

        await EnsureRoleExistsAsync("Admin");
        await EnsureRoleExistsAsync("Customer");

        var existing = await _userManager.FindByEmailAsync(adminEmail);
        if (existing != null)
        {
            
            if (!await _userManager.IsInRoleAsync(existing, "Admin"))
                await _userManager.AddToRoleAsync(existing, "Admin");
            return;
        }

        var adminUser = new ApplicationUser
        {
            FirstName = _config["AdminAccount:FirstName"] ?? "System",
            LastName = _config["AdminAccount:LastName"] ?? "Admin",
            Email = adminEmail,
            UserName = adminEmail,
            EmailConfirmed = true,
            IsActive = true,
            PreferredLanguage = "en"
        };

        var result = await _userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogError("Admin auto-provisioning failed: {Errors}", errors);
            throw new InvalidOperationException($"Admin account creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(adminUser, "Admin");
        _logger.LogInformation("Admin account auto-provisioned for {Email}", adminEmail);
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new IdentityRole(roleName));
    }

    // FIX: Never use int.Parse on config values — use TryParse with a default.
    // int.Parse throws FormatException when the key is missing on the host.
    private static int ParseConfigInt(string? value, int defaultValue)
        => int.TryParse(value, out var parsed) ? parsed : defaultValue;
}