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
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public ResultsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId =>
        HttpContext.Session.GetInt32("UserId")
        ?? throw new UnauthorizedAccessException();

    /// <summary>Capture a finish time and position for a participant. Organiser role only.</summary>
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Capture(CaptureResultDto dto)
    {
        var enrolment = await _db.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentId == dto.EnrolmentId);
        if (enrolment == null) return NotFound("Enrolment not found.");
        if (enrolment.Event.OrganiserId != CurrentUserId) return Forbid();

        if (!TimeSpan.TryParse(dto.FinishTime, out var finishTime))
            return BadRequest("Invalid finish time format. Use hh:mm:ss.");

        var result = new Result
        {
            EnrolmentId = dto.EnrolmentId,
            FinishTime = finishTime,
            FinishingPosition = dto.FinishingPosition
        };

        _db.Results.Add(result);
        await _db.SaveChangesAsync();
        return Ok(new { result.ResultId, result.FinishTime, result.FinishingPosition });
    }

    /// <summary>View your own results. Participant role only.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMine()
    {
        var results = await _db.Results
            .Where(r => r.Enrolment.ParticipantId == CurrentUserId)
            .Include(r => r.Enrolment)
                .ThenInclude(e => e.Event)
            .Include(r => r.Enrolment)
                .ThenInclude(e => e.Category)
            .Select(r => new ResultDto(r.ResultId,
                r.Enrolment.Participant.FullName,
                r.Enrolment.Event.Name,
                r.Enrolment.Category.Name,
                r.FinishTime,
                r.FinishingPosition))
            .ToListAsync();
        return Ok(results);
    }
}
