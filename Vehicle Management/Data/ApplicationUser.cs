namespace Vehicle_Management.Data
{
    public class UserManager : IdentityUser
    {
        public string? Name { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
