using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Vehicle_Management.Models;

namespace Vehicle_Management.Controllers
{
	public class DriverController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _dbContext;

		public DriverController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
			_logger = logger;
			_dbContext = dbContext;
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
			return View(model);
		}

        //Mark Task Complete
        [HttpGet]
        public IActionResult CompletedTask(int id)
        {
            var getRequest = _dbContext.Requests.FirstOrDefault(r => r.Id == id);
            var getRequestStatus = _dbContext.RequestStatuses.FirstOrDefault(rs => rs.Id == getRequest.RequestStatusId);
            if (getRequest is null)
            {
                return NotFound();
            }
            getRequest.IsCompleted = true;
            getRequestStatus.RequestStatusName = "Approved and Completed";
            _dbContext.SaveChanges();
            return RedirectToAction("ViewTask");
        }
    }
}
