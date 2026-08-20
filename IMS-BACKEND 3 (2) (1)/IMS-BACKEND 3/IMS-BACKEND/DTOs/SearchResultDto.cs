namespace IMSBackend.DTOs;

public class SearchResultDto
{
    public string Type { get; set; } = string.Empty;

    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    public string Route { get; set; } = string.Empty;

    public string? Icon { get; set; }
}
