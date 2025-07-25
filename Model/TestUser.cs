using FabricParaBank.Tests.Util;

namespace FabricParaBank.Tests.Model;

public record TestUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string PhoneNumber { get; set; }
    public string SSN { get; set; }
    public string Username { get; set; } 
    public string Password { get; set; }
}