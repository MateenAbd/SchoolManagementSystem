using System;

namespace SMS.Application.Dto
{
    public class ExamDto
    {
        public int ExamId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;//e.g.,"Term 1"
        public string ExamType { get; set; } = "Term";//UnitTest/Term/Final/Other
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublished { get; set; }
        public string? Description { get; set; }
    }
}