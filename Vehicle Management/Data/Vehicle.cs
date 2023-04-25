using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    [Table("Vehicle")]
    public class Vehicle
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

        public string? CreatedById { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
