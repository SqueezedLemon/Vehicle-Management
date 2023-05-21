using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    [Table("HubConnection")]
    public class HubConnection
    {
        public int Id { get; set; }
        public string? ConnectionId { get; set; }


        public string? UserId { get; set; }
        public UserManager? User { get; set; }

    }
}
