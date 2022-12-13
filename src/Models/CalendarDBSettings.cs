namespace CalendarBackend.Models;

public class CalendarDBSettings
{
    public String ConnectionString { get; set; } = null!;
    public String DatabaseName { get; set; } = null!;
    public String UserCollectionsName { get; set; } = null!;
    public String EventCollectionsName { get; set; } = null!;
}