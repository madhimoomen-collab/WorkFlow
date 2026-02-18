using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class WorkFlowInstanceHistory : BaseEntity
    {
        [ForeignKey("WorkFlowInstance")]
        public Guid WorkFlowInstanceId { get; set; }
        public WorkFlowInstance WorkFlowInstance { get; set; } = null!;

        [ForeignKey("FromNode")]
        public Guid FromNodeId { get; set; }
        public Node FromNode { get; set; } = null!;

        [ForeignKey("ToNode")]
        public Guid ToNodeId { get; set; }
        public Node ToNode { get; set; } = null!;

        public string? Comment { get; set; }

        public DateTime TransitionedAt { get; set; } = DateTime.UtcNow;
    }
}
