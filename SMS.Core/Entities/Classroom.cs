namespace SMS.Core.Entities
{
    public class Classroom
    {
        public int RoomId { get; set; }
        public string RoomCode { get; set; } = string.Empty;//unique
        public string? Name { get; set; }
        public int? Capacity { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
    }
}