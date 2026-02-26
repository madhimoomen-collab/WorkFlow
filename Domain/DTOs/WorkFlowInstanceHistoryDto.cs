namespace Domain.DTOs
{
    public class WorkFlowInstanceHistoryDto
    {
        public Guid Id { get; set; }
        public Guid WorkFlowInstanceId { get; set; }
        public Guid FromNodeId { get; set; }
        public Guid ToNodeId { get; set; }
        public NodeDto? FromNode { get; set; }
        public NodeDto? ToNode { get; set; }
        public string? Comment { get; set; }
        public DateTime TransitionedAt { get; set; }
    }
}
