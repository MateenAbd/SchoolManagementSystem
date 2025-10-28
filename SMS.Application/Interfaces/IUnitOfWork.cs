namespace SMS.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IStudentRepository StudentRepository { get; }
        IUserRepository UserRepository { get; }
        IAdmissionRepository AdmissionRepository { get; }
        IAttendanceRepository AttendanceRepository { get; }
        INotificationRepository NotificationRepository { get; }

        IAcademicRepository AcademicRepository { get; }
    }
}
