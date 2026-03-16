namespace Domain.DTOs
{
    public class WorkFlowDefinitionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Version { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}