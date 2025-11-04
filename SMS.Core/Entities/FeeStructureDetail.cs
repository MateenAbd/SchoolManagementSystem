namespace SMS.Core.Entities
{
    public class FeeStructureDetail
    {
        public int DetailId { get; set; }
        public int StructureId { get; set; }
        public int HeadId { get; set; }
        public decimal Amount { get; set; }
        public bool IsOptional { get; set; }
    }
}