namespace SMS.Application.Dto
{
    public class FeeStructureDetailDto
    {
        public int DetailId { get; set; }
        public int HeadId { get; set; }
        public decimal Amount { get; set; }
        public bool IsOptional { get; set; }
    }
}