using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Vehicle_Management.Controllers;
using Vehicle_Management.Hubs;
using Vehicle_Management.Services;

namespace Vehicle_Management.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly EmailService _emailService;

        public NotificationHub(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IHubContext<NotificationHub> hubContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
            _hubContext = hubContext;
            _emailService = emailService;
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

            var receiverUser = _userManager.Users.FirstOrDefault(u => u.Id == receiverUserId);
            if (receiverUser != null && receiverUser.Email!= null)
            {
                if (notificationType == "Is Approved")
                {
                    _emailService.SendEmail(receiverUser.Email, "Request Approved", "Your vehicle request has been approved.");
                }
                if (notificationType == "Is Pending")
                {
                    _emailService.SendEmail(receiverUser.Email, "Pending Request", "A new request is pending.");
                }
                notification.EmailSent = true;
                await _dbContext.SaveChangesAsync();
            }

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
            if (notificationType == "Needs Approval")
            {
                var emailBody = "A new request by " + senderUser.Name + " needs approval.";
                await _emailService.SendEmailToAdmins("Request Completed", emailBody);
            }
            if (notificationType == "Request Completed")
            {
                var emailBody = senderUser.Name + " has completed a request.";
                await _emailService.SendEmailToAdmins("Request Completed", emailBody);
            }
            notification.EmailSent = true;
            await _dbContext.SaveChangesAsync();
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
