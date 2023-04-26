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
        public IActionResult Home()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Home(UserRequest model, Request newRequest, RequestMessage newRequestMessage, RequestStatus newRequestStatus)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            newRequestStatus.CreatedById = currentUser.Id;
            newRequestStatus.CreatedDate = DateTime.Now;
            _dbContext.Add(newRequestStatus);
            _dbContext.SaveChanges();
            int newRequestStatusId = newRequestStatus.Id;

            newRequest.PickupPoint = model.PickupPoint;
            newRequest.PickupPointLandmark = model.PickupPointLandmark;
            newRequest.DropPoint = model.DropPoint;
            newRequest.DropPointLandmark = model.DropPointLandmark;
            newRequest.RequestedDate = model.RequestedDate;
            newRequest.UserId = currentUser.Id;
            newRequest.CreatedbyId = currentUser.Id;
            newRequest.CreatedDate = DateTime.Now;
            newRequest.RequestStatusId = newRequestStatusId;
            _dbContext.Add(newRequest);
            _dbContext.SaveChanges();
            int newRequestId = newRequest.Id;

            newRequestMessage.Message = model.Message;
            newRequestMessage.CreatedDate = DateTime.Now;
            newRequestMessage.RequestId = newRequestId;
            newRequestMessage.CreatedById = currentUser.Id;
            _dbContext.Add(newRequestMessage);
            _dbContext.SaveChanges();

            return RedirectToAction("Home");

        }
    }
}
