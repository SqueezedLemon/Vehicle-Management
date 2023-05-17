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
        private readonly RoleManager<IdentityRole> _roleManager;

		public NotificationHub(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IHubContext<NotificationHub> hubContext, EmailService emailService, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
            _hubContext = hubContext;
            _emailService = emailService;
            _roleManager = roleManager;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("OnConnected");
        }

        public async Task SendNotification(string receiverUserId, string senderUserId, int requestId, string notificationType, string targatedRole)
        {
            // Save the notification to the database
            var notification = new Notification() { 
                UserId = receiverUserId, 
                CreatedById = senderUserId,
                RequestId = requestId,
                Date = DateTime.Now,
                CreatedDate = DateTime.Now,};
			notification.SetNotificationType(_dbContext, notificationType);
			await notification.SetTargatedRole(_roleManager , targatedRole);
			
			var receiverUser = _userManager.Users.FirstOrDefault(u => u.Id == receiverUserId);
            if (receiverUser != null && receiverUser.Email!= null)
            {
                if (notificationType == "IsApproved")
                {
                    _emailService.SendEmail(receiverUser.Email, "Request Approved", "Your vehicle request has been approved.");
                }
                if (notificationType == "IsPending")
                {
                    _emailService.SendEmail(receiverUser.Email, "Pending Request", "A new request is pending.");
                }
                await _dbContext.SaveChangesAsync();
            }
			notification.EmailSent = true;
			_dbContext.Notifications.Add(notification);
			await _dbContext.SaveChangesAsync();

			var senderUser = _dbContext.Users.FirstOrDefault(u => u.Id == senderUserId);
            // Send the notification to the specified user
            await _hubContext.Clients.User(receiverUserId).SendAsync("ReceiveNotification", senderUser.Name, notification.RequestId, notificationType);
        }

        public async Task SendNotificationToAdmins(string senderUserId, int requestId, string notificationType)
        {
            // Save the notification to the database
            var notification = new Notification()
			{
                CreatedById = senderUserId,
                RequestId = requestId,
                Date = DateTime.Now,
                CreatedDate = DateTime.Now,
            };
			notification.SetNotificationType(_dbContext, notificationType);
			await notification.SetTargatedRole(_roleManager, "Admin");
			

			var senderUser = _dbContext.Users.FirstOrDefault(u => u.Id == senderUserId);
            if (notificationType == "NeedsApproval")
            {
                var emailBody = "A new request by " + senderUser.Name + " needs approval.";
                await _emailService.SendEmailToAdmins("Request Completed", emailBody);
            }
            if (notificationType == "RequestCompleted")
            {
                var emailBody = senderUser.Name + " has completed a request.";
                await _emailService.SendEmailToAdmins("Request Completed", emailBody);
            }
            notification.EmailSent = true;
			_dbContext.Notifications.Add(notification);
			await _dbContext.SaveChangesAsync();

			var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (admins != null)
            {
                var adminIds = admins.Select(u => u.Id);
                foreach (var adminId in adminIds)
                {
                    await _hubContext.Clients.User(adminId).SendAsync("ReceiveAdminNotification", senderUser.Name, notification.RequestId, notificationType);
                }
            }
        }

        public async Task ReadCompletedNotifications()
        {
            var notifications = _dbContext.Notifications.Include(n => n.NotificationType).AsEnumerable().Where(n => n.IsRead == false && n.IsOfType("RequestCompleted")).ToList();
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
            var notifications = _dbContext.Notifications.Include(n => n.NotificationType).AsEnumerable().Where(n => n.UserId == userId && n.IsOfType("IsApproved") && n.IsRead == false).ToList();
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
            var notifications = _dbContext.Notifications.Include(n => n.NotificationType).AsEnumerable().Where(n => n.UserId == userId && n.IsOfType("IsPending") && n.IsRead == false).ToList();
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
