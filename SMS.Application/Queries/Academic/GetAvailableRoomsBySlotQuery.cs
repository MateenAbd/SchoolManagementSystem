using System;
using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetAvailableRoomsBySlotQuery : IRequest<IEnumerable<ClassroomDto>>
    {
        public string AcademicYear { get; set; } = string.Empty;
        public byte DayOfWeek { get; set; }
        public int? PeriodNo { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}