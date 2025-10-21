using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;
using SMS.Core.Logger.Interfaces;
using SMS.Core.Logger.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Infrastructure.Repositories
{
    public class AdmissionRepository : IAdmissionRepository
    {
        private readonly IRepository _db;
        private readonly ILog _logger;

        public AdmissionRepository(IRepository db)
        {
            _db = db;
            _logger = new LogService();
        }

        // Inquiries
        public async Task<int> CreateInquiryAsync(CancellationToken token, AdmissionInquiry inquiry)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@Source", ParameterValue = inquiry.Source, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@LeadStatus", ParameterValue = inquiry.LeadStatus, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ApplicantName", ParameterValue = inquiry.ApplicantName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Email", ParameterValue = inquiry.Email, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Phone", ParameterValue = inquiry.Phone, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@InterestedClass", ParameterValue = inquiry.InterestedClass, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AcademicYear", ParameterValue = inquiry.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Notes", ParameterValue = inquiry.Notes, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FollowUpDate", ParameterValue = inquiry.FollowUpDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateAdmissionInquiry", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CreateInquiry failed");
                throw;
            }
        }

        public async Task<int> UpdateInquiryAsync(CancellationToken token, AdmissionInquiry inquiry)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@InquiryId", ParameterValue = inquiry.InquiryId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Source", ParameterValue = inquiry.Source, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@LeadStatus", ParameterValue = inquiry.LeadStatus, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ApplicantName", ParameterValue = inquiry.ApplicantName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Email", ParameterValue = inquiry.Email, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Phone", ParameterValue = inquiry.Phone, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@InterestedClass", ParameterValue = inquiry.InterestedClass, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AcademicYear", ParameterValue = inquiry.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Notes", ParameterValue = inquiry.Notes, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FollowUpDate", ParameterValue = inquiry.FollowUpDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateAdmissionInquiry", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateInquiry failed");
                throw;
            }
        }

        public async Task<int> UpdateInquiryStatusAsync(CancellationToken token, int inquiryId, string leadStatus)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@InquiryId", ParameterValue = inquiryId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@LeadStatus", ParameterValue = leadStatus, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateAdmissionInquiryStatus", p);
        }

        public Task<AdmissionInquiry?> GetInquiryByIdAsync(CancellationToken token, int inquiryId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@InquiryId", ParameterValue = inquiryId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<AdmissionInquiry>(token, "GetAdmissionInquiryById", p);
        }

        public Task<IEnumerable<AdmissionInquiry>> GetInquiryListAsync(CancellationToken token, string? academicYear, string? interestedClass, string? leadStatus)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@InterestedClass", ParameterValue = interestedClass, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@LeadStatus", ParameterValue = leadStatus, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<AdmissionInquiry>(token, "GetAdmissionInquiryList", p);
        }

        // Applications
        public async Task<int> CreateApplicationAsync(CancellationToken token, AdmissionApplication app)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@InquiryId", ParameterValue = app.InquiryId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ApplicationNo", ParameterValue = string.IsNullOrWhiteSpace(app.ApplicationNo) ? null : app.ApplicationNo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ApplicantName", ParameterValue = app.ApplicantName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@DateOfBirth", ParameterValue = app.DateOfBirth, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Gender", ParameterValue = app.Gender, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Email", ParameterValue = app.Email, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Phone", ParameterValue = app.Phone, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Address", ParameterValue = app.Address, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ParentName", ParameterValue = app.ParentName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ParentPhone", ParameterValue = app.ParentPhone, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ParentEmail", ParameterValue = app.ParentEmail, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PreviousSchool", ParameterValue = app.PreviousSchool, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ClassAppliedFor", ParameterValue = app.ClassAppliedFor, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AcademicYear", ParameterValue = app.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Status", ParameterValue = app.Status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@TotalMarks", ParameterValue = app.TotalMarks, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@EntranceScore", ParameterValue = app.EntranceScore, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Category", ParameterValue = app.Category, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@DocumentsVerified", ParameterValue = app.DocumentsVerified, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ApplicationDate", ParameterValue = app.ApplicationDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateAdmissionApplication", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CreateApplication failed");
                throw;
            }
        }

        public async Task<int> UpdateApplicationAsync(CancellationToken token, AdmissionApplication app)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@ApplicationId", ParameterValue = app.ApplicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@InquiryId", ParameterValue = app.InquiryId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ApplicationNo", ParameterValue = app.ApplicationNo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ApplicantName", ParameterValue = app.ApplicantName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@DateOfBirth", ParameterValue = app.DateOfBirth, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Gender", ParameterValue = app.Gender, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Email", ParameterValue = app.Email, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Phone", ParameterValue = app.Phone, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Address", ParameterValue = app.Address, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ParentName", ParameterValue = app.ParentName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ParentPhone", ParameterValue = app.ParentPhone, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ParentEmail", ParameterValue = app.ParentEmail, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PreviousSchool", ParameterValue = app.PreviousSchool, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ClassAppliedFor", ParameterValue = app.ClassAppliedFor, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@AcademicYear", ParameterValue = app.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Status", ParameterValue = app.Status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@TotalMarks", ParameterValue = app.TotalMarks, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@EntranceScore", ParameterValue = app.EntranceScore, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Category", ParameterValue = app.Category, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@DocumentsVerified", ParameterValue = app.DocumentsVerified, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ApplicationDate", ParameterValue = app.ApplicationDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateAdmissionApplication", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateApplication failed");
                throw;
            }
        }

        public IRepository Get_db()
        {
            return _db;
        }

        public async Task<int> UpdateApplicationStatusAsync(CancellationToken token, int applicationId, string status)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ApplicationId", ParameterValue = applicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateAdmissionApplicationStatus", p);
        }

        public Task<AdmissionApplication?> GetApplicationByIdAsync(CancellationToken token, int applicationId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ApplicationId", ParameterValue = applicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<AdmissionApplication>(token, "GetAdmissionApplicationById", p);
        }

        public Task<IEnumerable<AdmissionApplication>> GetApplicationListAsync(CancellationToken token, string? academicYear, string? classAppliedFor, string? status)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassAppliedFor", ParameterValue = classAppliedFor, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<AdmissionApplication>(token, "GetAdmissionApplicationList", p);
        }


        // Fees
        public async Task<int> CollectApplicationFeeAsync(CancellationToken token, AdmissionFeePayment payment)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@ApplicationId", ParameterValue = payment.ApplicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Amount", ParameterValue = payment.Amount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Currency", ParameterValue = payment.Currency, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PaymentMode", ParameterValue = payment.PaymentMode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ReferenceNo", ParameterValue = payment.ReferenceNo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Remarks", ParameterValue = payment.Remarks, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@CollectedByUserId", ParameterValue = payment.CollectedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PaymentDate", ParameterValue = payment.PaymentDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "CollectAdmissionApplicationFee", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CollectApplicationFee failed");
                throw;
            }
        }

        public Task<IEnumerable<AdmissionFeePayment>> GetApplicationFeesAsync(CancellationToken token, int applicationId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ApplicationId", ParameterValue = applicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<AdmissionFeePayment>(token, "GetApplicationFees", p);
        }

        public Task<AdmissionFeeSummary> GetApplicationFeeSummaryAsync(CancellationToken token, string? academicYear, string? classAppliedFor)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassAppliedFor", ParameterValue = classAppliedFor, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<AdmissionFeeSummary>(token, "GetApplicationFeeSummary", p);
        }

        // Shortlist
        public async Task<int> GenerateShortlistAsync(CancellationToken token, string academicYear, string classAppliedFor, decimal? minEntranceScore, int? topN)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassAppliedFor", ParameterValue = classAppliedFor, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@MinEntranceScore", ParameterValue = minEntranceScore, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TopN", ParameterValue = topN, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "GenerateAdmissionShortlist", p);
        }

        public Task<IEnumerable<AdmissionShortlistItem>> GetShortlistAsync(CancellationToken token, string academicYear, string classAppliedFor)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassAppliedFor", ParameterValue = classAppliedFor, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<AdmissionShortlistItem>(token, "GetAdmissionShortlist", p);
        }

        // Merit
        public async Task<int> GenerateMeritListAsync(CancellationToken token, string academicYear, string classAppliedFor, int? topN)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassAppliedFor", ParameterValue = classAppliedFor, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TopN", ParameterValue = topN, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "GenerateAdmissionMeritList", p);
        }

        public Task<IEnumerable<AdmissionMeritItem>> GetMeritListAsync(CancellationToken token, string academicYear, string classAppliedFor)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassAppliedFor", ParameterValue = classAppliedFor, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<AdmissionMeritItem>(token, "GetAdmissionMeritList", p);
        }


        // Application Documents
        public async Task<int> AddApplicationDocumentAsync(CancellationToken token, AdmissionApplicationDocument doc)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@ApplicationId", ParameterValue = doc.ApplicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FileName", ParameterValue = doc.FileName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@FilePath", ParameterValue = doc.FilePath, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@ContentType", ParameterValue = doc.ContentType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Description", ParameterValue = doc.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "AddAdmissionApplicationDocument", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddApplicationDocument failed");
                throw;
            }
        }

        public Task<IEnumerable<AdmissionApplicationDocument>> GetApplicationDocumentsAsync(CancellationToken token, int applicationId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ApplicationId", ParameterValue = applicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<AdmissionApplicationDocument>(token, "GetAdmissionApplicationDocuments", p);
        }

        public async Task<int> DeleteApplicationDocumentByIdAsync(CancellationToken token, int documentId)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@DocumentId", ParameterValue = documentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteAdmissionApplicationDocumentById", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteApplicationDocument failed");
                throw;
            }
        }

        public async Task<int> VerifyApplicationDocumentAsync(CancellationToken token, int documentId, int verifiedByUserId, bool verified)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@DocumentId", ParameterValue = documentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@VerifiedByUserId", ParameterValue = verifiedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Verified", ParameterValue = verified, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "VerifyAdmissionApplicationDocument", p);
        }

        public async Task<int> SetApplicationDocumentsVerifiedAsync(CancellationToken token, int applicationId, bool documentsVerified)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ApplicationId", ParameterValue = applicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DocumentsVerified", ParameterValue = documentsVerified, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "SetApplicationDocumentsVerified", p);
        }

        // Confirm Admission
        public async Task<int> ConfirmAdmissionAsync(CancellationToken token, int applicationId, string? section, DateTime enrollmentDate)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ApplicationId", ParameterValue = applicationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EnrollmentDate", ParameterValue = enrollmentDate, ParameterType = DbType.DateTime, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "ConfirmAdmission", p); // returns StudentId
        }
    }
}