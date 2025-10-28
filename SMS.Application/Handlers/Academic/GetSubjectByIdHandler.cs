using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetSubjectByIdHandler : IRequestHandler<GetSubjectByIdQuery, SubjectDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSubjectByIdHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow; 
            _mapper = mapper;
        }

        public async Task<SubjectDto?> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.AcademicRepository.GetSubjectByIdAsync(cancellationToken, request.SubjectId);
            return entity is null ? null : _mapper.Map<SubjectDto>(entity);
        }
    }
}