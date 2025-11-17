using System;

namespace SMS.Core.Entities
{
    public class MessageReceipt
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string ReceiptType { get; set; } = "Delivered";//Delivered/Read
        public DateTime ReceiptAtUtc { get; set; }
    }
}