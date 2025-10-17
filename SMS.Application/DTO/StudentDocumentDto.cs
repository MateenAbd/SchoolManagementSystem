using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Dto
{
    public class StudentDocumentDto
    {
        public int DocumentId { get; set; }
        public int StudentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public string? Description { get; set; }
        public DateTime UploadedOn { get; set; }
    }
}