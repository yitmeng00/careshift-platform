using ClinicalScheduler.Application.Swaps.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Swaps.Commands.Respond;

public record RespondSwapCommand(
    int SwapId,
    int CallerId,
    string CallerName,
    string Action,
    string? Note)
    : IRequest<SwapRequestDto>;