using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;

namespace SMS.Infrastructure.Repositories
{
    public class AcademicRepository : IAcademicRepository
    {
        private readonly IRepository _db;

        public AcademicRepository(IRepository db)
        {
            _db = db;
        }

        // Subjects
        public async Task<int> CreateSubjectAsync(CancellationToken token, Subject subject)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@SubjectCode", ParameterValue = subject.SubjectCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectName", ParameterValue = subject.SubjectName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ShortName", ParameterValue = subject.ShortName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = subject.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = subject.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateSubject", p);
        }

        public async Task<int> UpdateSubjectAsync(CancellationToken token, Subject subject)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@SubjectId", ParameterValue = subject.SubjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectCode", ParameterValue = subject.SubjectCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectName", ParameterValue = subject.SubjectName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ShortName", ParameterValue = subject.ShortName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = subject.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = subject.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateSubject", p);
        }

        public async Task<int> DeleteSubjectAsync(CancellationToken token, int subjectId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@SubjectId", ParameterValue = subjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteSubject", p);
        }

        public Task<Subject?> GetSubjectByIdAsync(CancellationToken token, int subjectId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@SubjectId", ParameterValue = subjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<Subject>(token, "GetSubjectById", p);
        }

        public Task<IEnumerable<Subject>> GetSubjectListAsync(CancellationToken token, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<Subject>(token, "GetSubjectList", p);
        }

        // Courses
        public async Task<int> CreateCourseAsync(CancellationToken token, Course course)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@SubjectId", ParameterValue = course.SubjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = course.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = course.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = course.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = course.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateCourse", p);
        }

        public async Task<int> UpdateCourseAsync(CancellationToken token, Course course)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@CourseId", ParameterValue = course.CourseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectId", ParameterValue = course.SubjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = course.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = course.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = course.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = course.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "UpdateCourse", p);
        }

        public async Task<int> DeleteCourseAsync(CancellationToken token, int courseId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@CourseId", ParameterValue = courseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "DeleteCourse", p);
        }

        public Task<Course?> GetCourseByIdAsync(CancellationToken token, int courseId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@CourseId", ParameterValue = courseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<Course>(token, "GetCourseById", p);
        }

        public Task<IEnumerable<Course>> GetCourseListAsync(CancellationToken token, string? academicYear, string? className, int? subjectId, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectId", ParameterValue = subjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<Course>(token, "GetCourseList", p);
        }

        // Syllabus
        public async Task<int> AddSyllabusItemAsync(CancellationToken token, CourseSyllabus item)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@CourseId", ParameterValue = item.CourseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UnitNo", ParameterValue = item.UnitNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Topic", ParameterValue = item.Topic, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubTopic", ParameterValue = item.SubTopic, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Objectives", ParameterValue = item.Objectives, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReferenceMaterials", ParameterValue = item.ReferenceMaterials, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EstimatedHours", ParameterValue = item.EstimatedHours, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@OrderIndex", ParameterValue = item.OrderIndex, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "AddSyllabusItem", p);
        }

        public async Task<int> UpdateSyllabusItemAsync(CancellationToken token, CourseSyllabus item)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@SyllabusId", ParameterValue = item.SyllabusId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CourseId", ParameterValue = item.CourseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UnitNo", ParameterValue = item.UnitNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Topic", ParameterValue = item.Topic, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubTopic", ParameterValue = item.SubTopic, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Objectives", ParameterValue = item.Objectives, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReferenceMaterials", ParameterValue = item.ReferenceMaterials, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EstimatedHours", ParameterValue = item.EstimatedHours, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@OrderIndex", ParameterValue = item.OrderIndex, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "UpdateSyllabusItem", p);
        }

        public async Task<int> DeleteSyllabusItemAsync(CancellationToken token, int syllabusId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@SyllabusId", ParameterValue = syllabusId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "DeleteSyllabusItem", p);
        }

        public Task<IEnumerable<CourseSyllabus>> GetSyllabusByCourseAsync(CancellationToken token, int courseId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@CourseId", ParameterValue = courseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<CourseSyllabus>(token, "GetSyllabusByCourse", p);
        }


        // Classrooms
        public async Task<int> CreateClassroomAsync(CancellationToken token, Classroom room)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@RoomCode", ParameterValue = room.RoomCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Name", ParameterValue = room.Name, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Capacity", ParameterValue = room.Capacity, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Location", ParameterValue = room.Location, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = room.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "CreateClassroom", p);
        }

        public async Task<int> UpdateClassroomAsync(CancellationToken token, Classroom room)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@RoomId", ParameterValue = room.RoomId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@RoomCode", ParameterValue = room.RoomCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Name", ParameterValue = room.Name, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Capacity", ParameterValue = room.Capacity, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Location", ParameterValue = room.Location, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = room.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "UpdateClassroom", p);
        }

        public async Task<int> DeleteClassroomAsync(CancellationToken token, int roomId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@RoomId", ParameterValue = roomId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "DeleteClassroom", p);
        }

        public Task<Classroom?> GetClassroomByIdAsync(CancellationToken token, int roomId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@RoomId", ParameterValue = roomId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<Classroom>(token, "GetClassroomById", p);
        }

        public Task<IEnumerable<Classroom>> GetClassroomListAsync(CancellationToken token, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<Classroom>(token, "GetClassroomList", p);
        }

        public Task<IEnumerable<Classroom>> GetAvailableRoomsBySlotAsync(CancellationToken token, string academicYear, byte dayOfWeek, int? periodNo, TimeSpan? startTime, TimeSpan? endTime)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DayOfWeek", ParameterValue = dayOfWeek, ParameterType = DbType.Byte, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PeriodNo", ParameterValue = periodNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartTime", ParameterValue = startTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndTime", ParameterValue = endTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<Classroom>(token, "GetAvailableRoomsBySlot", p);
        }

        // Timetable
        public async Task<int> UpsertTimetableEntryAsync(CancellationToken token, TimetableEntry entry)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@TimetableId", ParameterValue = entry.TimetableId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = entry.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = entry.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = entry.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DayOfWeek", ParameterValue = entry.DayOfWeek, ParameterType = DbType.Byte, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PeriodNo", ParameterValue = entry.PeriodNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartTime", ParameterValue = entry.StartTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndTime", ParameterValue = entry.EndTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectId", ParameterValue = entry.SubjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CourseId", ParameterValue = entry.CourseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TeacherUserId", ParameterValue = entry.TeacherUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@RoomId", ParameterValue = entry.RoomId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = entry.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "UpsertTimetableEntry", p);
        }

        public async Task<int> DeleteTimetableEntryAsync(CancellationToken token, int timetableId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@TimetableId", ParameterValue = timetableId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "DeleteTimetableEntry", p);
        }

        public Task<IEnumerable<TimetableEntry>> GetClassTimetableAsync(CancellationToken token, string academicYear, string className, string? section)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<TimetableEntry>(token, "GetClassTimetable", p);
        }

        public Task<IEnumerable<TimetableEntry>> GetTeacherTimetableAsync(CancellationToken token, string academicYear, int teacherUserId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TeacherUserId", ParameterValue = teacherUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<TimetableEntry>(token, "GetTeacherTimetable", p);
        }

        public async Task<int> ValidateTimetableConflictAsync(CancellationToken token, TimetableEntry entry)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@TimetableId", ParameterValue = entry.TimetableId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = entry.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = entry.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = entry.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DayOfWeek", ParameterValue = entry.DayOfWeek, ParameterType = DbType.Byte, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PeriodNo", ParameterValue = entry.PeriodNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartTime", ParameterValue = entry.StartTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndTime", ParameterValue = entry.EndTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TeacherUserId", ParameterValue = entry.TeacherUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@RoomId", ParameterValue = entry.RoomId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "ValidateTimetableConflict", p);
        }
    }
}