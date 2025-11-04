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
    public class GetStudentFeeAdjustmentsHandler : IRequestHandler<GetStudentFeeAdjustmentsQuery, IEnumerable<StudentFeeAdjustmentDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetStudentFeeAdjustmentsHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<IEnumerable<StudentFeeAdjustmentDto>> Handle(GetStudentFeeAdjustmentsQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FeeRepository.GetStudentFeeAdjustmentsAsync(cancellationToken, request.StudentId, request.AcademicYear, request.TermId, request.Type);
            return _mapper.Map<IEnumerable<StudentFeeAdjustmentDto>>(list);
        }
    }
}