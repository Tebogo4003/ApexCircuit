namespace RaceDay.API.DTOs;

public record CategoryDto(int CategoryId, string Name, string CategoryType);
public record CreateCategoryDto(string Name, string CategoryType);
