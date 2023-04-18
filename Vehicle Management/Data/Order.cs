using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
	[Table("Order")]
	public class Order
	{
		public int Id { get; set; }
		[Required]
		public string? Message { get; set; }
		public string? VehicleModel { get; set; }
        public int PassengerCapacity { get; set; }
        public DateTime RequestDate { get; set; }
        public bool Read { get; set; }
        public bool HasPaid { get; set; }
    }
}
