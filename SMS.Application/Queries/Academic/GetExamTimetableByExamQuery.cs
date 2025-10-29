using System.Collections.Generic;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Academic
{
    public class GetExamTimetableByExamQuery : IRequest<IEnumerable<ExamPaperDto>>
    {
        public int ExamId { get; set; }
    }
}