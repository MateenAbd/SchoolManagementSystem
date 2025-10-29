using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetExamByIdHandler : IRequestHandler<GetExamByIdQuery, ExamDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetExamByIdHandler(IUnitOfWork uow, IMapper mapper) { 
            _uow = uow;
            _mapper = mapper; 
        }

        public async Task<ExamDto?> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
        {
            var e = await _uow.AcademicRepository.GetExamByIdAsync(cancellationToken, request.ExamId);
            return e is null ? null : _mapper.Map<ExamDto>(e);
        }
    }
}