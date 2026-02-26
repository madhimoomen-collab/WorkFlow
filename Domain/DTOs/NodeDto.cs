namespace Domain.DTOs
{
    public class NodeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RoleReq { get; set; }
        public Guid WorkFlowDefinitionId { get; set; }
    }
}
