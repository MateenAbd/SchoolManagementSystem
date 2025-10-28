namespace SMS.Core.Entities
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}