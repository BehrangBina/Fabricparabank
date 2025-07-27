using System.Text.Json.Serialization;

namespace FabricParaBank.Tests.Model;

public record TransactionResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("accountId")]
    public int AccountId { get; set; }

    [JsonPropertyName("type")]
    public required string  Type { get; set; } // Consider using an enum if types are fixed

    [JsonPropertyName("date")]
    public long DateUnixMilliseconds { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    // Optional: Convert Unix timestamp to DateTime
    [JsonIgnore]
    public DateTime Date =>
        DateTimeOffset.FromUnixTimeMilliseconds(DateUnixMilliseconds).UtcDateTime;

}