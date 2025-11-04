namespace SMS.Core.Entities
{
    public class FeeReceiptItem
    {
        public int ReceiptItemId { get; set; }
        public int ReceiptId { get; set; }
        public int HeadId { get; set; }
        public decimal Amount { get; set; }
    }
}