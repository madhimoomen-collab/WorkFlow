namespace Domain.DTOs.Requests
{
    public record UpdateEdgeRequest(string? Name, string? Condition, Guid TargetId);
}