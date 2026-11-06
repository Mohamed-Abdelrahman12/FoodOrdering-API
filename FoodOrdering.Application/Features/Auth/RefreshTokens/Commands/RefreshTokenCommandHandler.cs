using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Auth.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FoodOrdering.Application.Features.Auth.RefreshTokens.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResultDto>>
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public RefreshTokenCommandHandler(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork uow,
            IMapper mapper,
            IConfiguration config)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _uow = uow;
            _mapper = mapper;
            _config = config;
        }

        public async Task<ApiResponse<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            
            var storedToken = await _uow.RefreshTokens.Query()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

            if (storedToken is null || !storedToken.IsActive)
                return ApiResponse<AuthResultDto>.Fail("Invalid or expired refresh token.", 401);

            var user = storedToken.User;

            if (!user.IsActive)
                return ApiResponse<AuthResultDto>.Fail("User account is inactive.", 401);

            storedToken.IsRevoked = true;
            await _uow.RefreshTokens.UpdateAsync(storedToken, cancellationToken);


            
            var now = DateTime.UtcNow;
            var staleTokens = await _uow.RefreshTokens.Query()
                .Where(t => t.UserId == user.Id && (t.IsRevoked || t.ExpiresAt <= now))
                .ToListAsync(cancellationToken);

            foreach (var stale in staleTokens)
                await _uow.RefreshTokens.DeleteAsync(stale, cancellationToken);

            var expiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");
            var newRefreshTokenEntity = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
            };

            await _uow.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var expiryMinutes = int.Parse(_config["Jwt:AccessTokenExpiryMinutes"] ?? "60");

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles.ToList();

            return ApiResponse<AuthResultDto>.Ok(new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenEntity.Token,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(expiryMinutes),
                User = userDto
            });
        }
    }
}
