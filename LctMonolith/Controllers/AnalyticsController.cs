using LctMonolith.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LctMonolith.Controllers;

/// <summary>
/// Basic analytics endpoints.
/// </summary>
[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;
    public AnalyticsController(IAnalyticsService analytics)
    {
        _analytics = analytics;
    }

    /// <summary>Get aggregate system summary metrics.</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var summary = await _analytics.GetSummaryAsync(ct);
        return Ok(summary);
    }
}

