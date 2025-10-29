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

        // ---- Lesson Plans ----
        public async Task<int> CreateLessonPlanAsync(CancellationToken token, LessonPlan plan)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = plan.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = plan.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = plan.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CourseId", ParameterValue = plan.CourseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectId", ParameterValue = plan.SubjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TeacherUserId", ParameterValue = plan.TeacherUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PlanDate", ParameterValue = plan.PlanDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PeriodNo", ParameterValue = plan.PeriodNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartTime", ParameterValue = plan.StartTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndTime", ParameterValue = plan.EndTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Topic", ParameterValue = plan.Topic, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubTopic", ParameterValue = plan.SubTopic, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Objectives", ParameterValue = plan.Objectives, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Activities", ParameterValue = plan.Activities, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Resources", ParameterValue = plan.Resources, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Homework", ParameterValue = plan.Homework, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AssessmentMethods", ParameterValue = plan.AssessmentMethods, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = plan.Status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Notes", ParameterValue = plan.Notes, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateLessonPlan", p);
        }

        public async Task<int> UpdateLessonPlanAsync(CancellationToken token, LessonPlan plan)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@PlanId", ParameterValue = plan.PlanId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = plan.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = plan.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = plan.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CourseId", ParameterValue = plan.CourseId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectId", ParameterValue = plan.SubjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TeacherUserId", ParameterValue = plan.TeacherUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PlanDate", ParameterValue = plan.PlanDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PeriodNo", ParameterValue = plan.PeriodNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartTime", ParameterValue = plan.StartTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndTime", ParameterValue = plan.EndTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Topic", ParameterValue = plan.Topic, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubTopic", ParameterValue = plan.SubTopic, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Objectives", ParameterValue = plan.Objectives, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Activities", ParameterValue = plan.Activities, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Resources", ParameterValue = plan.Resources, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Homework", ParameterValue = plan.Homework, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AssessmentMethods", ParameterValue = plan.AssessmentMethods, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = plan.Status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Notes", ParameterValue = plan.Notes, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateLessonPlan", p);
        }

        public async Task<int> DeleteLessonPlanAsync(CancellationToken token, int planId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@PlanId", ParameterValue = planId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "DeleteLessonPlan", p);
        }

        public async Task<int> UpdateLessonPlanStatusAsync(CancellationToken token, int planId, string status)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@PlanId", ParameterValue = planId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "UpdateLessonPlanStatus", p);
        }

        public Task<LessonPlan?> GetLessonPlanByIdAsync(CancellationToken token, int planId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@PlanId", ParameterValue = planId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<LessonPlan>(token, "GetLessonPlanById", p);
        }

        public Task<IEnumerable<LessonPlan>> GetLessonPlanListAsync(
            CancellationToken token,
            string? academicYear, string? className, string? section,
            int? subjectId, int? teacherUserId,
            DateTime? fromDate, DateTime? toDate, string? status)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectId", ParameterValue = subjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TeacherUserId", ParameterValue = teacherUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@FromDate", ParameterValue = fromDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ToDate", ParameterValue = toDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<LessonPlan>(token, "GetLessonPlanList", p);
        }

        // ---Academic Calendar---
        public async Task<int> CreateCalendarEventAsync(CancellationToken token, AcademicCalendarEvent e)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = e.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Title", ParameterValue = e.Title, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EventType", ParameterValue = e.EventType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartDate", ParameterValue = e.StartDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndDate", ParameterValue = e.EndDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsAllDay", ParameterValue = e.IsAllDay, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = e.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = e.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Location", ParameterValue = e.Location, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = e.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = e.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CreatedByUserId", ParameterValue = e.CreatedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateCalendarEvent", p);
        }

        public async Task<int> UpdateCalendarEventAsync(CancellationToken token, AcademicCalendarEvent e)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@EventId", ParameterValue = e.EventId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = e.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Title", ParameterValue = e.Title, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EventType", ParameterValue = e.EventType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartDate", ParameterValue = e.StartDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndDate", ParameterValue = e.EndDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsAllDay", ParameterValue = e.IsAllDay, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = e.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = e.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Location", ParameterValue = e.Location, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = e.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = e.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UpdatedByUserId", ParameterValue = e.UpdatedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateCalendarEvent", p);
        }

        public async Task<int> DeleteCalendarEventAsync(CancellationToken token, int eventId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@EventId", ParameterValue = eventId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteCalendarEvent", p);
        }

        public Task<AcademicCalendarEvent?> GetCalendarEventByIdAsync(CancellationToken token, int eventId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@EventId", ParameterValue = eventId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<AcademicCalendarEvent>(token, "GetCalendarEventById", p);
        }

        public Task<IEnumerable<AcademicCalendarEvent>> GetCalendarEventsAsync(
            CancellationToken token, string? academicYear, DateTime? fromDate, DateTime? toDate,
            string? className, string? section, string? eventType, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@FromDate", ParameterValue = fromDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ToDate", ParameterValue = toDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EventType", ParameterValue = eventType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<AcademicCalendarEvent>(token, "GetCalendarEvents", p);
        }

        // ----Exams---
        public async Task<int> CreateExamAsync(CancellationToken token, Exam exam)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = exam.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamName", ParameterValue = exam.ExamName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamType", ParameterValue = exam.ExamType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = exam.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = exam.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartDate", ParameterValue = exam.StartDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndDate", ParameterValue = exam.EndDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsPublished", ParameterValue = exam.IsPublished, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = exam.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateExam", p);
        }

        public async Task<int> UpdateExamAsync(CancellationToken token, Exam exam)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ExamId", ParameterValue = exam.ExamId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = exam.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamName", ParameterValue = exam.ExamName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamType", ParameterValue = exam.ExamType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = exam.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = exam.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartDate", ParameterValue = exam.StartDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndDate", ParameterValue = exam.EndDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsPublished", ParameterValue = exam.IsPublished, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = exam.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateExam", p);
        }

        public async Task<int> DeleteExamAsync(CancellationToken token, int examId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ExamId", ParameterValue = examId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int) await _db.ExecuteSpReturnValueAsync(token, "DeleteExam", p);
        }

        public Task<Exam?> GetExamByIdAsync(CancellationToken token, int examId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ExamId", ParameterValue = examId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<Exam>(token, "GetExamById", p);
        }

        public Task<IEnumerable<Exam>> GetExamListAsync(CancellationToken token, string? academicYear, string? className, string? section, string? examType, bool? isPublished)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamType", ParameterValue = examType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsPublished", ParameterValue = isPublished, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<Exam>(token, "GetExamList", p);
        }



        // ---- Exam Papers ----
        public async Task<int> UpsertExamPaperAsync(CancellationToken token, ExamPaper paper)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@PaperId", ParameterValue = paper.PaperId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamId", ParameterValue = paper.ExamId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SubjectId", ParameterValue = paper.SubjectId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamDate", ParameterValue = paper.ExamDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartTime", ParameterValue = paper.StartTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndTime", ParameterValue = paper.EndTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DurationMinutes", ParameterValue = paper.DurationMinutes, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@RoomId", ParameterValue = paper.RoomId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@InvigilatorUserId", ParameterValue = paper.InvigilatorUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@MaxMarks", ParameterValue = paper.MaxMarks, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PassingMarks", ParameterValue = paper.PassingMarks, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Notes", ParameterValue = paper.Notes, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = paper.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpsertExamPaper", p);
        }

        public async Task<int> DeleteExamPaperAsync(CancellationToken token, int paperId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@PaperId", ParameterValue = paperId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteExamPaper", p);
        }

        public Task<IEnumerable<ExamPaper>> GetExamTimetableByExamAsync(CancellationToken token, int examId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ExamId", ParameterValue = examId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<ExamPaper>(token, "GetExamTimetableByExam", p);
        }

        public Task<IEnumerable<ExamPaper>> GetExamTimetableByClassAsync(CancellationToken token, string academicYear, string className, string? section)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<ExamPaper>(token, "GetExamTimetableByClass", p);
        }

        public async Task<int> ValidateExamPaperConflictAsync(CancellationToken token, ExamPaper paper)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@PaperId", ParameterValue = paper.PaperId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamId", ParameterValue = paper.ExamId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ExamDate", ParameterValue = paper.ExamDate.Date, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StartTime", ParameterValue = paper.StartTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EndTime", ParameterValue = paper.EndTime, ParameterType = DbType.Time, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@RoomId", ParameterValue = paper.RoomId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@InvigilatorUserId", ParameterValue = paper.InvigilatorUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "ValidateExamPaperConflict", p);
        }
    }
}