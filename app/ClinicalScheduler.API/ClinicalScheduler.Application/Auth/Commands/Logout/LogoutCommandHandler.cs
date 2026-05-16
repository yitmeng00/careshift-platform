using ClinicalScheduler.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Auth.Commands.Logout;

public class LogoutCommandHandler(IApplicationDbContext context)
    : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var token = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (token is { IsRevoked: false })
        {
            token.IsRevoked = true;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}