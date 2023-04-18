using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Vehicle_Management.Models;

namespace Vehicle_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var vehicles = _dbContext.Vehicles.ToList();
			List<HomeModel> homeModels = vehicles.Select(v => new HomeModel
			{
                Id = v.Id,
				RegistrationNumber = v.RegistrationNumber,
				ManufactureCompany = v.ManufactureCompany,
				VehicleModel = v.VehicleModel,
				EngineCapacity = v.EngineCapacity,
				ManufacturedYear = v.ManufacturedYear,
				PurchasedOn = v.PurchasedOn,
				Color = v.Color,
				EngineNumber = v.EngineNumber,
				ChasisNumber = v.ChasisNumber,
				PassengerCapacity = v.PassengerCapacity,
				Fuel = v.Fuel,
				IsAvailable = v.IsAvailable
			}).ToList();
			return View(homeModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AddVehicle()
        {
            return View();
        }
		public IActionResult EditVehicle()
		{
			return View();
		}
	}
}