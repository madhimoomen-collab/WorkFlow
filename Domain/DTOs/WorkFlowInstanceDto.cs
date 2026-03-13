namespace Domain.DTOs
{
    public class WorkFlowInstanceDto
    {
        public Guid Id { get; set; }
        public Guid WorkFlowDefinitionId { get; set; }
        public Guid? NodeId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public string InitiatedBy { get; set; } = string.Empty;
        public NodeDto? CurrentNode { get; set; }
    }
}
