using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    [Table("Driver")]
    public class Driver
    {
        public int Id { get; set; }
        public string? LicenceNumber { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? UserID { get; set; }
        public string? CreatedById { get; set; }
        public UserManager? User { get; set; }
    }
}
