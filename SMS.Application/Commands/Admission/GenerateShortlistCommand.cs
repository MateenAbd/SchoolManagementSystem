using MediatR;

namespace SMS.Application.Commands.Admission
{
    public class GenerateShortlistCommand : IRequest<int>
    {
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassAppliedFor { get; set; } = string.Empty;
        public decimal? MinEntranceScore { get; set; }
        public int? TopN { get; set; }
        public string? Notes { get; set; }
    }
}