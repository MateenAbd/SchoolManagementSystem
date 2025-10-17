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
    }
}