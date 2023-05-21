using Microsoft.EntityFrameworkCore;
using Vehicle_Management.Models;

namespace Vehicle_Management.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserManager> _userManager;

        public NotificationService (ApplicationDbContext dbContext, UserManager<UserManager> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public List<NotificationView> getUserNotification(string userId)
        {
            var Notifications = _dbContext.Notifications.Include(n => n.NotificationType).Where(n => n.UserId == userId && n.IsRead == false).OrderByDescending(n => n.Date).Select(n => new NotificationView
            {
                Id = n.Id,
                NotificationType = n.NotificationType.Type,
                CreatedById = n.CreatedById,
                RequestId = n.RequestId,
                Date = n.Date
            }).ToList();
            return Notifications;
        }

        public List<NotificationView> getAdminNotification()
        {
			var Notifications = _dbContext.Notifications
				.Include(n => n.NotificationType)
				.Include(n => n.Role)
				.AsEnumerable()
				.Where(n => n.IsRead == false && n.HasTargatedRole("Admin"))
	            .OrderByDescending(n => n.Date)
	            .Select(n => new NotificationView
	            {
		            Id = n.Id,
                    NotificationType = n.NotificationType.Type,
                    CreatedById = n.CreatedById,
                    RequestId = n.RequestId,
                    Date = n.Date
                }).ToList();
            return Notifications;
        }

        public List<NotificationView> getDriverNotification(string userId)
        {
            var Notifications = _dbContext.Notifications.Where(n => n.UserId == userId && n.IsRead == false).OrderByDescending(n => n.Date).Select(n => new NotificationView
            {
                Id = n.Id,
                NotificationType = n.NotificationType.Type,
                CreatedById = n.CreatedById,
                RequestId = n.RequestId,
                Date = n.Date
            }).ToList();
            return Notifications;
        }
    }
}
