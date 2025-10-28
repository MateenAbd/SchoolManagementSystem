using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Academic;

namespace SMS.Application.Handlers.Academic
{
    public class GetClassroomByIdHandler : IRequestHandler<GetClassroomByIdQuery, ClassroomDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetClassroomByIdHandler(IUnitOfWork uow, IMapper mapper) 
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ClassroomDto?> Handle(GetClassroomByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.AcademicRepository.GetClassroomByIdAsync(cancellationToken, request.RoomId);
            return entity is null ? null : _mapper.Map<ClassroomDto>(entity);
        }
    }
}