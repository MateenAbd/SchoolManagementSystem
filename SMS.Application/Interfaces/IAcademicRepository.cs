using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SMS.Core.Entities;

namespace SMS.Application.Interfaces
{
    public interface IAcademicRepository
    {
        // Subjects
        Task<int> CreateSubjectAsync(CancellationToken token, Subject subject);
        Task<int> UpdateSubjectAsync(CancellationToken token, Subject subject);
        Task<int> DeleteSubjectAsync(CancellationToken token, int subjectId);
        Task<Subject?> GetSubjectByIdAsync(CancellationToken token, int subjectId);
        Task<IEnumerable<Subject>> GetSubjectListAsync(CancellationToken token, bool? isActive);

        // Courses
        Task<int> CreateCourseAsync(CancellationToken token, Course course);
        Task<int> UpdateCourseAsync(CancellationToken token, Course course);
        Task<int> DeleteCourseAsync(CancellationToken token, int courseId);
        Task<Course?> GetCourseByIdAsync(CancellationToken token, int courseId);
        Task<IEnumerable<Course>> GetCourseListAsync(CancellationToken token, string? academicYear, string? className, int? subjectId, bool? isActive);

        // Syllabus
        Task<int> AddSyllabusItemAsync(CancellationToken token, CourseSyllabus item);
        Task<int> UpdateSyllabusItemAsync(CancellationToken token, CourseSyllabus item);
        Task<int> DeleteSyllabusItemAsync(CancellationToken token, int syllabusId);
        Task<IEnumerable<CourseSyllabus>> GetSyllabusByCourseAsync(CancellationToken token, int courseId);

        
        // Classrooms
        Task<int> CreateClassroomAsync(CancellationToken token, Classroom room);
        Task<int> UpdateClassroomAsync(CancellationToken token, Classroom room);
        Task<int> DeleteClassroomAsync(CancellationToken token, int roomId);
        Task<Classroom?> GetClassroomByIdAsync(CancellationToken token, int roomId);
        Task<IEnumerable<Classroom>> GetClassroomListAsync(CancellationToken token, bool? isActive);
        Task<IEnumerable<Classroom>> GetAvailableRoomsBySlotAsync(CancellationToken token, string academicYear, byte dayOfWeek, int? periodNo, TimeSpan? startTime, TimeSpan? endTime);

        // Timetable
        Task<int> UpsertTimetableEntryAsync(CancellationToken token, TimetableEntry entry);
        Task<int> DeleteTimetableEntryAsync(CancellationToken token, int timetableId);
        Task<IEnumerable<TimetableEntry>> GetClassTimetableAsync(CancellationToken token, string academicYear, string className, string? section);
        Task<IEnumerable<TimetableEntry>> GetTeacherTimetableAsync(CancellationToken token, string academicYear, int teacherUserId);
        Task<int> ValidateTimetableConflictAsync(CancellationToken token, TimetableEntry entry); // 0=no conflict, -1 class, -2 teacher, -3 room

        // Lesson Plans
        Task<int> CreateLessonPlanAsync(CancellationToken token, LessonPlan plan);
        Task<int> UpdateLessonPlanAsync(CancellationToken token, LessonPlan plan);
        Task<int> DeleteLessonPlanAsync(CancellationToken token, int planId);
        Task<int> UpdateLessonPlanStatusAsync(CancellationToken token, int planId, string status);
        Task<LessonPlan?> GetLessonPlanByIdAsync(CancellationToken token, int planId);
        Task<IEnumerable<LessonPlan>> GetLessonPlanListAsync(CancellationToken token, string? academicYear, string? className, string? section, int? subjectId, int? teacherUserId,DateTime? fromDate, DateTime? toDate, string? status);

        // Academic calendar
        Task<int> CreateCalendarEventAsync(CancellationToken token, AcademicCalendarEvent e);
        Task<int> UpdateCalendarEventAsync(CancellationToken token, AcademicCalendarEvent e);
        Task<int> DeleteCalendarEventAsync(CancellationToken token, int eventId);
        Task<AcademicCalendarEvent?> GetCalendarEventByIdAsync(CancellationToken token, int eventId);
        Task<IEnumerable<AcademicCalendarEvent>> GetCalendarEventsAsync(CancellationToken token, string? academicYear, DateTime? fromDate, DateTime? toDate, string? className, string? section, string? eventType, bool? isActive);
        // Exams
        Task<int> CreateExamAsync(CancellationToken token, Exam exam);
        Task<int> UpdateExamAsync(CancellationToken token, Exam exam);
        Task<int> DeleteExamAsync(CancellationToken token, int examId);
        Task<Exam?> GetExamByIdAsync(CancellationToken token, int examId);
        Task<IEnumerable<Exam>> GetExamListAsync(CancellationToken token, string? academicYear, string? className, string? section, string? examType, bool? isPublished);

        // Exam papers
        Task<int> UpsertExamPaperAsync(CancellationToken token, ExamPaper paper);  // returns >0 id or negative conflict code
        Task<int> DeleteExamPaperAsync(CancellationToken token, int paperId);
        Task<IEnumerable<ExamPaper>> GetExamTimetableByExamAsync(CancellationToken token, int examId);
        Task<IEnumerable<ExamPaper>> GetExamTimetableByClassAsync(CancellationToken token, string academicYear, string className, string? section);
        Task<int> ValidateExamPaperConflictAsync(CancellationToken token, ExamPaper paper); // 0 ok, -1 class conflict, -2 invigilator, -3 room
    }
}