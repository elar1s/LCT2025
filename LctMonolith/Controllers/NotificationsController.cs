using System.Security.Claims;
using LctMonolith.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LctMonolith.Controllers;

/// <summary>
/// In-app user notifications API.
/// </summary>
[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notifications;

    public NotificationsController(INotificationService notifications)
    {
        _notifications = notifications;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get up to 100 unread notifications.</summary>
    [HttpGet("unread")] 
    public async Task<IActionResult> GetUnread(CancellationToken ct)
    {
        var list = await _notifications.GetUnreadAsync(GetUserId(), ct);
        return Ok(list);
    }

    /// <summary>Get recent notifications (paged by take).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int take = 100, CancellationToken ct = default)
    {
        var list = await _notifications.GetAllAsync(GetUserId(), take, ct);
        return Ok(list);
    }

    /// <summary>Mark a notification as read.</summary>
    [HttpPost("mark/{id:guid}")] 
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        await _notifications.MarkReadAsync(GetUserId(), id, ct);
        return NoContent();
    }

    /// <summary>Mark all notifications as read.</summary>
    [HttpPost("mark-all")] 
    public async Task<IActionResult> MarkAll(CancellationToken ct)
    {
        var cnt = await _notifications.MarkAllReadAsync(GetUserId(), ct);
        return Ok(new { updated = cnt });
    }
}

