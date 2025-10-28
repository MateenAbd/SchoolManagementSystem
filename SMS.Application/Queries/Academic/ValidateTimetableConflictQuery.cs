using System;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class ValidateTimetableConflictQuery : IRequest<int>
    {
        public TimetableEntryDto Entry { get; set; } = new();//returns 0=no conflict, -1 class, -2 teacher, -3 room
    }
}