using ClinicalScheduler.Application.Auth.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;
