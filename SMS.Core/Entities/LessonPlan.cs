using System;

namespace SMS.Core.Entities
{
    public class LessonPlan
    {
        public int PlanId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }

        public int? CourseId { get; set; }// optional;if set, SubjectId can be inferred; still keep SubjectId for flexibility
        public int? SubjectId { get; set; }//optional if CourseId covers

        public int TeacherUserId { get; set; }//required
        public DateTime PlanDate { get; set; }
        public int? PeriodNo { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public string Topic { get; set; } = string.Empty;
        public string? SubTopic { get; set; }
        public string? Objectives { get; set; }//learning objectives
        public string? Activities { get; set; }
        public string? Resources { get; set; }//materials/links
        public string? Homework { get; set; }
        public string? AssessmentMethods { get; set; } //quiz/worksheet/viva/etc.

        public string Status { get; set; } = "Draft"; //draft/planned/delivered
        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public DateTime? DeliveredAtUtc { get; set; }
    }
}