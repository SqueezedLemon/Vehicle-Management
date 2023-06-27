namespace Vehicle_Management.Models
{
    public class NotificationView
    {
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public bool EmailSent { get; set; } = false;
		public bool IsRead { get; set; } = false;
		public string? NotificationType { get; set; }
		public string? TargetedRole { get; set; }
		public DateTime CreatedDate { get; set; }

		public int RequestId { get; set; }
		public Request? Request { get; set; }

		public string? UserId { get; set; }
		public string? CreatedById { get; set; }
		public UserManager? User { get; set; }
	}

    public class AllNotificationView
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool EmailSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public string? NotificationType { get; set; }
        public string? TargetedRole { get; set; }
        public DateTime CreatedDate { get; set; }

        public int RequestId { get; set; }
        public Request? Request { get; set; }

        public string? UserId { get; set; }
        public string? CreatedById { get; set; }
        public UserManager? User { get; set; }
    }
}
