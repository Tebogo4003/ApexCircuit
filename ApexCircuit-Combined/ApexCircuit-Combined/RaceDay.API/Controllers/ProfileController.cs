using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public ProfileController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId =>
        HttpContext.Session.GetInt32("UserId")
        ?? throw new UnauthorizedAccessException();

    /// <summary>View your own profile information.</summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _db.Users.FindAsync(CurrentUserId);
        if (user == null) return NotFound();
        return Ok(new UserProfileDto(user.UserId, user.FullName, user.Email, user.Role));
    }

    /// <summary>Update your own profile information.</summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var user = await _db.Users.FindAsync(CurrentUserId);
        if (user == null) return NotFound();

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        await _db.SaveChangesAsync();

        return Ok(new UserProfileDto(user.UserId, user.FullName, user.Email, user.Role));
    }
}
