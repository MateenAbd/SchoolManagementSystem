using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Fee
{
    public class UpsertStudentScholarshipCommand : IRequest<int>
    {
        public StudentScholarshipDto Scholarship { get; set; } = new();
    }
}