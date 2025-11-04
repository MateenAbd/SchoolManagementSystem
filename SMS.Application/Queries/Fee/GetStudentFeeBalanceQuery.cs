using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetStudentFeeBalanceQuery : IRequest<StudentFeeBalanceDto?>
    {
        public int StudentId { get; set; }
        public string? AcademicYear { get; set; }
        public int? TermId { get; set; }
    }
}