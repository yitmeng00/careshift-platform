using MediatR;

namespace ClinicalScheduler.Application.Swaps.Commands.Cancel;

public record CancelSwapCommand(int SwapId, int CallerId, string CallerName) : IRequest;