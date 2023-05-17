using Vehicle_Management.Constants;

namespace Vehicle_Management.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Roles
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Driver.ToString()));

			//Seed Request Types and Notification Types
			var context = service.GetService<ApplicationDbContext>();

			// For Request Types
			if (!context.RequestStatuses.Any())
			{
				var requestStatuses = Enum.GetValues(typeof(Requests)).Cast<Requests>();
				foreach (var requestStatus in requestStatuses)
				{
					context.RequestStatuses.Add(new RequestStatus
					{
						RequestStatusName = requestStatus.ToString()
					});
				}
			}

			// For Notification Types
			if (!context.NotificationTypes.Any())
			{
				var notificationTypes = Enum.GetValues(typeof(Notifications)).Cast<Notifications>();
				
				foreach (var notificationType in notificationTypes)
				{
					context.NotificationTypes.Add(new NotificationType
					{
						Type = notificationType.ToString()
					});
				}
				await context.SaveChangesAsync();
			}
		}
    }
}
