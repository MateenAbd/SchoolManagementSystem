using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Fee
{
    public class GetStudentLedgerQuery : IRequest<IEnumerable<StudentFeeLedgerDto>>
    {
        public int StudentId { get; set; }
        public string? AcademicYear { get; set; }
        public int? TermId { get; set; }
    }
}