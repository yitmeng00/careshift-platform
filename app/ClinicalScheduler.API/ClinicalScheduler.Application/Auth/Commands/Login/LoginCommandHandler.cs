using ClinicalScheduler.Application.Auth.Dtos;
using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Domain.Entities;
using MediatR;

namespace ClinicalScheduler.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    IStaffRepository staffRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHasher,
    IApplicationDbContext context)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!staff.IsActive)
            throw new UnauthorizedException("Account is deactivated. Contact your administrator.");

        if (!passwordHasher.Verify(request.Password, staff.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var accessToken = tokenService.GenerateAccessToken(staff);
        var refreshToken = tokenService.GenerateRefreshToken();

        context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            StaffId = staff.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(tokenService.RefreshTokenExpiryDays),
        });

        await context.SaveChangesAsync(cancellationToken);

        return new LoginResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresIn: 15 * 60,
            Staff: new StaffProfileDto(
                Id: staff.Id,
                FullName: staff.FullName,
                Email: staff.Email,
                Role: staff.Role.ToString(),
                Department: staff.Department?.Name ?? string.Empty,
                Initials: staff.Initials));
    }
}