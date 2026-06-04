namespace Folha360.Application.DTOs;

public sealed record HealthResponse(string Status, Dictionary<string, HealthItem> Services);

public sealed record HealthItem(string Status, string? Description, TimeSpan Duration);

public sealed record ProblemDetailsResponse(
    string Type,
    string Title,
    int Status,
    string Detail,
    string Instance,
    Dictionary<string, string[]>? Errors);
