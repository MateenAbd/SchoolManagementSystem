using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Entities; // ParametersCollection
using SMS.Core.Interfaces;

namespace SMS.Infrastructure.Repositories
{
    public class FeeRepository : IFeeRepository
    {
        private readonly IRepository _db;
        public FeeRepository(IRepository db) { _db = db; }

        // Fee Heads
        public async Task<int> CreateFeeHeadAsync(CancellationToken token, FeeHead head)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@HeadCode", ParameterValue = head.HeadCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@HeadName", ParameterValue = head.HeadName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = head.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SortOrder", ParameterValue = head.SortOrder, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = head.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateFeeHead", p);
        }

        public async Task<int> UpdateFeeHeadAsync(CancellationToken token, FeeHead head)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@HeadId", ParameterValue = head.HeadId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@HeadCode", ParameterValue = head.HeadCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@HeadName", ParameterValue = head.HeadName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Description", ParameterValue = head.Description, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SortOrder", ParameterValue = head.SortOrder, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = head.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateFeeHead", p);
        }

        public async Task<int> DeleteFeeHeadAsync(CancellationToken token, int headId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@HeadId", ParameterValue = headId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteFeeHead", p);
        }

        public Task<FeeHead?> GetFeeHeadByIdAsync(CancellationToken token, int headId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@HeadId", ParameterValue = headId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<FeeHead>(token, "GetFeeHeadById", p);
        }

        public Task<IEnumerable<FeeHead>> GetFeeHeadListAsync(CancellationToken token, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<FeeHead>(token, "GetFeeHeadList", p);
        }

        // Fee Terms
        public async Task<int> CreateFeeTermAsync(CancellationToken token, FeeTerm term)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = term.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermCode", ParameterValue = term.TermCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermName", ParameterValue = term.TermName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SequenceNo", ParameterValue = term.SequenceNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DueDate", ParameterValue = term.DueDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = term.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateFeeTerm", p);
        }

        public async Task<int> UpdateFeeTermAsync(CancellationToken token, FeeTerm term)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@TermId", ParameterValue = term.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = term.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermCode", ParameterValue = term.TermCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermName", ParameterValue = term.TermName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SequenceNo", ParameterValue = term.SequenceNo, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DueDate", ParameterValue = term.DueDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = term.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateFeeTerm", p);
        }

        public async Task<int> DeleteFeeTermAsync(CancellationToken token, int termId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteFeeTerm", p);
        }

        public Task<FeeTerm?> GetFeeTermByIdAsync(CancellationToken token, int termId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<FeeTerm>(token, "GetFeeTermById", p);
        }

        public Task<IEnumerable<FeeTerm>> GetFeeTermListAsync(CancellationToken token, string? academicYear, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<FeeTerm>(token, "GetFeeTermList", p);
        }

        // Fee Structure
        public async Task<int> UpsertFeeStructureAsync(CancellationToken token, FeeStructureHeader header, IEnumerable<FeeStructureDetail> details)
        {
            // Upsert header
            var ph = new List<ParametersCollection>
            {
                new() { ParameterName = "@StructureId", ParameterValue = header.StructureId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = header.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = header.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = header.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = header.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EffectiveFrom", ParameterValue = header.EffectiveFrom, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = header.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            var structureId = await _db.ExecuteSpReturnValueAsync(token, "UpsertFeeStructureHeader", ph);

            // Reset details
            var pdDel = new List<ParametersCollection>
            {
                new() { ParameterName = "@StructureId", ParameterValue = structureId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            await _db.ExecuteSpReturnValueAsync(token, "DeleteFeeStructureDetails", pdDel);

            // Add details
            foreach (var d in details)
            {
                var pd = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StructureId", ParameterValue = structureId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@HeadId", ParameterValue = d.HeadId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Amount", ParameterValue = d.Amount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@IsOptional", ParameterValue = d.IsOptional, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
                };
                await _db.ExecuteSpReturnValueAsync(token, "AddFeeStructureDetail", pd);
            }

            return (int)structureId;
        }

        public async Task<int> DeleteFeeStructureAsync(CancellationToken token, int structureId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StructureId", ParameterValue = structureId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteFeeStructure", p);
        }

        public Task<FeeStructureHeader?> GetFeeStructureHeaderByIdAsync(CancellationToken token, int structureId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StructureId", ParameterValue = structureId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<FeeStructureHeader>(token, "GetFeeStructureHeaderById", p);
        }

        public Task<IEnumerable<FeeStructureDetail>> GetFeeStructureDetailsAsync(CancellationToken token, int structureId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StructureId", ParameterValue = structureId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<FeeStructureDetail>(token, "GetFeeStructureDetailsByStructureId", p);
        }

        public Task<FeeStructureHeader?> GetFeeStructureHeaderByClassTermAsync(CancellationToken token, string academicYear, string className, string? section, int termId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<FeeStructureHeader>(token, "GetFeeStructureHeaderByClassTerm", p);
        }

        public Task<IEnumerable<FeeStructureHeader>> GetFeeStructureHeadersAsync(CancellationToken token, string? academicYear, string? className, string? section, int? termId, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<FeeStructureHeader>(token, "GetFeeStructureHeaders", p);
        }

        // Generate fee demand (debits)
        public async Task<int> GenerateStudentTermFeeAsync(CancellationToken token, int studentId, string academicYear, int termId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "GenerateStudentTermFee", p);
        }

        // Receipts
        public async Task<int> CreateFeeReceiptAsync(CancellationToken token, FeeReceipt receipt)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = receipt.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = receipt.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = receipt.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PaymentMode", ParameterValue = receipt.PaymentMode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReferenceNo", ParameterValue = receipt.ReferenceNo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TotalAmount", ParameterValue = receipt.TotalAmount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReceiptDate", ParameterValue = receipt.ReceiptDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReceivedByUserId", ParameterValue = receipt.ReceivedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateFeeReceipt", p);
        }

        public async Task<int> AddFeeReceiptItemAsync(CancellationToken token, FeeReceiptItem item)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ReceiptId", ParameterValue = item.ReceiptId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@HeadId", ParameterValue = item.HeadId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Amount", ParameterValue = item.Amount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "AddFeeReceiptItem", p);
        }

        public async Task<int> PostLedgerCreditAsync(CancellationToken token, StudentFeeLedger entry)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = entry.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = entry.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = entry.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@HeadId", ParameterValue = entry.HeadId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EntryType", ParameterValue = entry.EntryType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Amount", ParameterValue = entry.Amount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Narration", ParameterValue = entry.Narration, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReceiptId", ParameterValue = entry.ReceiptId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EntryDate", ParameterValue = entry.EntryDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "InsertStudentFeeLedgerEntry", p);
        }

        public Task<FeeReceipt?> GetFeeReceiptByIdAsync(CancellationToken token, int receiptId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ReceiptId", ParameterValue = receiptId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<FeeReceipt>(token, "GetFeeReceiptById", p);
        }

        public Task<IEnumerable<FeeReceipt>> GetFeeReceiptListAsync(CancellationToken token, string? academicYear, int? studentId, int? termId, DateTime? fromDate, DateTime? toDate, string? paymentMode)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@FromDate", ParameterValue = fromDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ToDate", ParameterValue = toDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PaymentMode", ParameterValue = paymentMode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<FeeReceipt>(token, "GetFeeReceiptList", p);
        }

        public Task<IEnumerable<FeeReceiptItem>> GetFeeReceiptItemsAsync(CancellationToken token, int receiptId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ReceiptId", ParameterValue = receiptId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<FeeReceiptItem>(token, "GetFeeReceiptItemsByReceiptId", p);
        }

        public Task<IEnumerable<StudentFeeLedger>> GetStudentLedgerAsync(CancellationToken token, int studentId, string? academicYear, int? termId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StudentFeeLedger>(token, "GetStudentFeeLedger", p);
        }

        public Task<StudentFeeBalance?> GetStudentFeeBalanceAsync(CancellationToken token, int studentId, string? academicYear, int? termId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<StudentFeeBalance>(token, "GetStudentFeeBalance", p);
        }

        // Fine Rules
        public async Task<int> UpsertFeeFineRuleAsync(CancellationToken token, FeeFineRule rule)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@RuleId", ParameterValue = rule.RuleId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = rule.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = rule.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = rule.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = rule.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@GraceDays", ParameterValue = rule.GraceDays, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Mode", ParameterValue = rule.Mode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Rate", ParameterValue = rule.Rate, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@MaxAmount", ParameterValue = rule.MaxAmount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@FineHeadId", ParameterValue = rule.FineHeadId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = rule.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpsertFeeFineRule", p);
        }

        public async Task<int> DeleteFeeFineRuleAsync(CancellationToken token, int ruleId)
        {
            var p = new List<ParametersCollection> { new() { ParameterName = "@RuleId", ParameterValue = ruleId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input } };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteFeeFineRule", p);
        }

        public Task<IEnumerable<FeeFineRule>> GetFeeFineRulesAsync(CancellationToken token, string? ay, string? className, string? section, int? termId, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = ay, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<FeeFineRule>(token, "GetFeeFineRules", p);
        }

        // Discount Schemes
        public async Task<int> UpsertFeeDiscountSchemeAsync(CancellationToken token, FeeDiscountScheme s)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@SchemeId", ParameterValue = s.SchemeId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SchemeCode", ParameterValue = s.SchemeCode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SchemeName", ParameterValue = s.SchemeName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = s.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = s.ClassName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = s.Section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = s.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Mode", ParameterValue = s.Mode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Value", ParameterValue = s.Value, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CapAmount", ParameterValue = s.CapAmount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@DiscountHeadId", ParameterValue = s.DiscountHeadId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = s.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpsertFeeDiscountScheme", p);
        }

        public async Task<int> DeleteFeeDiscountSchemeAsync(CancellationToken token, int schemeId)
        {
            var p = new List<ParametersCollection> { new() { ParameterName = "@SchemeId", ParameterValue = schemeId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input } };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteFeeDiscountScheme", p);
        }

        public Task<IEnumerable<FeeDiscountScheme>> GetFeeDiscountSchemesAsync(CancellationToken token, string? ay, string? className, string? section, int? termId, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = ay, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ClassName", ParameterValue = className, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Section", ParameterValue = section, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<FeeDiscountScheme>(token, "GetFeeDiscountSchemes", p);
        }

        // Scholarships
        public async Task<int> UpsertStudentScholarshipAsync(CancellationToken token, StudentScholarship s)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ScholarshipId", ParameterValue = s.ScholarshipId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StudentId", ParameterValue = s.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = s.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = s.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SchemeId", ParameterValue = s.SchemeId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Mode", ParameterValue = s.Mode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Value", ParameterValue = s.Value, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CapAmount", ParameterValue = s.CapAmount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ScholarshipHeadId", ParameterValue = s.ScholarshipHeadId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = s.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpsertStudentScholarship", p);
        }

        public async Task<int> DeleteStudentScholarshipAsync(CancellationToken token, int scholarshipId)
        {
            var p = new List<ParametersCollection> { new() { ParameterName = "@ScholarshipId", ParameterValue = scholarshipId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input } };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "DeleteStudentScholarship", p);
        }

        public Task<IEnumerable<StudentScholarship>> GetStudentScholarshipsAsync(CancellationToken token, int? studentId, string? academicYear, int? termId, bool? isActive)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = academicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsActive", ParameterValue = isActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StudentScholarship>(token, "GetStudentScholarships", p);
        }

        // Apply (late fee / discount)
        public async Task<int> ApplyLateFeeForTermAsync(CancellationToken token, string ay, int termId, DateTime asOfDate)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@AcademicYear", ParameterValue = ay, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AsOfDate", ParameterValue = asOfDate, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "ApplyLateFeeForTerm", p);
        }

        public async Task<int> ApplyDiscountForStudentTermAsync(CancellationToken token, int studentId, string ay, int termId, int? schemeId, string? mode, decimal? value, decimal? cap)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = ay, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SchemeId", ParameterValue = schemeId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Mode", ParameterValue = mode, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Value", ParameterValue = value, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CapAmount", ParameterValue = cap, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "ApplyDiscountForStudentTerm", p);
        }

        // Adjustments
        public async Task<int> InsertStudentFeeAdjustmentAsync(CancellationToken token, StudentFeeAdjustment adj)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = adj.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = adj.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = adj.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@HeadId", ParameterValue = adj.HeadId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Type", ParameterValue = adj.Type, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Amount", ParameterValue = adj.Amount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Narration", ParameterValue = adj.Narration, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EntryDate", ParameterValue = adj.EntryDate, ParameterType = DbType.Date, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CreatedByUserId", ParameterValue = adj.CreatedByUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "InsertStudentFeeAdjustment", p);
        }

        public Task<IEnumerable<StudentFeeAdjustment>> GetStudentFeeAdjustmentsAsync(CancellationToken token, int? studentId, string? ay, int? termId, string? type)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = ay, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = termId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Type", ParameterValue = type, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<StudentFeeAdjustment>(token, "GetStudentFeeAdjustments", p);
        }

        public async Task<int> CreatePaymentOrderAsync(CancellationToken token, PaymentGatewayOrder order)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@GatewayName", ParameterValue = order.GatewayName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@StudentId", ParameterValue = order.StudentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AcademicYear", ParameterValue = order.AcademicYear, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@TermId", ParameterValue = order.TermId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Amount", ParameterValue = order.Amount, ParameterType = DbType.Decimal, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Currency", ParameterValue = order.Currency, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReturnUrl", ParameterValue = order.ReturnUrl, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@CallbackUrl", ParameterValue = order.CallbackUrl, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ItemsJson", ParameterValue = order.ItemsJson, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreatePaymentOrder", p);
        }

        public async Task<int> UpdatePaymentOrderStatusAsync(CancellationToken token, int orderId, string status, string? paymentId, string? gatewayOrderId, string? referenceNo)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@OrderId", ParameterValue = orderId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Status", ParameterValue = status, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PaymentId", ParameterValue = paymentId, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@GatewayOrderId", ParameterValue = gatewayOrderId, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReferenceNo", ParameterValue = referenceNo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdatePaymentOrderStatus", p);
        }

        public Task<PaymentGatewayOrder?> GetPaymentOrderByOrderNoAsync(CancellationToken token, string orderNo)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@OrderNo", ParameterValue = orderNo, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<PaymentGatewayOrder>(token, "GetPaymentOrderByOrderNo", p);
        }
        public Task<PaymentGatewayOrder?> GetPaymentOrderByOrderIdAsync(CancellationToken token, int orderId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@OrderId", ParameterValue = orderId, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<PaymentGatewayOrder>(token, "GetPaymentOrderByOrderId", p);
        }

        public async Task<int> MarkPaymentOrderReceiptedAsync(CancellationToken token, int orderId, int receiptId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@OrderId", ParameterValue = orderId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReceiptId", ParameterValue = receiptId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "MarkPaymentOrderReceipted", p);
        }

        public async Task<int> InsertPaymentGatewayEventAsync(CancellationToken token, PaymentGatewayEvent ev)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@OrderId", ParameterValue = ev.OrderId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@EventType", ParameterValue = ev.EventType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Payload", ParameterValue = ev.Payload, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "InsertPaymentGatewayEvent", p);
        }

        public Task<PaymentGatewayOrder?> GetPaymentOrderByGatewayOrderIdAsync(CancellationToken token, string gatewayOrderId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@GatewayOrderId", ParameterValue = gatewayOrderId, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<PaymentGatewayOrder>(token, "GetPaymentOrderByGatewayOrderId", p);
        }
    }
}