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
    public class GetSubjectListHandler : IRequestHandler<GetSubjectListQuery, IEnumerable<SubjectDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSubjectListHandler(IUnitOfWork uow, IMapper mapper) {
            _uow = uow;
            _mapper = mapper; 
        }

        public async Task<IEnumerable<SubjectDto>> Handle(GetSubjectListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.AcademicRepository.GetSubjectListAsync(cancellationToken, request.IsActive);
            return _mapper.Map<IEnumerable<SubjectDto>>(list);
        }
    }
}