//using AutoMapper;
//using FoodOrdering.Application.Common.Models;
//using FoodOrdering.Application.Features.Auth.DTOs;
//using FoodOrdering.Domain.Entities;
//using MediatR;
//using Microsoft.AspNetCore.Identity;
//namespace FoodOrdering.Application.Features.Auth.Register.Commands
//{
//    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<UserDto>>
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IMapper _mapper;

//        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
//        {
//            _userManager = userManager;
//            _mapper = mapper;
//        }

//        public async Task<ApiResponse<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
//        {
//            var existingUser = await _userManager.FindByEmailAsync(request.Email);
//            if (existingUser != null)
//                return ApiResponse<UserDto>.Fail("Email already registered.");

//            var user = new ApplicationUser
//            {
//                FirstName = request.FirstName,
//                LastName = request.LastName,
//                Email = request.Email,
//                UserName = request.Email,
//                PhoneNumber = request.PhoneNumber,
//                Address = request.Address,
//                PreferredLanguage = request.PreferredLanguage
//            };

//            var result = await _userManager.CreateAsync(user, request.Password);
//            if (!result.Succeeded)
//            {
//                var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
//                return ApiResponse<UserDto>.ValidationFail(errors);
//            }

//            await _userManager.AddToRoleAsync(user, "Customer");

//            var dto = _mapper.Map<UserDto>(user);
//            dto.Roles = new List<string> { "Customer" };

//            return ApiResponse<UserDto>.Created(dto, "Registration successful.");
//        }
//    }
//}
using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Auth.DTOs;
using FoodOrdering.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FoodOrdering.Application.Features.Auth.Register.Commands;

/// <summary>
/// Handles customer self-registration.
///
/// Key rules enforced here:
/// 1. The admin e-mail (read from AdminAccount:Email in configuration) is permanently
///    reserved. Any attempt to register with it is rejected with 409 Conflict —
///    even if the admin account has not yet been created in the database.
/// 2. Any other e-mail already in the database is also rejected with 409 Conflict.
/// 3. On success the new user is assigned the "Customer" role automatically.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IConfiguration config)
    {
        _userManager = userManager;
        _mapper = mapper;
        _config = config;
    }

    public async Task<ApiResponse<UserDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // ── Guard 1: reserved admin e-mail ───────────────────────────────────
        // Read the configured admin e-mail and compare case-insensitively.
        // This blocks the admin address even before the admin account is created.
        var adminEmail = _config["AdminAccount:Email"] ?? string.Empty;

        if (string.Equals(request.Email, adminEmail, StringComparison.OrdinalIgnoreCase))
            return ApiResponse<UserDto>.Fail(
                "This email address is reserved and cannot be used for registration.", 409);

        // ── Guard 2: duplicate e-mail already in the database ─────────────────
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return ApiResponse<UserDto>.Fail("Email already registered.", 409);

        // ── Create the new customer account ───────────────────────────────────
        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,       // UserName == Email avoids username conflicts
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            PreferredLanguage = request.PreferredLanguage,
            EmailConfirmed = true,          // Skip e-mail confirmation for this prototype
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            // Return field-level errors (e.g. password policy violations)
            var errors = result.Errors.ToDictionary(
                e => e.Code,
                e => new[] { e.Description });

            return ApiResponse<UserDto>.ValidationFail(errors);
        }

        // Assign the default Customer role
        await _userManager.AddToRoleAsync(user, "Customer");

        // Map entity → DTO and attach role list
        var dto = _mapper.Map<UserDto>(user);
        dto.Roles = new List<string> { "Customer" };

        return ApiResponse<UserDto>.Created(dto, "Registration successful.");
    }
}
