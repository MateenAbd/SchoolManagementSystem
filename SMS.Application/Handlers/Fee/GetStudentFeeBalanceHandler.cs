using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Dto;
using SMS.Application.Interfaces;
using SMS.Application.Queries.Fee;

namespace SMS.Application.Handlers.Fee
{
    public class GetStudentFeeBalanceHandler : IRequestHandler<GetStudentFeeBalanceQuery, StudentFeeBalanceDto?>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetStudentFeeBalanceHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<StudentFeeBalanceDto?> Handle(GetStudentFeeBalanceQuery request, CancellationToken cancellationToken)
        {
            var s = await _uow.FeeRepository.GetStudentFeeBalanceAsync(cancellationToken, request.StudentId, request.AcademicYear, request.TermId);
            return s is null ? null : _mapper.Map<StudentFeeBalanceDto>(s);
        }
    }
}