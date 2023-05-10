using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Vehicle_Management.Controllers;
using Vehicle_Management.Hubs;

namespace Vehicle_Management.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHub(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IHubContext<NotificationHub> hubContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("OnConnected");
        }

        public async Task SendNotification(string receiverUserId, string senderUserId, int requestId, string notificationType, string targatedRole)
        {
            // Save the notification to the database
            var notification = new Notification { 
                UserId = receiverUserId, 
                CreatedById = senderUserId,
                RequestId = requestId,
                Date = DateTime.Now,
                CreatedDate = DateTime.Now,
                NotificationType = notificationType,
                TargetedRole = targatedRole};
            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            var senderUser = _dbContext.Users.FirstOrDefault(u => u.Id == senderUserId);
            // Send the notification to the specified user
            await _hubContext.Clients.User(receiverUserId).SendAsync("ReceiveNotification", senderUser.Name, notification.RequestId, notification.NotificationType);
        }

        public async Task SendNotificationToAdmins(string senderUserId, int requestId, string notificationType)
        {
            // Save the notification to the database
            var notification = new Notification
            {
                CreatedById = senderUserId,
                RequestId = requestId,
                Date = DateTime.Now,
                CreatedDate = DateTime.Now,
                NotificationType = notificationType,
                TargetedRole = "Admin"
            };
            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            var senderUser = _dbContext.Users.FirstOrDefault(u => u.Id == senderUserId);
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (admins != null)
            {
                var adminIds = admins.Select(u => u.Id);
                foreach (var adminId in adminIds)
                {
                    await _hubContext.Clients.User(adminId).SendAsync("ReceiveAdminNotification", senderUser.Name, notification.RequestId, notification.NotificationType);
                }
            }
        }

        public async Task ReadCompletedNotifications()
        {
            var notifications = _dbContext.Notifications.Where(n => n.NotificationType == "Request Completed" && n.IsRead == false).ToList();
            if (notifications != null)
            { 
                foreach (var notification in notifications) 
                {
                    notification.IsRead = true;
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task ReadApprovedNotifications(string userId)
        {
            var notifications = _dbContext.Notifications.Where(n => n.UserId == userId && n.NotificationType == "Is Approved" && n.IsRead == false).ToList();
            if (notifications != null)
            {
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task ReadPendingNotifications(string userId)
        {
            var notifications = _dbContext.Notifications.Where(n => n.UserId == userId && n.NotificationType == "Is Pending" && n.IsRead == false).ToList();
            if (notifications != null)
            {
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
