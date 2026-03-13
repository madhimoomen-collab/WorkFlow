namespace Domain.DTOs.Requests
{
    public record UpdateNodeRequest(string Name, string Type, string? Description, string? RoleReq);
}