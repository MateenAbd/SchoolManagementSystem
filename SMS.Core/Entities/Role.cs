namespace SMS.Core.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
    }
}