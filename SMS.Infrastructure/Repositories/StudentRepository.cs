using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;
using SMS.Core.Logger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;


namespace SMS.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IRepository _dbRepository;
        private readonly LogService _logger;

        public StudentRepository(IRepository dbRepository)
        {
            _dbRepository = dbRepository;
            _logger = new LogService();
        }

        public async Task<IEnumerable<Student>> GetStudentListAsync(CancellationToken token)
        {
            try
            {
                return await _dbRepository.ExecuteSpListAsync<Student>(token, "GetStudentList", null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStudentList failed");
                throw;
            }
        }

        public async Task<Student> GetStudentByIdAsync(CancellationToken token, int studentId)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpSingleAsync<Student>(token, "GetStudentById", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStudentById failed");
                throw;
            }
        }

        public async Task<long> AddStudentAsync(CancellationToken token, Student student)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@AdmissionNo", ParameterValue = student.AdmissionNo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FirstName", ParameterValue = student.FirstName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@LastName", ParameterValue = student.LastName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ClassName", ParameterValue = student.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Section", ParameterValue = student.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Gender", ParameterValue = student.Gender, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Email", ParameterValue = student.Email, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Phone", ParameterValue = student.Phone, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Address", ParameterValue = student.Address, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@GuardianName", ParameterValue = student.GuardianName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@HealthInfo", ParameterValue = student.HealthInfo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PhotoUrl", ParameterValue = student.PhotoUrl, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpReturnValueAsync(token, "AddStudent", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddStudent failed");
                throw;
            }
        }

        public async Task<long> UpdateStudentAsync(CancellationToken token, Student student)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = student.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AdmissionNo", ParameterValue = student.AdmissionNo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FirstName", ParameterValue = student.FirstName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@LastName", ParameterValue = student.LastName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ClassName", ParameterValue = student.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Section", ParameterValue = student.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Gender", ParameterValue = student.Gender, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Email", ParameterValue = student.Email, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Phone", ParameterValue = student.Phone, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Address", ParameterValue = student.Address, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@GuardianName", ParameterValue = student.GuardianName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@HealthInfo", ParameterValue = student.HealthInfo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PhotoUrl", ParameterValue = student.PhotoUrl, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpReturnValueAsync(token, "UpdateStudent", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateStudent failed");
                throw;
            }
        }

        public async Task<long> DeleteStudentByIdAsync(CancellationToken token, int studentId)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpReturnValueAsync(token, "DeleteStudentById", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteStudentById failed");
                throw;
            }
        }

        public async Task<long> SetStudentPhotoAsync(CancellationToken token, int studentId, string photoUrl)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PhotoUrl", ParameterValue = photoUrl, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpReturnValueAsync(token, "SetStudentPhoto", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SetStudentPhoto failed");
                throw;
            }
        }

        public async Task<long> AddStudentDocumentAsync(CancellationToken token, StudentDocument doc)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = doc.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FileName", ParameterValue = doc.FileName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FilePath", ParameterValue = doc.FilePath, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ContentType", ParameterValue = doc.ContentType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Description", ParameterValue = doc.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpReturnValueAsync(token, "AddStudentDocument", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddStudentDocument failed");
                throw;
            }
        }

        public async Task<IEnumerable<StudentDocument>> GetStudentDocumentsAsync(CancellationToken token, int studentId)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpListAsync<StudentDocument>(token, "GetStudentDocuments", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetStudentDocuments failed");
                throw;
            }
        }

        public async Task<long> DeleteStudentDocumentByIdAsync(CancellationToken token, int documentId)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@DocumentId", ParameterValue = documentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpReturnValueAsync(token, "DeleteStudentDocumentById", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteStudentDocumentById failed");
                throw;
            }
        }

        public async Task<long> AddOrUpdateEnrollmentAsync(CancellationToken token, StudentEnrollment enrollment)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@EnrollmentId", ParameterValue = enrollment.EnrollmentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@StudentId", ParameterValue = enrollment.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AcademicYear", ParameterValue = enrollment.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ClassName", ParameterValue = enrollment.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Section", ParameterValue = enrollment.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@EnrollmentDate", ParameterValue = enrollment.EnrollmentDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@IsActive", ParameterValue = enrollment.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpReturnValueAsync(token, "AddOrUpdateEnrollment", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddOrUpdateEnrollment failed");
                throw;
            }
        }

        public async Task<StudentEnrollment?> GetEnrollmentByStudentAsync(CancellationToken token, int studentId)
        {
            try
            {
                var parameters = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return await _dbRepository.ExecuteSpSingleAsync<StudentEnrollment>(token, "GetEnrollmentByStudent", parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetEnrollmentByStudent failed");
                throw;
            }
        }
    }
}
