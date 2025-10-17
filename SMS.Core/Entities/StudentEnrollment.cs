using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Entities
{
    public class StudentEnrollment
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }
}
