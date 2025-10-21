using MediatR;

namespace SMS.Application.Commands.Admission
{
    public class GenerateMeritListCommand : IRequest<int>
    {
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassAppliedFor { get; set; } = string.Empty;
        public int? TopN { get; set; }
        public string? Notes { get; set; }
    }
}