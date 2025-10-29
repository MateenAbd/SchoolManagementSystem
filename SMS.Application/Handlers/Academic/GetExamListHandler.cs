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
    public class GetExamListHandler : IRequestHandler<GetExamListQuery, IEnumerable<ExamDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetExamListHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow; 
            _mapper = mapper; 
        }

        public async Task<IEnumerable<ExamDto>> Handle(GetExamListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AcademicRepository.GetExamListAsync(
                cancellationToken, request.AcademicYear, request.ClassName, request.Section, request.ExamType, request.IsPublished);
            return _mapper.Map<IEnumerable<ExamDto>>(list);
        }
    }
}