namespace SMS.Application.Dto
{
    public class FeeHeadDto
    {
        public int HeadId { get; set; }
        public string HeadCode { get; set; } = string.Empty; //"TUI", "LAB"
        public string HeadName { get; set; } = string.Empty; //Tuition Fee
        public string? Description { get; set; }
        public int? SortOrder { get; set; } // for display ordering
        public bool IsActive { get; set; }
    }
}