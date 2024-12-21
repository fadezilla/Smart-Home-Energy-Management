namespace SmartHome.backend.Dto;
public class CreateScheduleDto
{
    public string TargetType { get; set; } // "device" or "group"
    public int TargetId { get; set; }
    public string Action { get; set; } // "on" or "off"
    public DateTime ScheduledTime { get; set; }
}