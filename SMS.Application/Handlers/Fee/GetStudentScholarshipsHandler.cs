using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;

namespace SMS.Application.Handlers.Fee
{
    public class GetStudentScholarshipsHandler : IRequestHandler<GetStudentScholarshipsQuery, IEnumerable<StudentScholarshipDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetStudentScholarshipsHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<StudentScholarshipDto>> Handle(GetStudentScholarshipsQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetStudentScholarshipsAsync(cancellationToken, request.StudentId, request.AcademicYear, request.TermId, request.IsActive);
            return _mapper.Map<IEnumerable<StudentScholarshipDto>>(list);
        }
    }
}