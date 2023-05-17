using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Management.Data
{
	[Table("NotificationType")]
	public class NotificationType
	{
        public int Id { get; set; }
        public string? Type { get; set; }
    }
}
