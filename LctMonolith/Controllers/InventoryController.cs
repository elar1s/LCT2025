using System.Security.Claims;
using LctMonolith.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LctMonolith.Controllers;

/// <summary>
/// Inventory endpoints for viewing owned store items and artifacts.
/// </summary>
[ApiController]
[Route("api/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventory;
    public InventoryController(IInventoryService inventory)
    {
        _inventory = inventory;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>List owned store inventory entries.</summary>
    [HttpGet("store-items")]
    public async Task<IActionResult> GetStoreItems(CancellationToken ct)
    {
        var items = await _inventory.GetStoreInventoryAsync(GetUserId(), ct);
        return Ok(items.Select(i => new { i.StoreItemId, i.Quantity, i.AcquiredAt, i.IsReturned, i.StoreItem?.Name }));
    }

    /// <summary>List owned artifacts.</summary>
    [HttpGet("artifacts")]
    public async Task<IActionResult> GetArtifacts(CancellationToken ct)
    {
        var artifacts = await _inventory.GetArtifactsAsync(GetUserId(), ct);
        return Ok(artifacts.Select(a => new { a.ArtifactId, a.ObtainedAt, a.Artifact?.Name, a.Artifact?.Rarity }));
    }
}

