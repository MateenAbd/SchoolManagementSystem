using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetTeacherTimetableQuery : IRequest<IEnumerable<TimetableEntryDto>>
    {
        public string AcademicYear { get; set; } = string.Empty;
        public int TeacherUserId { get; set; }
    }
}