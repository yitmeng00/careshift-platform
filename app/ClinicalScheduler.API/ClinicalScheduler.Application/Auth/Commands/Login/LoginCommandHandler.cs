using ClinicalScheduler.Application.Auth.Dtos;
using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using MediatR;


namespace ClinicalScheduler.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    IStaffRepository staffRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!staff.IsActive)
            throw new UnauthorizedException("Account is deactivated. Contact your administrator.");

        if (!passwordHasher.Verify(request.Password, staff.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var token = tokenService.GenerateAccessToken(staff);

        return new LoginResponseDto(
            AccessToken: token,
            ExpiresIn: 3600,
            Staff: new StaffProfileDto(
                Id: staff.Id,
                FullName: staff.FullName,
                Email: staff.Email,
                Role: staff.Role.ToString(),
                Department: staff.Department?.Name ?? string.Empty,
                Initials: staff.Initials));
    }
}
