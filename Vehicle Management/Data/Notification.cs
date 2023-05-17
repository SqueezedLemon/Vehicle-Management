using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
    public class Notification
    {
		public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool EmailSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; }

        public int RequestId { get; set; }
        public Request? Request { get; set; }

        public int NotificationTypeId { get; set; }
        public NotificationType? NotificationType { get; set; }

        public string? RoleId {get; set; }
        public IdentityRole? Role { get; set; }

        public string? UserId { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? User { get; set; }


		public bool IsOfType(string typeName)
		{
			return NotificationType?.Type.Equals(typeName, StringComparison.OrdinalIgnoreCase) ?? false;
		}
		public void SetNotificationType(ApplicationDbContext dbContext, string notificationTypeName)
		{
			var notificationType = dbContext.NotificationTypes.AsEnumerable().FirstOrDefault(nt => nt.Type.Equals(notificationTypeName, StringComparison.OrdinalIgnoreCase));

			if (notificationType != null)
			{
				NotificationType = notificationType;
				NotificationTypeId = notificationType.Id;
			}
		}

		public bool HasTargatedRole(string roleName)
		{
			return Role?.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase) ?? false;
		}
		public async Task SetTargatedRole(RoleManager<IdentityRole> roleManager, string roleName)
		{
			var role = await roleManager.FindByNameAsync(roleName);

			if (role != null)
			{
				Role = role;
				RoleId = role.Id;
			}
		}

	}
}
