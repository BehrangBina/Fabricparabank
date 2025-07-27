namespace FabricParaBank.Tests.Model;

public record PayeeUser
{
    // Basic Payee Information
    public required string PayeeName { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public required string PhoneNumber { get; set; }

    // Account Details
    public required string AccountNumber { get; set; }
    public required string VerifyAccountNumber { get; set; }

    // Payment Details
    public decimal Amount { get; set; }

    // Optional: Validation Method
    public bool IsAccountVerified()
    {
        return AccountNumber == VerifyAccountNumber;
    }
}
 