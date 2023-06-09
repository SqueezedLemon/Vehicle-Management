﻿namespace Vehicle_Management.Models
{
    public class UserRequest
    {
        public int Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public string? PickupPoint { get; set; }
        public string? PickupPointLandmark { get; set; }
        public string? DropPoint { get; set; }
        public string? DropPointLandmark { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsUnapproved { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public bool IsNotified { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public string? Message { get; set; }
        public DateTime CreatedDate { get; set; }

        public int RequestStatusId { get; set; }
        public RequestStatus? RequestStatus { get; set; }

        public string? UserId { get; set; }
        public string? DriverUserId { get; set; }
        public string? CreatedbyId { get; set; }
        public UserManager? User { get; set; }
    }
}
