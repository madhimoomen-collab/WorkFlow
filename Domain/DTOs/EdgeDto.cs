namespace Domain.DTOs
{
    public class EdgeDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Condition { get; set; }
        public Guid NodeId { get; set; }
        public Guid TargetId { get; set; }
        public NodeDto? TargetNode { get; set; }
    }
}
