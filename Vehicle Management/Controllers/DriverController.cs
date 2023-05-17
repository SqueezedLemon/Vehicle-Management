using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Vehicle_Management.Hubs;
using Vehicle_Management.Models;
using Vehicle_Management.Services;

namespace Vehicle_Management.Controllers
{
	public class DriverController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _dbContext;
        private readonly NotificationHub _notification;
        private readonly NotificationService _notificationService;

        public DriverController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, NotificationHub notification, NotificationService notificationService)
		{
			_userManager = userManager;
			_logger = logger;
			_dbContext = dbContext;
			_notification = notification;
			_notificationService = notificationService;
		}

		// View Tasks
		public async Task<ActionResult> ViewTask(BaseViewModel model)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			var request = _dbContext.Requests.ToList();
			model.UserRequests = request.Where(r => r.DriverUserId == currentUser.Id).Select(r => new UserRequest
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
            model.Notifications = _notificationService.getDriverNotification(currentUser.Id);
            return View(model);
		}

        //Mark Task Complete
        [HttpGet]
        public async Task<IActionResult> CompletedTask(int id, RequestHistory newRequestHistory)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var getRequest = _dbContext.Requests.Include(r => r.RequestStatus).FirstOrDefault(r => r.Id == id);
            if (getRequest is null)
            {
                return NotFound();
            }
            getRequest.IsCompleted = true;
			getRequest.SetRequestStatus(_dbContext,"Completed");
            _dbContext.SaveChanges();

			newRequestHistory = new RequestHistory();
			newRequestHistory.CreatedDate = DateTime.Now;
			newRequestHistory.RequestId = getRequest.Id;
			newRequestHistory.RequestStatusId = getRequest.RequestStatus.Id;
			newRequestHistory.CreatedById = currentUser.Id;
			_dbContext.Add(newRequestHistory);
			_dbContext.SaveChanges();

			await _notification.SendNotificationToAdmins(currentUser.Id, getRequest.Id, "RequestCompleted");
            return RedirectToAction("ViewTask");
        }
    }
}
