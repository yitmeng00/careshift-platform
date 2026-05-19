using ClinicalScheduler.Application.Swaps.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Swaps.Commands.Submit;

public record SubmitSwapCommand(
    int RequesterId,
    string RequesterName,
    int RequesterShiftId,
    int RequesteeShiftId,
    string Reason)
    : IRequest<SwapRequestDto>;