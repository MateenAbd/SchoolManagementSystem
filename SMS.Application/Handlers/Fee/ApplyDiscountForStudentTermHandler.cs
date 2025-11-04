using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Fee;
using SMS.Application.Interfaces;

namespace SMS.Application.Handlers.Fee
{
    public class ApplyDiscountForStudentTermHandler : IRequestHandler<ApplyDiscountForStudentTermCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public ApplyDiscountForStudentTermHandler(IUnitOfWork uow) { _uow = uow; }

        public Task<int> Handle(ApplyDiscountForStudentTermCommand request, CancellationToken cancellationToken) =>
            _uow.FeeRepository.ApplyDiscountForStudentTermAsync(cancellationToken,
                request.StudentId, request.AcademicYear, request.TermId, request.SchemeId, request.Mode, request.Value, request.CapAmount);
    }
}