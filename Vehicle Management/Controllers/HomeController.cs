﻿ using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Vehicle_Management.Constants;
using Vehicle_Management.Data;
using Vehicle_Management.Hubs;
using Vehicle_Management.Models;
using Vehicle_Management.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Vehicle_Management.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserManager<UserManager> _userManager;	
        private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _dbContext;
		private readonly NotificationHub _notification;
		private readonly NotificationService _notificationService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<UserManager> userManager, NotificationService notificationService, NotificationHub notification)
		{
			_userManager = userManager;
			_logger = logger;
			_dbContext = dbContext;
			_notification = notification;
			_notificationService = notificationService;
		}

		[HttpGet]
		public async Task<IActionResult> Index(BaseViewModel model, TotalData totalData)
		{

			if (User.IsInRole("User"))
			{
				return RedirectToAction("Home","User");
            }
			if (User.IsInRole("Driver"))
			{
				return RedirectToAction("ViewTask", "Driver");
			}

			model.Notifications = _notificationService.getAdminNotification();
			await totalData.SetData(_dbContext, _userManager);
            model.TotalData = totalData;
            return View(model);
        }

		[HttpPost]
        public IActionResult Index(BaseViewModel model)
		{
			if (model.UserRequest != null)
			{
				TempData["PickupPoint"] = model.UserRequest.PickupPoint;
                TempData["DropPoint"] = model.UserRequest.DropPoint;
                TempData["PickupPointLandmark"] = model.UserRequest.PickupPointLandmark;
                TempData["DropPointLandmark"] = model.UserRequest.DropPointLandmark;
                TempData["RequestedDate"] = model.UserRequest.RequestedDate;
                TempData["RequestMessage"] = model.UserRequest.Message;
            }
            return RedirectToPage("/Account/Login", new { area = "Identity" });
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

		//Register New User
		[HttpGet]
        public IActionResult RegisterUser()
		{
			return View();
		}

        [HttpPost]
        public async Task<IActionResult> RegisterUser(BaseViewModel model, Driver newDriver)
        {
            if (model.RegisterUser != null)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.RegisterUser.Email);
                if (existingUser == null)
				{
                    var user = new UserManager
                    {
                        UserName = model.RegisterUser.Email,
                        Email = model.RegisterUser.Email,
                        Name = model.RegisterUser.Name,
                        IsDisabled = false
                    };

                    var result = await _userManager.CreateAsync(user, model.RegisterUser.Password);
                    if (result.Succeeded)
                    {
                        TempData["message"] = "New User Created!";
                        var userId = await _userManager.GetUserIdAsync(user);
                        if (model.RegisterUser.Role == "User")
                        {
                            await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                            _logger.LogInformation("User created a new account with password.");
                        }
                        if (model.RegisterUser.Role == "Admin")
                        {
                            await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                            _logger.LogInformation("User created a new account with password with admin permissions.");
                        }
                        if (model.RegisterUser.Role == "Driver")
                        {
                            await _userManager.AddToRoleAsync(user, Roles.Driver.ToString());
                            _logger.LogInformation("User created a new account with password with driver permissions.");
                            newDriver.CreatedDate = DateTime.Now;
                            newDriver.UserID = userId;
                            var currentUser = await _userManager.GetUserAsync(User);
                            newDriver.CreatedById = currentUser.Id;
                            _dbContext.Add(newDriver);
                            _dbContext.SaveChanges();
                        }
                        return RedirectToAction("Index");
                    }
                    TempData["message"] = "Failed To Create User";
                }
                TempData["message"] = "User with Email Exists";
            }
            return View();
        }

        //View All requests
        [HttpGet]
		public IActionResult ViewRequests(BaseViewModel model)
		{
			var request = _dbContext.Requests.ToList();
			model.UserRequests = request.Select(r => new UserRequest
			{
				Id = r.Id,
				RequestedDate = r.RequestedDate,
				PickupPoint = r.PickupPoint,
				PickupPointLandmark = r.PickupPointLandmark,
				DropPoint = r.DropPoint,
				DropPointLandmark = r.DropPointLandmark,
				CreatedDate = r.CreatedDate,
				IsApproved = r.IsApproved,
				IsUnapproved = r.IsUnapproved,
				IsCompleted = r.IsCompleted,
				IsCancelled = r.IsCancelled,
				UserId = r.UserId,
			}).ToList();
            model.Notifications = _notificationService.getAdminNotification();
            return View(model);
		}

		//Approve Request

		[HttpPost]
		public async Task<IActionResult> ApproveRequest(int id, BaseViewModel model, RequestHistory newRequestHistory)
		{
			var getRequest = _dbContext.Requests.Include(r => r.RequestStatus).FirstOrDefault(r => r.Id == id);
			var currentUser = await _userManager.GetUserAsync(User);

			getRequest.DriverUserId = model.UserRequest.DriverUserId;
            getRequest.IsApproved = true;
			getRequest.SetRequestStatus(_dbContext, "Approved");

			var getNotification = _dbContext.Notifications.Include(n => n.NotificationType).AsEnumerable().FirstOrDefault(n => n.RequestId == id && n.IsOfType("NeedsApproval"));
			if (getNotification != null)
			{
                getNotification.IsRead = true;
            }
			_dbContext.SaveChanges();

			newRequestHistory = new RequestHistory();
			newRequestHistory.CreatedDate = DateTime.Now;
			newRequestHistory.RequestId = getRequest.Id;
			newRequestHistory.RequestStatusId = getRequest.RequestStatus.Id;
			newRequestHistory.CreatedById = currentUser.Id;
			_dbContext.Add(newRequestHistory);
			_dbContext.SaveChanges();

			var requestByUser = _userManager.Users.FirstOrDefault(u=> u.Id == getRequest.UserId);
            var assignedDriverUser = _userManager.Users.FirstOrDefault(u => u.Id == getRequest.DriverUserId);
            if (currentUser != null && requestByUser != null && assignedDriverUser != null)
            {
                await _notification.SendNotification(getRequest.UserId , currentUser.Id, getRequest.Id, "IsApproved" , "User");
                await _notification.SendNotification(getRequest.DriverUserId, currentUser.Id, getRequest.Id, "IsPending", "Driver");
            }
			return RedirectToAction("ViewRequests");
		}

		[HttpGet]
        public IActionResult ApproveRequest(BaseViewModel model, int id)
        {
			var getRequest = _dbContext.Requests.FirstOrDefault(r => r.Id == id);
			var getRequestMessage = _dbContext.RequestMessages.FirstOrDefault(rm => rm.RequestId == id);
			if (getRequest != null && getRequestMessage!=null)
			{          
                model.UserRequest = new UserRequest();
                model.UserRequest.Id = getRequest.Id;
                model.UserRequest.RequestedDate = getRequest.RequestedDate;
                model.UserRequest.PickupPoint = getRequest.PickupPoint;
                model.UserRequest.PickupPointLandmark = getRequest.PickupPointLandmark;
                model.UserRequest.DropPoint = getRequest.DropPoint;
                model.UserRequest.DropPointLandmark = getRequest.DropPointLandmark;
                model.UserRequest.CreatedDate = getRequest.CreatedDate;
                model.UserRequest.UserId = getRequest.UserId;
                model.UserRequest.Message = getRequestMessage.Message;
				model.UserRequest.IsApproved = getRequest.IsApproved;
            }
            model.Notifications = _notificationService.getAdminNotification();
            return View(model);
        }
    

        //Unapprove Request
        [HttpGet]
        public async Task<IActionResult> UnapproveRequest(int id, RequestHistory newRequestHistory)
        {
            var getRequest = _dbContext.Requests.Include(r => r.RequestStatus).FirstOrDefault(r => r.Id == id);
			var currentUser = await _userManager.GetUserAsync(User);
			if (getRequest is null)
            {
                return NotFound();
            }
			getRequest.IsUnapproved = true;
			getRequest.IsCompleted = true;
			getRequest.SetRequestStatus(_dbContext, "Unapproved");
			
			var getNotification = _dbContext.Notifications.Include(n => n.NotificationType).AsEnumerable().FirstOrDefault(n => n.RequestId == id && n.IsOfType("NeedsApproval"));
			if (getNotification != null)
			{
				getNotification.IsRead = true;
			}

			newRequestHistory = new RequestHistory();
			newRequestHistory.CreatedDate = DateTime.Now;
			newRequestHistory.RequestId = getRequest.Id;
			newRequestHistory.RequestStatusId = getRequest.RequestStatus.Id;
			newRequestHistory.CreatedById = currentUser.Id;
			_dbContext.SaveChanges();
			_dbContext.Add(newRequestHistory);
			
            return RedirectToAction("ViewRequests");
        }

        // View Vehicle Table
        [HttpGet]
		public IActionResult ViewVehicles(BaseViewModel model)
		{
            var vehicles = _dbContext.Vehicles.ToList();
            model.Vehicles = vehicles.Select(v => new VehicleView
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
            model.Notifications = _notificationService.getAdminNotification();
            return View(model);
		}

		//Add New Vehicle

		[HttpPost]
		public async Task<IActionResult> AddVehicle(BaseViewModel model, Vehicle newVehicle)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            newVehicle.RegistrationNumber = model.Vehicle.RegistrationNumber;
			newVehicle.ManufactureCompany = model.Vehicle.ManufactureCompany;
			newVehicle.VehicleModel = model.Vehicle.VehicleModel;
			newVehicle.EngineCapacity = model.Vehicle.EngineCapacity;
			newVehicle.ManufacturedYear = model.Vehicle.ManufacturedYear;
			newVehicle.PurchasedOn = model.Vehicle.PurchasedOn;
			newVehicle.Color = model.Vehicle.Color;
			newVehicle.EngineNumber = model.Vehicle.EngineNumber;
			newVehicle.ChasisNumber = model.Vehicle.ChasisNumber;
			newVehicle.PassengerCapacity = model.Vehicle.PassengerCapacity;
			newVehicle.Fuel = model.Vehicle.Fuel;
			newVehicle.IsAvailable = true;
			newVehicle.CreatedDate = DateTime.Now;
            newVehicle.CreatedById = currentUser.Id;
            _dbContext.Add(newVehicle);
			_dbContext.SaveChanges();
			return RedirectToAction("ViewVehicles");
		}

		[HttpGet]
		public IActionResult AddVehicle(BaseViewModel model)
		{
            model.Notifications = _notificationService.getAdminNotification();
            return View(model);
		}

		//Edit Vehicle Data

		[HttpPost]
		public IActionResult EditVehicle(int id, BaseViewModel model)
		{
			var getVehicle = _dbContext.Vehicles.FirstOrDefault(v => v.Id == id);
			getVehicle.RegistrationNumber = model.Vehicle.RegistrationNumber;
			getVehicle.ManufactureCompany = model.Vehicle.ManufactureCompany;
			getVehicle.VehicleModel = model.Vehicle.VehicleModel;
			getVehicle.EngineCapacity = model.Vehicle.EngineCapacity;
			getVehicle.ManufacturedYear = model.Vehicle.ManufacturedYear;
			getVehicle.PurchasedOn = model.Vehicle.PurchasedOn;
			getVehicle.Color = model.Vehicle.Color;
			getVehicle.EngineNumber = model.Vehicle.EngineNumber;
			getVehicle.ChasisNumber = model.Vehicle.ChasisNumber;
			getVehicle.PassengerCapacity = model.Vehicle.PassengerCapacity;
			getVehicle.Fuel = model.Vehicle.Fuel;
			getVehicle.IsAvailable = model.Vehicle.IsAvailable;
			_dbContext.SaveChanges();
			return RedirectToAction("ViewVehicles");
		}

		[HttpGet]
		public IActionResult EditVehicle(BaseViewModel model, int id)
		{
			var getVehicle = _dbContext.Vehicles.FirstOrDefault(v => v.Id == id);
			if (getVehicle != null)
			{
				model.Vehicle = new VehicleView();
				model.Vehicle.Id = getVehicle.Id;
				model.Vehicle.RegistrationNumber = getVehicle.RegistrationNumber;
				model.Vehicle.ManufactureCompany = getVehicle.ManufactureCompany;
				model.Vehicle.VehicleModel = getVehicle.VehicleModel;
				model.Vehicle.EngineCapacity = getVehicle.EngineCapacity;
				model.Vehicle.ManufacturedYear = getVehicle.ManufacturedYear;
				model.Vehicle.PurchasedOn = getVehicle.PurchasedOn;
				model.Vehicle.Color = getVehicle.Color;
				model.Vehicle.EngineNumber = getVehicle.EngineNumber;
				model.Vehicle.ChasisNumber = getVehicle.ChasisNumber;
				model.Vehicle.PassengerCapacity = getVehicle.PassengerCapacity;
				model.Vehicle.Fuel = getVehicle.Fuel;
				model.Vehicle.IsAvailable = getVehicle.IsAvailable;
			}
            model.Notifications = _notificationService.getAdminNotification();
            return View(model);
		}

		//Disable Vehicle
		[HttpGet]
		public IActionResult DisableVehicle(int id)
		{
			if (User.IsInRole("Admin"))
			{ 
			var getVehicle = _dbContext.Vehicles.FirstOrDefault(v => v.Id == id);
			if (getVehicle is null)
			{
				return NotFound();
			}
			getVehicle.IsAvailable = false;
			_dbContext.SaveChanges();
			return RedirectToAction("ViewVehicles");
			}
            return NotFound();
        }

        //Enable Vehicle
        [HttpGet]
        public IActionResult EnableVehicle(int id)
        {
            if (User.IsInRole("Admin"))
            {
                var getVehicle = _dbContext.Vehicles.FirstOrDefault(v => v.Id == id);
                if (getVehicle is null)
                {
                    return NotFound();
                }
                getVehicle.IsAvailable = true;
                _dbContext.SaveChanges();
                return RedirectToAction("ViewVehicles");
            }
            return NotFound();
        }

        //Delete Existing Vehicle
        [HttpGet]
		public IActionResult DeleteVehicle(int id)
		{
			if (User.IsInRole("Admin"))
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
            return NotFound();
        }

		//View All Notifiactions
		[HttpGet]
		public async  Task<IActionResult> AllNotifications(BaseViewModel model)
		{
			if (User == null)
			{
				return NotFound();
			}
			if (User.IsInRole("Admin"))
			{
				model.AllNotifications = _notificationService.getAllAdminNotifications();
				return View(model);
			}
			else
			{
				var currrentUser = await _userManager.GetUserAsync(User);
                model.AllNotifications = _notificationService.getAllUserNotifications(currrentUser.Id);
				return View(model);
			}
        }

        //View All Users
        [HttpGet]
        public async Task<IActionResult> AllUsers(BaseViewModel model)
        {
            var users = await _dbContext.Users.ToListAsync();
            var userDetails = new List<UserDetail>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();

                var totalRequests = _dbContext.Requests.Count(r => r.CreatedbyId == user.Id);

                var userDetail = new UserDetail
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = roleName,
                    TotalRequests = totalRequests,
                    IsDisabled = user.IsDisabled
                };

                userDetails.Add(userDetail);
            }

            model.UserDetails = userDetails;

            return View(model);
        }

        //Enable User
        [HttpGet]
        public IActionResult EnableUser(String id)
        {
            if (User.IsInRole("Admin"))
            {
                var getUser = _dbContext.Users.FirstOrDefault(u => u.Id == id);
                if (getUser is null)
                {
                    return NotFound();
                }
                getUser.IsDisabled = false;
                _dbContext.SaveChanges();
                return RedirectToAction("AllUsers");
            }
            return NotFound();
        }

        //Disable User
        [HttpGet]
        public IActionResult DisableUser(String id)
        {
            if (User.IsInRole("Admin"))
            {
                var getUser = _dbContext.Users.FirstOrDefault(u => u.Id == id);
                if (getUser is null)
                {
                    return NotFound();
                }
                getUser.IsDisabled = true;
                _dbContext.SaveChanges();
                return RedirectToAction("AllUsers");
            }
            return NotFound();
        }
    }
}