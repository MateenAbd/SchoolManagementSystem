using System.Collections.Generic;
using System;

namespace SMS.Application.Dto
{
    // Full structure for a class/term (header + list of details)
    public class FeeStructureDto
    {
        public int StructureId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? Section { get; set; }
        public int TermId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public bool IsActive { get; set; } = true;

        public List<FeeStructureDetailDto> Details { get; set; } = new();
    }
}