using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/events/{eventId}/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public CategoriesController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId =>
        HttpContext.Session.GetInt32("UserId")
        ?? throw new UnauthorizedAccessException();

    /// <summary>Get all categories for an event. Accessible by both roles.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(int eventId)
    {
        var categories = await _db.Categories
            .Where(c => c.EventId == eventId)
            .Select(c => new CategoryDto(c.CategoryId, c.Name, c.CategoryType))
            .ToListAsync();
        return Ok(categories);
    }

    /// <summary>Add a category to an event. Organiser role only.</summary>
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int eventId, CreateCategoryDto dto)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound("Event not found.");
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        var category = new Category
        {
            Name = dto.Name,
            CategoryType = dto.CategoryType,
            EventId = eventId
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return Ok(new CategoryDto(category.CategoryId, category.Name, category.CategoryType));
    }
}
