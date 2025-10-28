namespace SMS.Core.Entities
{
    public class CourseSyllabus
    {
        public int SyllabusId { get; set; }
        public int CourseId { get; set; }
        public int? UnitNo { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string? SubTopic { get; set; }
        public string? Objectives { get; set; }
        public string? ReferenceMaterials { get; set; }
        public decimal? EstimatedHours { get; set; }
        public int? OrderIndex { get; set; }
    }
}