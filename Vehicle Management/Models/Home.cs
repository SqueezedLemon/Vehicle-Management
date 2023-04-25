using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Vehicle_Management.Models
{
	public class HomeModel
	{
		public int Id { get; set; }
		[Required]
		public string? RegistrationNumber { get; set; }
		public string? ManufactureCompany { get; set; }
		public string? VehicleModel { get; set; }
		public int EngineCapacity { get; set; }
		public int ManufacturedYear { get; set; }
		public DateTime PurchasedOn { get; set; }
		public string? Color { get; set; }
		public string? EngineNumber { get; set; }
		public string? ChasisNumber { get; set; }
		public int PassengerCapacity { get; set; }
		public string? Fuel { get; set; }
		public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
        public String? CreatedById { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
