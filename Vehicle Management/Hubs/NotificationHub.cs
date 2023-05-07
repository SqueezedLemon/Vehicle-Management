using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Vehicle_Management.Hubs;

namespace Vehicle_Management.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationHub(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Admin"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
            }

            await base.OnConnectedAsync();
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

            // Send the notification to the specified user
            await Clients.User(receiverUserId).SendAsync("ReceiveNotification", notificationType);
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

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var adminIds = admins.Select(u => u.Id);
            await Clients.Groups(adminIds).SendAsync("ReceiveNotification", notificationType);
        }
    }
}
