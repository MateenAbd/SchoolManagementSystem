using System;

namespace SMS.Application.Dto
{
    public class ExamPaperDto
    {
        public int PaperId { get; set; } //0 for create,>0 for update
        public int ExamId { get; set; }//fk to Exam (contains class/section)
        public int SubjectId { get; set; }//fk to Subjects
        public DateTime ExamDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public int? RoomId { get; set; }//fk to Classrooms
        public int? InvigilatorUserId { get; set; }//fk to Users
        public int MaxMarks { get; set; }
        public int? PassingMarks { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}