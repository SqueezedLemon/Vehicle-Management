using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    [Table("RequestStatus")]
    public class RequestStatus
    {
        public int Id { get; set; }
        public string? RequestStatusName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? CreatedById  { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
