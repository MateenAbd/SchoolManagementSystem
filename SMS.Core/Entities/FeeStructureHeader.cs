using System;

namespace SMS.Core.Entities
{
    public class FeeStructureHeader
    {
        public int StructureId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public int TermId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public bool IsActive { get; set; } = true;
    }
}