using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public EventsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId =>
        HttpContext.Session.GetInt32("UserId")
        ?? throw new UnauthorizedAccessException();

    /// <summary>Get all events. Accessible by both roles and unauthenticated users.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.Events
            .Select(e => new EventDto(e.EventId, e.Name, e.Description,
                e.EventDate, e.Location, e.Distance, e.EventType))
            .ToListAsync();
        return Ok(events);
    }

    /// <summary>Get a single event by ID.</summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _db.Events.FindAsync(id);
        if (e == null) return NotFound();
        return Ok(new EventDto(e.EventId, e.Name, e.Description,
            e.EventDate, e.Location, e.Distance, e.EventType));
    }

    /// <summary>Create a new event. Organiser role only.</summary>
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(CreateEventDto dto)
    {
        var ev = new Event
        {
            Name = dto.Name,
            Description = dto.Description,
            EventDate = dto.EventDate,
            Location = dto.Location,
            Distance = dto.Distance,
            EventType = dto.EventType,
            OrganiserId = CurrentUserId
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = ev.EventId },
            new EventDto(ev.EventId, ev.Name, ev.Description,
                ev.EventDate, ev.Location, ev.Distance, ev.EventType));
    }

    /// <summary>Update an existing event. Organiser role only (own events).</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, UpdateEventDto dto)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        ev.Name = dto.Name;
        ev.Description = dto.Description;
        ev.EventDate = dto.EventDate;
        ev.Location = dto.Location;
        ev.Distance = dto.Distance;
        ev.EventType = dto.EventType;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Delete an event. Organiser role only (own events).</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
