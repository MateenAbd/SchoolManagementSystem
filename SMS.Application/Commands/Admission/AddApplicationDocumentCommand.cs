using MediatR;

namespace SMS.Application.Commands.Admission
{
    public class AddApplicationDocumentCommand : IRequest<int>
    {
        public int ApplicationId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public string? Description { get; set; }
    }
}