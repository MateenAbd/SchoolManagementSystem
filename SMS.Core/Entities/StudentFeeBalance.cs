namespace SMS.Core.Entities
{
    public class StudentFeeBalance
    {
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int? TermId { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Balance => TotalDebit - TotalCredit;
    }
}