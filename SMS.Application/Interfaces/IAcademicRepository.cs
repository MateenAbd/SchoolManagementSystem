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
    }
}