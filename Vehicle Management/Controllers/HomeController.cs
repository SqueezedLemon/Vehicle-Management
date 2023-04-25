﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Vehicle_Management.Models;

namespace Vehicle_Management.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _dbContext;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
			_logger = logger;
			_dbContext = dbContext;
		}

		public IActionResult Index()
		{
			return View();
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

		// View Vehicle Table
		[HttpGet]
		public IActionResult ViewVehicles()
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

		//Add New Vehicle

		[HttpPost]
		public async Task<IActionResult> AddVehicle(HomeModel model, Vehicle newVehicle)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            newVehicle.RegistrationNumber = model.RegistrationNumber;
			newVehicle.ManufactureCompany = model.ManufactureCompany;
			newVehicle.VehicleModel = model.VehicleModel;
			newVehicle.EngineCapacity = model.EngineCapacity;
			newVehicle.ManufacturedYear = model.ManufacturedYear;
			newVehicle.PurchasedOn = model.PurchasedOn;
			newVehicle.Color = model.Color;
			newVehicle.EngineNumber = model.EngineNumber;
			newVehicle.ChasisNumber = model.ChasisNumber;
			newVehicle.PassengerCapacity = model.PassengerCapacity;
			newVehicle.Fuel = model.Fuel;
			newVehicle.IsAvailable = true;
			newVehicle.CreatedDate = DateTime.Now;
            newVehicle.CreatedById = currentUser.Id;
            _dbContext.Add(newVehicle);
			_dbContext.SaveChanges();
			return RedirectToAction("ViewVehicles");
		}

		[HttpGet]
		public IActionResult AddVehicle()
		{
			return View();
		}

		//Edit Vehicle Data

		[HttpPost]
		public IActionResult EditVehicle(int id, HomeModel model)
		{
			var getVehicle = _dbContext.Vehicles.FirstOrDefault(v => v.Id == id);
			getVehicle.RegistrationNumber = model.RegistrationNumber;
			getVehicle.ManufactureCompany = model.ManufactureCompany;
			getVehicle.VehicleModel = model.VehicleModel;
			getVehicle.EngineCapacity = model.EngineCapacity;
			getVehicle.ManufacturedYear = model.ManufacturedYear;
			getVehicle.PurchasedOn = model.PurchasedOn;
			getVehicle.Color = model.Color;
			getVehicle.EngineNumber = model.EngineNumber;
			getVehicle.ChasisNumber = model.ChasisNumber;
			getVehicle.PassengerCapacity = model.PassengerCapacity;
			getVehicle.Fuel = model.Fuel;
			getVehicle.IsAvailable = model.IsAvailable;
			_dbContext.SaveChanges();
			return RedirectToAction("ViewVehicles");
		}

		[HttpGet]
		public IActionResult EditVehicle(HomeModel model, int id)
		{
			var getVehicle = _dbContext.Vehicles.FirstOrDefault(v => v.Id == id);
			model.Id = getVehicle.Id;
			model.RegistrationNumber = getVehicle.RegistrationNumber;
			model.ManufactureCompany = getVehicle.ManufactureCompany;
			model.VehicleModel = getVehicle.VehicleModel;
			model.EngineCapacity = getVehicle.EngineCapacity;
			model.ManufacturedYear = getVehicle.ManufacturedYear;
			model.PurchasedOn = getVehicle.PurchasedOn;
			model.Color = getVehicle.Color;
			model.EngineNumber = getVehicle.EngineNumber;
			model.ChasisNumber = getVehicle.ChasisNumber;
			model.PassengerCapacity = getVehicle.PassengerCapacity;
			model.Fuel = getVehicle.Fuel;
			model.IsAvailable = getVehicle.IsAvailable;				
			return View(model);
		}

		//Delete Existing Vehicle
		[HttpGet]
		public IActionResult DeleteVehicle(int id)
		{
			var getVehicle = _dbContext.Vehicles.FirstOrDefault(v => v.Id == id);
			if (getVehicle is null)
			{
				return NotFound();
			}
			_dbContext.Remove(getVehicle);
			_dbContext.SaveChanges();
			return RedirectToAction("ViewVehicles");

		}
	}
}