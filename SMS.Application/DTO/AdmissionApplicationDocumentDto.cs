using System;

namespace SMS.Application.Dto
{
    public class AdmissionApplicationDocumentDto
    {
        public int DocumentId { get; set; }
        public int ApplicationId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public string? Description { get; set; }
        public DateTime UploadedOn { get; set; }
        public bool Verified { get; set; }
        public int? VerifiedByUserId { get; set; }
        public DateTime? VerifiedAtUtc { get; set; }
    }
}