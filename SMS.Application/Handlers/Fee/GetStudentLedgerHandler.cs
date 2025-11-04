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
    public class GetStudentLedgerHandler : IRequestHandler<GetStudentLedgerQuery, IEnumerable<StudentFeeLedgerDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetStudentLedgerHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<StudentFeeLedgerDto>> Handle(GetStudentLedgerQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetStudentLedgerAsync(cancellationToken, request.StudentId, request.AcademicYear, request.TermId);
            return _mapper.Map<IEnumerable<StudentFeeLedgerDto>>(list);
        }
    }
}