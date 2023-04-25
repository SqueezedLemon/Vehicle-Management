using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    [Table("RequestMessage")]
    public class RequestMessage
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedDate { get; set; }

        public int RequestId { get; set; }
        public Request? Request { get; set; }

        public string? CreatedById { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
