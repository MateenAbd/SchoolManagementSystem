using System;

namespace SMS.Core.Entities
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty; // e.g.,"Term 1"
        public string ExamType { get; set; } = "Term";//UnitTest/Term/Final/Other
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public DateTime? StartDate { get; set; }//optional summary dates
        public DateTime? EndDate { get; set; }
        public bool IsPublished { get; set; } = false;
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}