using ClinicalScheduler.Application.Auth.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<LoginResult>;