using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SMS.Core.Entities;

namespace SMS.Application.Interfaces
{
    public interface IFeeRepository
    {
        // Fee Heads
        Task<int> CreateFeeHeadAsync(CancellationToken token, FeeHead head);
        Task<int> UpdateFeeHeadAsync(CancellationToken token, FeeHead head);
        Task<int> DeleteFeeHeadAsync(CancellationToken token, int headId);
        Task<FeeHead?> GetFeeHeadByIdAsync(CancellationToken token, int headId);
        Task<IEnumerable<FeeHead>> GetFeeHeadListAsync(CancellationToken token, bool? isActive);

        // Fee Terms
        Task<int> CreateFeeTermAsync(CancellationToken token, FeeTerm term);
        Task<int> UpdateFeeTermAsync(CancellationToken token, FeeTerm term);
        Task<int> DeleteFeeTermAsync(CancellationToken token, int termId);
        Task<FeeTerm?> GetFeeTermByIdAsync(CancellationToken token, int termId);
        Task<IEnumerable<FeeTerm>> GetFeeTermListAsync(CancellationToken token, string? academicYear, bool? isActive);

        // Fee Structure (header + details)
        Task<int> UpsertFeeStructureAsync(CancellationToken token, FeeStructureHeader header, IEnumerable<FeeStructureDetail> details);
        Task<int> DeleteFeeStructureAsync(CancellationToken token, int structureId);
        Task<FeeStructureHeader?> GetFeeStructureHeaderByIdAsync(CancellationToken token, int structureId);
        Task<IEnumerable<FeeStructureDetail>> GetFeeStructureDetailsAsync(CancellationToken token, int structureId);
        Task<FeeStructureHeader?> GetFeeStructureHeaderByClassTermAsync(CancellationToken token, string academicYear, string className, string? section, int termId);
        Task<IEnumerable<FeeStructureHeader>> GetFeeStructureHeadersAsync(CancellationToken token, string? academicYear, string? className, string? section, int? termId, bool? isActive);

        //Fee demand (debits)
        Task<int> GenerateStudentTermFeeAsync(CancellationToken token, int studentId, string academicYear, int termId);
        
        //Fee collection (receipts + credits)
        Task<int> CreateFeeReceiptAsync(CancellationToken token, FeeReceipt receipt);
        Task<int> AddFeeReceiptItemAsync(CancellationToken token, FeeReceiptItem item);
        Task<int> PostLedgerCreditAsync(CancellationToken token, StudentFeeLedger entry); // for each item
        
        // Queries
        Task<FeeReceipt?> GetFeeReceiptByIdAsync(CancellationToken token, int receiptId);
        Task<IEnumerable<FeeReceipt>> GetFeeReceiptListAsync(CancellationToken token, string? academicYear, int? studentId, int? termId, DateTime? fromDate, DateTime? toDate, string? paymentMode);
        Task<IEnumerable<FeeReceiptItem>> GetFeeReceiptItemsAsync(CancellationToken token, int receiptId);

        Task<IEnumerable<StudentFeeLedger>> GetStudentLedgerAsync(CancellationToken token, int studentId, string? academicYear, int? termId);
        Task<StudentFeeBalance?> GetStudentFeeBalanceAsync(CancellationToken token, int studentId, string? academicYear, int? termId);

        //rules and adjustments
        Task<int> UpsertFeeFineRuleAsync(CancellationToken token, FeeFineRule rule);
        Task<int> DeleteFeeFineRuleAsync(CancellationToken token, int ruleId);
        Task<IEnumerable<FeeFineRule>> GetFeeFineRulesAsync(CancellationToken token, string? academicYear, string? className, string? section, int? termId, bool? isActive);

        Task<int> UpsertFeeDiscountSchemeAsync(CancellationToken token, FeeDiscountScheme scheme);
        Task<int> DeleteFeeDiscountSchemeAsync(CancellationToken token, int schemeId);
        Task<IEnumerable<FeeDiscountScheme>> GetFeeDiscountSchemesAsync(CancellationToken token, string? academicYear, string? className, string? section, int? termId, bool? isActive);

        Task<int> UpsertStudentScholarshipAsync(CancellationToken token, StudentScholarship scholarship);
        Task<int> DeleteStudentScholarshipAsync(CancellationToken token, int scholarshipId);
        Task<IEnumerable<StudentScholarship>> GetStudentScholarshipsAsync(CancellationToken token, int? studentId, string? academicYear, int? termId, bool? isActive);

        Task<int> ApplyLateFeeForTermAsync(CancellationToken token, string academicYear, int termId, DateTime asOfDate);
        Task<int> ApplyDiscountForStudentTermAsync(CancellationToken token, int studentId, string academicYear, int termId, int? schemeId, string? mode, decimal? value, decimal? capAmount);

        Task<int> InsertStudentFeeAdjustmentAsync(CancellationToken token, StudentFeeAdjustment adj);
        Task<IEnumerable<StudentFeeAdjustment>> GetStudentFeeAdjustmentsAsync(CancellationToken token, int? studentId, string? academicYear, int? termId, string? type);
    }
}