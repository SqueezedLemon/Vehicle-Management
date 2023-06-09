﻿using Vehicle_Management.Areas.Identity.Pages.Account;

namespace Vehicle_Management.Models
{
    public class BaseViewModel
    {
        public VehicleView? Vehicle { get; set; }
        public List<VehicleView>? Vehicles { get; set; }
        public UserRequest? UserRequest { get; set; }
        public List<UserRequest>? UserRequests { get; set; }
        public List<NotificationView>? Notifications { get; set; }
        public List<AllNotificationView>? AllNotifications { get; set; }
        public TotalData? TotalData { get; set; }
        public List<UserDetail>? UserDetails { get; set; }
        public RegisterUser? RegisterUser { get; set; }
    }
}
