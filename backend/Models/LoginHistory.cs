using IMSBackend.Models;
using System.ComponentModel.DataAnnotations;

public class LoginHistory
{
    [Key]
    public int LoginHistoryId { get; set; }

    public int UserId { get; set; }

    public DateTime LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public string? DeviceInfo { get; set; }

    public string? Browser { get; set; }

    public string? OperatingSystem { get; set; }

    public string? IpAddress { get; set; }

    public string? LogoutType { get; set; }
    // Manual
    // Logout All Devices
    // Password Changed
    // Token Expired

    public bool IsCurrentSession { get; set; }

    public User? User { get; set; }
}