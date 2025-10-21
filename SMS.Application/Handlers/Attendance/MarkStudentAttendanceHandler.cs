using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;
using SMS.Core.Entities;

namespace SMS.Application.Handlers.Attendance
{
    public class MarkStudentAttendanceHandler : IRequestHandler<MarkStudentAttendanceCommand, int>
    {
        private readonly IUnitOfWork _uow;

        public MarkStudentAttendanceHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Handle(MarkStudentAttendanceCommand request, CancellationToken cancellationToken)
        {
            var entity = new StudentAttendance
            {
                StudentId = request.StudentId,
                AttendanceDate = request.AttendanceDate.Date,
                ClassName = request.ClassName,
                Section = request.Section,
                Status = request.Status,
                Remarks = request.Remarks,
                MarkedByUserId = request.MarkedByUserId,
                SubjectCode = request.SubjectCode,
                PeriodNo = request.PeriodNo
            };
            return _uow.AttendanceRepository.UpsertStudentAttendanceAsync(cancellationToken, entity);
        }
    }
}