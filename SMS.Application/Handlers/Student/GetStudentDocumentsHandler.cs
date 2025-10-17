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
    public class GetStudentDocumentsHandler : IRequestHandler<GetStudentDocumentsQuery, IEnumerable<StudentDocumentDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetStudentDocumentsHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentDocumentDto>> Handle(GetStudentDocumentsQuery request, CancellationToken cancellationToken)
        {
            var docs = await _uow.StudentRepository.GetStudentDocumentsAsync(cancellationToken, request.StudentId);
            return _mapper.Map<IEnumerable<StudentDocumentDto>>(docs);
        }
    }
}
