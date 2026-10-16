namespace RaceDay.API.DTOs;

public record RegisterDto(string FullName, string Email, string Password, string Role);
public record LoginDto(string Email, string Password);
public record UserProfileDto(int UserId, string FullName, string Email, string Role);
public record UpdateProfileDto(string FullName, string Email);
