using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    [Table("AssignedVehicle")]
    public class AssignedVehicle
    {
        public int Id { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        public string? CreatedById { get; set; }
        public UserManager? User { get; set; }

        public int RequestStatusId { get; set; }
        public RequestStatus? RequestStatus { get; set; }
    }
}
