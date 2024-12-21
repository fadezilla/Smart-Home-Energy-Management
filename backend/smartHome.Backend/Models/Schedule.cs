using System;
using System.ComponentModel.DataAnnotations;

namespace SmartHome.backend.Models
{
    public class Schedule
    {
        public int Id { get; set; }

        [Required]
        public string TargetType { get; set; } = string.Empty; // e.g. "device" or "group"

        [Required]
        public int TargetId { get; set; } // The DeviceId or DeviceGroupId

        [Required]
        public string Action { get; set; } = "on"; // e.g. "on" or "off"

        [Required]
        public DateTime ScheduledTime { get; set; }
    }
}
