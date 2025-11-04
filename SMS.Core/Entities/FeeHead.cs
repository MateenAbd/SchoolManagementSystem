namespace SMS.Core.Entities
{
    public class FeeHead
    {
        public int HeadId { get; set; }
        public string HeadCode { get; set; } = string.Empty; // unique
        public string HeadName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}