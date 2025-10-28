using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Academic
{
    public class UpsertTimetableEntryCommand : IRequest<int>
    {
        public TimetableEntryDto Entry { get; set; } = new();
    }
}