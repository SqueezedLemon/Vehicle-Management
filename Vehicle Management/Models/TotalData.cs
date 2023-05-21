using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Vehicle_Management.Models
{
    public class TotalData
    {
        public int TotalUsers { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int[]? TotalMonthlyRequests { get; set; }
        public int[]? UserInRoleCounts { get; set; }

        public async Task SetData(ApplicationDbContext dbContext, UserManager<UserManager> userManager)
        {
            TotalUsers = dbContext.Users.Count();
            TotalVehicles = dbContext.Vehicles.Count();
            TotalRequests = dbContext.Requests.Count();
            PendingRequests = dbContext.Requests.Where(r => r.IsApproved == false && r.IsCompleted == false).Count();

            //Setting TotalMonthlyRequests for chart
            int currentYear = DateTime.Now.Year;

            List<Request> requests = dbContext.Requests
                .Where(r => r.RequestedDate.Year == currentYear)
                .ToList();

            TotalMonthlyRequests = new int[12];

            var groupedRequests = requests
                .GroupBy(r => r.RequestedDate.Month)
                .Select(g => new { Month = g.Key, TotalRequests = g.Count() });

            foreach (var group in groupedRequests)
            {
                int monthIndex = group.Month - 1;
                TotalMonthlyRequests[monthIndex] = group.TotalRequests;
            }

            // Count of total users in each role for pie chart
            UserInRoleCounts = new int[3];

            var users = await dbContext.Users.ToListAsync();
            UserInRoleCounts[0] = users.Count(u => userManager.IsInRoleAsync(u, "Admin").Result);
            UserInRoleCounts[1] = users.Count(u => userManager.IsInRoleAsync(u, "User").Result);
            UserInRoleCounts[2] = users.Count(u => userManager.IsInRoleAsync(u, "Driver").Result);
        }
    }
}
