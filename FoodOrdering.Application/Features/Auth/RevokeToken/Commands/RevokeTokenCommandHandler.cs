using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Auth.RevokeToken.Commands
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _uow;

        public RevokeTokenCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<bool>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return ApiResponse<bool>.Fail("Token is required.", 400);

            var token = await _uow.RefreshTokens.Query()
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);          

            if (token is null)
                return ApiResponse<bool>.Fail("Token not found.", 404);

            if (!token.IsActive)
                return ApiResponse<bool>.Fail("Token is already revoked or expired.", 400);


            token.IsRevoked = true;
            await _uow.RefreshTokens.UpdateAsync(token, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Token revoked successfully.");

        }
    }
}
