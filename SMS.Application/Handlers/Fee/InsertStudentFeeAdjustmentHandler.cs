using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Fee
{
    public class InsertStudentFeeAdjustmentHandler : IRequestHandler<InsertStudentFeeAdjustmentCommand, int>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public InsertStudentFeeAdjustmentHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public Task<int> Handle(InsertStudentFeeAdjustmentCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<StudentFeeAdjustment>(request.Adjustment);
            return _uow.FeeRepository.InsertStudentFeeAdjustmentAsync(cancellationToken, entity);
        }
    }
}