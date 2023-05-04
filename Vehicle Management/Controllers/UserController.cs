using Microsoft.AspNetCore.Mvc;
using Vehicle_Management.Models;

namespace Vehicle_Management.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Home(UserRequestViewModel model)
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
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Home(UserRequestViewModel model, Request newRequest, RequestMessage newRequestMessage, RequestStatus newRequestStatus)
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

            return RedirectToAction("Home");

        }
    }
}
