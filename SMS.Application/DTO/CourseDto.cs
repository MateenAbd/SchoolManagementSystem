namespace SMS.Application.Dto
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}