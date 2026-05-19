using ClinicalScheduler.Application.Swaps.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Swaps.Commands.Review;

public record ReviewSwapCommand(
    int SwapId,
    int CallerId,
    string CallerName,
    string Action,
    string? Note)
    : IRequest<SwapRequestDto>;