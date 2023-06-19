using Microsoft.AspNetCore.Mvc;
using Vehicle_Management.Models;
using Vehicle_Management.Hubs;
using Microsoft.AspNetCore.SignalR;
using Vehicle_Management.Data;
using Vehicle_Management.Services;
using Microsoft.EntityFrameworkCore;

namespace Vehicle_Management.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<UserManager> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly NotificationHub _notification;
        private readonly NotificationService _notificationService;

        public UserController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<UserManager> userManager, NotificationHub notification, NotificationService notificationService)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
            _notification = notification;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Home(BaseViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
            var request = _dbContext.Requests.ToList();
                if (request != null)
                {
                    model.UserRequests = request.Where(r => r.UserId == currentUser.Id).Select(r => new UserRequest
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
                        UserId = r.UserId,
                    }).ToList();
                    model.Notifications = _notificationService.getUserNotification(currentUser.Id);
                    model.UserRequest = new UserRequest
                    {
                        PickupPoint = TempData["PickupPoint"]?.ToString() ?? "",
                        DropPoint = TempData["DropPoint"]?.ToString() ?? "",
                        PickupPointLandmark = TempData["PickupPointLandmark"]?.ToString() ?? "",
                        DropPointLandmark = TempData["DropPointLandmark"]?.ToString() ?? "",
                        RequestedDate = TempData["RequestedDate"] as DateTime? ?? DateTime.MinValue,
                        Message = TempData["RequestMessage"]?.ToString() ?? ""
                    };
                }
            }
            return View(model);
		}
		[HttpPost]
        public async Task<IActionResult> Home(BaseViewModel model, Request newRequest, RequestMessage newRequestMessage, RequestHistory newRequestHistory)
        {
			var currentUser = await _userManager.GetUserAsync(User);

            newRequest.PickupPoint = model.UserRequest.PickupPoint;
            newRequest.PickupPointLandmark = model.UserRequest.PickupPointLandmark;
            newRequest.DropPoint = model.UserRequest.DropPoint;
            newRequest.DropPointLandmark = model.UserRequest.DropPointLandmark;
            newRequest.RequestedDate = model.UserRequest.RequestedDate;
            newRequest.UserId = currentUser.Id;
            newRequest.CreatedbyId = currentUser.Id;
            newRequest.CreatedDate = DateTime.Now;
			newRequest.SetRequestStatus(_dbContext, "Requested");
			_dbContext.Add(newRequest);
			_dbContext.SaveChanges();
			int newRequestId = newRequest.Id;

            newRequestMessage.Message = model.UserRequest.Message;
            newRequestMessage.CreatedDate = DateTime.Now;
            newRequestMessage.RequestId = newRequestId;
            newRequestMessage.CreatedById = currentUser.Id;
            _dbContext.Add(newRequestMessage);

            newRequestHistory.CreatedDate = DateTime.Now;
            newRequestHistory.RequestId = newRequestId;
            newRequestHistory.RequestStatusId = newRequest.RequestStatusId;
            newRequestHistory.CreatedById = currentUser.Id;
			_dbContext.Add(newRequestHistory);
			_dbContext.SaveChanges();

            TempData["message"] = "New Vehicle Request Created";
			if (currentUser != null)
            {
               await _notification.SendNotificationToAdmins(currentUser.Id, newRequestId, "NeedsApproval");
            }

            return RedirectToAction("Home");

        }

        [HttpGet]
        public async Task<IActionResult> CancelRequest(int id, RequestHistory newRequestHistory)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null) 
            {
                var getRequest = _dbContext.Requests.Include(r => r.RequestStatus).AsEnumerable().FirstOrDefault(r => r.Id == id && r.CreatedbyId == currentUser.Id);
                if (getRequest is null)
                {
                    return NotFound();
                }
                getRequest.IsCancelled = true;
                getRequest.IsCompleted = true;
                getRequest.SetRequestStatus(_dbContext, "Cancelled");

                var notification = _dbContext.Notifications.Include(n => n.NotificationType).AsEnumerable().FirstOrDefault(n => n.RequestId == getRequest.Id && n.IsOfType("NeedsApproval"));
                if (notification != null)
                {
                    _dbContext.Remove(notification);
                    _dbContext.SaveChanges();
                }

                newRequestHistory = new RequestHistory();
                newRequestHistory.CreatedDate = DateTime.Now;
                newRequestHistory.RequestId = getRequest.Id;
                newRequestHistory.RequestStatusId = getRequest.RequestStatus.Id;
                newRequestHistory.CreatedById = currentUser.Id;
                _dbContext.Add(newRequestHistory);
                _dbContext.SaveChanges();

                return RedirectToAction("Home");
            }
            return NotFound();
        }
    }
}
