using System.Security.Claims;
using LctMonolith.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LctMonolith.Controllers;

/// <summary>
/// Endpoints exposing gamification progress information.
/// </summary>
[ApiController]
[Route("api/gamification")]
[Authorize]
public class GamificationController : ControllerBase
{
    private readonly IGamificationService _gamificationService;

    public GamificationController(IGamificationService gamificationService)
    {
        _gamificationService = gamificationService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Returns current user progress snapshot (rank, xp, outstanding requirements).</summary>
    [HttpGet("progress")] 
    public async Task<IActionResult> GetProgress(CancellationToken ct)
    {
        var snapshot = await _gamificationService.GetProgressAsync(GetUserId(), ct);
        return Ok(snapshot);
    }
}

