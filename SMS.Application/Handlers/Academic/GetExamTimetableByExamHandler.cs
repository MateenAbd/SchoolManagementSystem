using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetExamTimetableByExamHandler : IRequestHandler<GetExamTimetableByExamQuery, IEnumerable<ExamPaperDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetExamTimetableByExamHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExamPaperDto>> Handle(GetExamTimetableByExamQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AcademicRepository.GetExamTimetableByExamAsync(cancellationToken, request.ExamId);
            return _mapper.Map<IEnumerable<ExamPaperDto>>(list);
        }
    }
}