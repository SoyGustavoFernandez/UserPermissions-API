namespace UserPermissions.Application.DTOs
{
    public class KafkaMessageDto
    {
        public Guid Id { get; set; }
        public string NameOperation { get; set; }
        public int PermissionId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}