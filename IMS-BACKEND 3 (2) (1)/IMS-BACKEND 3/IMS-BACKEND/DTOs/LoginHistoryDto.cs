public class LoginHistoryDto
{
    public DateTime LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public string? Browser { get; set; }

    public string? OperatingSystem { get; set; }

    public string? DeviceInfo { get; set; }

    public string? IpAddress { get; set; }

    public string? LogoutType { get; set; }

    public bool IsCurrentSession { get; set; }
}