namespace SMS.Application.Dto
{
    public class ClassAttendanceBulkItemDto
    {
        public int StudentId { get; set; }
        public string Status { get; set; } = "Present";
        public string? Remarks { get; set; }
    }
}