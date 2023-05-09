using Microsoft.AspNetCore.Mvc;
using Vehicle_Management.Models;
using Vehicle_Management.Hubs;
using Microsoft.AspNetCore.SignalR;
using Vehicle_Management.Data;
using Vehicle_Management.Services;

namespace Vehicle_Management.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly NotificationHub _notification;
        private readonly NotificationService _notificationService;

        public UserController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, NotificationHub notification, NotificationService notificationService)
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
            var request = _dbContext.Requests.ToList();
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
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Home(BaseViewModel model, Request newRequest, RequestMessage newRequestMessage, RequestStatus newRequestStatus)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            newRequestStatus.CreatedById = currentUser.Id;
            newRequestStatus.CreatedDate = DateTime.Now;
            _dbContext.Add(newRequestStatus);
            _dbContext.SaveChanges();
            int newRequestStatusId = newRequestStatus.Id;

            newRequest.PickupPoint = model.UserRequest.PickupPoint;
            newRequest.PickupPointLandmark = model.UserRequest.PickupPointLandmark;
            newRequest.DropPoint = model.UserRequest.DropPoint;
            newRequest.DropPointLandmark = model.UserRequest.DropPointLandmark;
            newRequest.RequestedDate = model.UserRequest.RequestedDate;
            newRequest.UserId = currentUser.Id;
            newRequest.CreatedbyId = currentUser.Id;
            newRequest.CreatedDate = DateTime.Now;
            newRequest.RequestStatusId = newRequestStatusId;
            _dbContext.Add(newRequest);
            _dbContext.SaveChanges();
            int newRequestId = newRequest.Id;

            newRequestMessage.Message = model.UserRequest.Message;
            newRequestMessage.CreatedDate = DateTime.Now;
            newRequestMessage.RequestId = newRequestId;
            newRequestMessage.CreatedById = currentUser.Id;
            _dbContext.Add(newRequestMessage);
            _dbContext.SaveChanges();

            if (currentUser != null)
            {
               await _notification.SendNotificationToAdmins(currentUser.Id, newRequestId, "Needs Approval");
            }

            return RedirectToAction("Home");

        }
    }
}
