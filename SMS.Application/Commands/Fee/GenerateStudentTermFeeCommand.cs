using MediatR;

namespace SMS.Application.Commands.Fee
{
    public class GenerateStudentTermFeeCommand : IRequest<int>
    {
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermId { get; set; }
    }
}