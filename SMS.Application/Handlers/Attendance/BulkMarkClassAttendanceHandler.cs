using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Attendance
{
    public class BulkMarkClassAttendanceHandler : IRequestHandler<BulkMarkClassAttendanceCommand, int>
    {
        private readonly IUnitOfWork _uow;

        public BulkMarkClassAttendanceHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> Handle(BulkMarkClassAttendanceCommand request, CancellationToken cancellationToken)
        {
            int count = 0;
            foreach (var item in request.Items)
            {
                var entity = new StudentAttendance
                {
                    StudentId = item.StudentId,
                    AttendanceDate = request.AttendanceDate.Date,
                    ClassName = request.ClassName,
                    Section = request.Section,
                    Status = item.Status,
                    Remarks = item.Remarks,
                    MarkedByUserId = request.MarkedByUserId,
                    SubjectCode = request.SubjectCode,
                    PeriodNo = request.PeriodNo
                };
                var id = await _uow.AttendanceRepository.UpsertStudentAttendanceAsync(cancellationToken, entity);
                if (id > 0) count++;
            }
            return count;
        }
    }
}