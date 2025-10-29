using System;

namespace SMS.Application.Dto
{
    public class LessonPlanDto
    {
        public int PlanId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }

        public int? CourseId { get; set; }
        public int? SubjectId { get; set; }

        public int TeacherUserId { get; set; }
        public DateTime PlanDate { get; set; }
        public int? PeriodNo { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public string Topic { get; set; } = string.Empty;
        public string? SubTopic { get; set; }
        public string? Objectives { get; set; }
        public string? Activities { get; set; }
        public string? Resources { get; set; }
        public string? Homework { get; set; }
        public string? AssessmentMethods { get; set; }

        public string Status { get; set; } = "Draft";
        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public DateTime? DeliveredAtUtc { get; set; }
    }
}