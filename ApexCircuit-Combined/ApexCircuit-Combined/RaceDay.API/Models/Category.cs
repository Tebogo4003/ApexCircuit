namespace RaceDay.API.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. "Under 20", "Senior", "10km"
    public string CategoryType { get; set; } = string.Empty; // "Age" | "Distance"
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
}
