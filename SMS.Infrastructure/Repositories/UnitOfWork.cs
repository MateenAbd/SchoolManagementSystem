using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Application.Interfaces;
using SMS.Core.Interfaces;

namespace SMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IRepository _dbRepository;

        public UnitOfWork(IRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }

        private IStudentRepository _studentRepository;
        public IStudentRepository StudentRepository => _studentRepository ??= new StudentRepository(_dbRepository);
        

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_dbRepository);


        private IAdmissionRepository? _admissionRepository;
        public IAdmissionRepository AdmissionRepository => _admissionRepository ??= new AdmissionRepository(_dbRepository);

        private IAttendanceRepository? _attendanceRepository;
        public IAttendanceRepository AttendanceRepository => _attendanceRepository ??= new AttendanceRepository(_dbRepository);
    }
}