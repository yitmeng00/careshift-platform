using MediatR;

namespace ClinicalScheduler.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;