namespace ClinicalScheduler.Application.Dashboard.Dtos;

public record ActivityEntryDto(
    int Id,
    string Action,
    string EntityType,
    int? EntityId,
    string? Detail,
    string? Icon,
    string PerformedBy,
    DateTime Timestamp);
