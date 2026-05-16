using ClinicalScheduler.Application.Auth.Dtos;
using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Auth.Commands.Refresh;

public class RefreshCommandHandler(
    IApplicationDbContext context,
    ITokenService tokenService)
    : IRequestHandler<RefreshCommand, LoginResult>
{
    public async Task<LoginResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.RefreshTokens
            .Include(rt => rt.Staff)
            .ThenInclude(s => s.Department)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (existing.IsRevoked)
            throw new UnauthorizedException("Refresh token has been revoked.");

        if (existing.ExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token has expired.");

        if (!existing.Staff.IsActive)
            throw new UnauthorizedException("Account is deactivated.");

        // Rotate: revoke old, issue new
        existing.IsRevoked = true;

        var newRefreshToken = tokenService.GenerateRefreshToken();
        context.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            StaffId = existing.StaffId,
            ExpiresAt = DateTime.UtcNow.AddDays(tokenService.RefreshTokenExpiryDays),
        });

        await context.SaveChangesAsync(cancellationToken);

        var accessToken = tokenService.GenerateAccessToken(existing.Staff);

        return new LoginResult(
            AccessToken: accessToken,
            RefreshToken: newRefreshToken,
            ExpiresIn: 15 * 60,
            Staff: new StaffProfileDto(
                Id: existing.Staff.Id,
                FullName: existing.Staff.FullName,
                Email: existing.Staff.Email,
                Role: existing.Staff.Role.ToString(),
                Department: existing.Staff.Department?.Name ?? string.Empty,
                Initials: existing.Staff.Initials));
    }
}