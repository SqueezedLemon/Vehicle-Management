using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    public class Notification
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool EmailSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; }

        public int RequestId { get; set; }
        public Request? Request { get; set; }

        public string? UserId { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
