namespace FabricParaBank.Tests.Model;

public record PayeeUser
{
    // Basic Payee Information
    public string PayeeName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string PhoneNumber { get; set; }

    // Account Details
    public string AccountNumber { get; set; }
    public string VerifyAccountNumber { get; set; }

    // Payment Details
    public decimal Amount { get; set; }

    // Optional: Validation Method
    public bool IsAccountVerified()
    {
        return AccountNumber == VerifyAccountNumber;
    }
}
 