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
    }
}
