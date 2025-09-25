using System.ComponentModel.DataAnnotations;

namespace StoreService.Models;

/// <summary>
/// Request body to create a new order consisting of store item identifiers.
/// </summary>
public class CreateOrderRequest
{
    /// <summary>Identifier of the user creating the order.</summary>
    [Required]
    public long UserId { get; set; }

    /// <summary>Collection of store item ids to include (unique). Duplicates are ignored.</summary>
    [Required]
    [MinLength(1)]
    public List<long> StoreItemIds { get; set; } = new();
}

