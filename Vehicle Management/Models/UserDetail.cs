namespace Vehicle_Management.Models
{
    public class UserDetail
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int TotalRequests { get; set; }
        public bool IsDisabled { get; set; }
    }
}