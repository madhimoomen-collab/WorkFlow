using System;
using System.Collections.Generic;
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

        // Navigation
        public ICollection<Node> Nodes { get; set; } = new List<Node>();
        public ICollection<WorkFlowInstance> Instances { get; set; } = new List<WorkFlowInstance>();
    }

    public class Node : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // e.g., 'Start', 'Task', 'End'

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? RoleReq { get; set; } // Specific role required for this node

        [ForeignKey("WorkFlowDefinition")]
        public Guid WorkFlowDefinitionId { get; set; }
        public WorkFlowDefinition WorkFlowDefinition { get; set; } = null!;

        // Navigation for Graph Edges
        public ICollection<Edge> OutgoingEdges { get; set; } = new List<Edge>();
    }

    public class Edge : BaseEntity
    {
        [MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(300)]
        public string? Condition { get; set; } // Logic for traversing this edge

        // Source Node
        [ForeignKey("SourceNode")]
        public Guid NodeId { get; set; }
        public Node SourceNode { get; set; } = null!;

        // Target Node
        [ForeignKey("TargetNode")]
        public Guid TargetId { get; set; }
        public Node TargetNode { get; set; } = null!;
    }
}