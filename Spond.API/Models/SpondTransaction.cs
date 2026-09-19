namespace Spond.API.Models;

/// <summary>
/// Represents a Spond Club financial transaction.
/// </summary>
public class SpondTransaction
{
    /// <summary>
    /// The unique identifier of the transaction.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the payment was made.
    /// </summary>
    public string? PaidAt { get; set; }

    /// <summary>
    /// The name of the payment or charge.
    /// </summary>
    public string? PaymentName { get; set; }

    /// <summary>
    /// The name of the person who made the payment.
    /// </summary>
    public string? PaidByName { get; set; }

    /// <summary>
    /// The amount of the transaction.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// The currency code for the transaction (e.g. "NOK").
    /// </summary>
    public string? Currency { get; set; }
}
