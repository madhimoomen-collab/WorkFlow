using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class WorkFlowDefinition : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public float Version { get; set; }

        public ICollection<Node> Nodes { get; set; } = new List<Node>();
        public ICollection<WorkFlowInstance> Instances { get; set; } = new List<WorkFlowInstance>();
    }

    public class Node : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? RoleReq { get; set; }

        [ForeignKey("WorkFlowDefinition")]
        public Guid WorkFlowDefinitionId { get; set; }
        public WorkFlowDefinition WorkFlowDefinition { get; set; } = null!;

        public ICollection<Edge> OutgoingEdges { get; set; } = new List<Edge>();
    }

    public class Edge : BaseEntity
    {
        [MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(300)]
        public string? Condition { get; set; }

        [ForeignKey("SourceNode")]
        public Guid NodeId { get; set; }
        public Node SourceNode { get; set; } = null!;

        [ForeignKey("TargetNode")]
        public Guid TargetId { get; set; }
        public Node TargetNode { get; set; } = null!;
    }
}