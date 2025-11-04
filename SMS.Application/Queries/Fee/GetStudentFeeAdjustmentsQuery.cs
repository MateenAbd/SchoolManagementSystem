using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetStudentFeeAdjustmentsQuery : IRequest<IEnumerable<StudentFeeAdjustmentDto>>
    {
        public int? StudentId { get; set; }
        public string? AcademicYear { get; set; }
        public int? TermId { get; set; }
        public string? Type { get; set; } // Fine/Discount/Scholarship/WriteOff
    }
}