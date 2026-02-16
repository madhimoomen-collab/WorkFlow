using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class WorkFlowInstance : BaseEntity
    {
        [ForeignKey("WorkFlowDefinition")]
        public Guid WorkFlowDefinitionId { get; set; }
        public WorkFlowDefinition WorkFlowDefinition { get; set; } = null!;

        [ForeignKey("CurrentNode")]
        public Guid NodeId { get; set; }
        public Node CurrentNode { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime? CompletedAt { get; set; }

        [MaxLength(200)]
        public string InitiatedBy { get; set; } = string.Empty;
    }
}