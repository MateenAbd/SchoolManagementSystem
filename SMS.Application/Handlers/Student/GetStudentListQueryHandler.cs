using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SMS.Application.Handlers.Student
{
    public class GetStudentListQueryHandler : IRequestHandler<GetStudentListQuery, IEnumerable<StudentDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetStudentListQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentDto>> Handle(GetStudentListQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.StudentRepository.GetStudentListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<StudentDto>>(list);
        }
    }
}
