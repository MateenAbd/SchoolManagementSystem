using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Core.Entities;

namespace SMS.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetStudentListAsync(CancellationToken token);
        Task<Student> GetStudentByIdAsync(CancellationToken token, int studentId);
        Task<long> AddStudentAsync(CancellationToken token, Student student);
        Task<long> UpdateStudentAsync(CancellationToken token, Student student);
        Task<long> DeleteStudentByIdAsync(CancellationToken token, int studentId);

        
        Task<long> SetStudentPhotoAsync(CancellationToken token, int studentId, string photoUrl);        
        Task<long> AddStudentDocumentAsync(CancellationToken token, StudentDocument doc);
        Task<IEnumerable<StudentDocument>> GetStudentDocumentsAsync(CancellationToken token, int studentId);
        Task<long> DeleteStudentDocumentByIdAsync(CancellationToken token, int documentId);      
        Task<long> AddOrUpdateEnrollmentAsync(CancellationToken token, StudentEnrollment enrollment);
        Task<StudentEnrollment?> GetEnrollmentByStudentAsync(CancellationToken token, int studentId);
    }
}
