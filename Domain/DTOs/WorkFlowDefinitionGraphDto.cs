namespace Domain.DTOs
{
    public class WorkFlowDefinitionGraphDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Version { get; set; }
        public ICollection<NodeGraphDto> Nodes { get; set; } = new List<NodeGraphDto>();
    }

    public class NodeGraphDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RoleReq { get; set; }
        public Guid WorkFlowDefinitionId { get; set; }
        public ICollection<EdgeDto> OutgoingEdges { get; set; } = new List<EdgeDto>();
    }
}
