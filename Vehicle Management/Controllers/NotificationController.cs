using Microsoft.AspNetCore.Mvc;
using Vehicle_Management.Models;

namespace Vehicle_Management.Controllers
{
    public class NavbarController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public NavbarController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<PartialViewResult> NavbarView(NavbarViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                if (User.IsInRole("Admin"))
                {
                    var notifications = _dbContext.Notifications.ToList();
                    model.Notifications = notifications.Where(n => n.TargetedRole == "Admin").Select(n => new NotificationView
                    {
                        Id = n.Id,
                        NotificationType = n.NotificationType,
                        CreatedById = n.CreatedById,
                        Date = n.Date
                    }).ToList();
                }
                else
                {
                    var notifications = _dbContext.Notifications.ToList();
                    model.Notifications = notifications.Where(n => n.UserId == currentUser.Id).Select(n => new NotificationView
                    {
                        Id = n.Id,
                        NotificationType = n.NotificationType,
                        Date = n.Date
                    }).ToList();
                }
                return PartialView("_NavbarPartial", model);
            }
            return PartialView("_NavbarPartial");
        }
    }
}
