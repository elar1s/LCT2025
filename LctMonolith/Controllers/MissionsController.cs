using System.Security.Claims;
using LctMonolith.Domain.Entities;
using LctMonolith.Services;
using LctMonolith.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LctMonolith.Controllers;

/// <summary>
/// Endpoints for listing and managing missions.
/// </summary>
[ApiController]
[Route("api/missions")]
[Authorize]
public class MissionsController : ControllerBase
{
    private readonly IMissionService _missionService;

    public MissionsController(IMissionService missionService)
    {
        _missionService = missionService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Returns missions currently available to the authenticated user.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mission>>> GetAvailable(CancellationToken ct)
    {
        var userId = GetUserId();
        var list = await _missionService.GetAvailableMissionsAsync(userId, ct);
        return Ok(list);
    }

    /// <summary>Create a mission (HR functionality – for now any authenticated user).</summary>
    [HttpPost]
    public async Task<ActionResult<Mission>> Create(CreateMissionModel model, CancellationToken ct)
    {
        var mission = await _missionService.CreateMissionAsync(model, ct);
        return CreatedAtAction(nameof(GetAvailable), new { id = mission.Id }, mission);
    }

    /// <summary>Update mission status for current user (submit/complete/etc.).</summary>
    [HttpPatch("{missionId:guid}/status")]
    public async Task<ActionResult> UpdateStatus(Guid missionId, UpdateMissionStatusRequest req, CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await _missionService.UpdateStatusAsync(userId, missionId, req.Status, req.SubmissionData, ct);
        return Ok(new { result.MissionId, result.Status, result.UpdatedAt });
    }
}

