using FabricParaBank.Tests.Util;

namespace FabricParaBank.Tests.Model;

public record TestUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Ssn { get; set; }
    public required string Username { get; set; } 
    public required string Password { get; set; }
}