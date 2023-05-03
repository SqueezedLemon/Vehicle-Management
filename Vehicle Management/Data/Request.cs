using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
	[Table("Request")]
	public class Request
	{
		public int Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public string? PickupPoint { get; set; }
		public string? PickupPointLandmark { get; set; }
        public string? DropPoint { get; set; }
        public string? DropPointLandmark { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsUnapproved { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public bool IsNotified { get; set; } = false;

        public DateTime CreatedDate { get; set; }

        public int RequestStatusId { get; set; }
        public RequestStatus? RequestStatus { get; set; }

        public string? UserId { get; set; }
        public string? DriverUserId { get; set; }
        public string? CreatedbyId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
