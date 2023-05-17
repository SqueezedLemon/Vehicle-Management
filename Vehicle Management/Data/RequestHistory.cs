using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    [Table("RequestHistory")]
    public class RequestHistory
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public int RequestId { get; set; }
        public Request? Request { get; set; }

        public int RequestStatusId { get; set; }
        public RequestStatus? RequestStatus { get; set; }

        public string? CreatedById { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
