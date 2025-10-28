namespace SMS.Application.Dto
{
    public class ClassroomDto
    {
        public int RoomId { get; set; }
        public string RoomCode { get; set; } = string.Empty;
        public string? Name { get; set; }
        public int? Capacity { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }
    }
}