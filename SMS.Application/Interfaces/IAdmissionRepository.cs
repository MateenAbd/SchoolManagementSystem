using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SMS.Core.Entities;

namespace SMS.Application.Interfaces
{
    public interface IAdmissionRepository
    {
        // Inquiries
        Task<int> CreateInquiryAsync(CancellationToken token, AdmissionInquiry inquiry);
        Task<int> UpdateInquiryAsync(CancellationToken token, AdmissionInquiry inquiry);
        Task<int> UpdateInquiryStatusAsync(CancellationToken token, int inquiryId, string leadStatus);
        Task<AdmissionInquiry?> GetInquiryByIdAsync(CancellationToken token, int inquiryId);
        Task<IEnumerable<AdmissionInquiry>> GetInquiryListAsync(CancellationToken token, string? academicYear, string? interestedClass, string? leadStatus);

        // Applications
        Task<int> CreateApplicationAsync(CancellationToken token, AdmissionApplication app);
        Task<int> UpdateApplicationAsync(CancellationToken token, AdmissionApplication app);
        Task<int> UpdateApplicationStatusAsync(CancellationToken token, int applicationId, string status);
        Task<AdmissionApplication?> GetApplicationByIdAsync(CancellationToken token, int applicationId);
        Task<IEnumerable<AdmissionApplication>> GetApplicationListAsync(CancellationToken token, string? academicYear, string? classAppliedFor, string? status);




        // Fees
        Task<int> CollectApplicationFeeAsync(CancellationToken token, AdmissionFeePayment payment);
        Task<IEnumerable<AdmissionFeePayment>> GetApplicationFeesAsync(CancellationToken token, int applicationId);
        Task<AdmissionFeeSummary> GetApplicationFeeSummaryAsync(CancellationToken token, string? academicYear, string? classAppliedFor);

        // Shortlist and Merit
        Task<int> GenerateShortlistAsync(CancellationToken token, string academicYear, string classAppliedFor, decimal? minEntranceScore, int? topN);
        Task<IEnumerable<AdmissionShortlistItem>> GetShortlistAsync(CancellationToken token, string academicYear, string classAppliedFor);

        Task<int> GenerateMeritListAsync(CancellationToken token, string academicYear, string classAppliedFor, int? topN);
        Task<IEnumerable<AdmissionMeritItem>> GetMeritListAsync(CancellationToken token, string academicYear, string classAppliedFor);

        // Application Documents
        Task<int> AddApplicationDocumentAsync(CancellationToken token, AdmissionApplicationDocument doc);
        Task<IEnumerable<AdmissionApplicationDocument>> GetApplicationDocumentsAsync(CancellationToken token, int applicationId);
        Task<int> DeleteApplicationDocumentByIdAsync(CancellationToken token, int documentId);
        Task<int> VerifyApplicationDocumentAsync(CancellationToken token, int documentId, int verifiedByUserId, bool verified);

        Task<int> SetApplicationDocumentsVerifiedAsync(CancellationToken token, int applicationId, bool documentsVerified);

        // Admission Confirmation
        Task<int> ConfirmAdmissionAsync(CancellationToken token, int applicationId, string? section, DateTime enrollmentDate);
    }
}