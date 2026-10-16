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
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId =>
        HttpContext.Session.GetInt32("UserId")
        ?? throw new UnauthorizedAccessException();

    /// <summary>Enrol in an event by selecting a category. Participant role only.</summary>
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Enrol(EnrolDto dto)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == dto.CategoryId && c.EventId == dto.EventId);
        if (category == null)
            return BadRequest("Category does not belong to this event.");

        var alreadyEnrolled = await _db.Enrolments
            .AnyAsync(e => e.ParticipantId == CurrentUserId && e.EventId == dto.EventId);
        if (alreadyEnrolled)
            return Conflict("Already enrolled in this event.");

        var enrolment = new Enrolment
        {
            ParticipantId = CurrentUserId,
            EventId = dto.EventId,
            CategoryId = dto.CategoryId
        };

        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return Ok(new { enrolment.EnrolmentId, enrolment.EnrolledAt });
    }

    /// <summary>View your own enrolments. Participant role only.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMine()
    {
        var enrolments = await _db.Enrolments
            .Where(e => e.ParticipantId == CurrentUserId)
            .Include(e => e.Event)
            .Include(e => e.Category)
            .Select(e => new EnrolmentDto(e.EnrolmentId,
                e.Participant.FullName, e.Event.Name, e.Category.Name, e.EnrolledAt))
            .ToListAsync();
        return Ok(enrolments);
    }

    /// <summary>View all enrolments for a specific event. Organiser role only (own events).</summary>
    [HttpGet("event/{eventId}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        var enrolments = await _db.Enrolments
            .Where(e => e.EventId == eventId)
            .Include(e => e.Participant)
            .Include(e => e.Category)
            .Select(e => new EnrolmentDto(e.EnrolmentId,
                e.Participant.FullName, e.Event.Name, e.Category.Name, e.EnrolledAt))
            .ToListAsync();
        return Ok(enrolments);
    }
}
