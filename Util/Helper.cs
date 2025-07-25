using Bogus;
using FabricParaBank.Tests.Model;
using Microsoft.Extensions.Logging;

namespace FabricParaBank.Tests.Util;

public abstract class Helper
{
    public static string GetUniqueUser(ILogger logger)
    {
        const string username= "BehrangB";
        var uniqueName =  (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var userName=  $"{username}-{uniqueName}";
        logger.LogInformation("User Name Set : {un}",userName);
        return userName;
    }

    public static TestUser GetTestUser(ILogger logger)
    {
        var userFaker = new Faker<TestUser>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Address, f => f.Address.StreetAddress())
            .RuleFor(u => u.City, f => f.Address.City())
            .RuleFor(u => u.State, f => f.Address.StateAbbr())
            .RuleFor(u => u.ZipCode, f => f.Address.ZipCode())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("###-###-####"))
            .RuleFor(u => u.SSN, f => f.Random.Replace("###-##-####"))
            .RuleFor(u => u.Password, f => f.Internet.Password(10, true, prefix: "!Aa1"));
        var testUser = userFaker.Generate();
        testUser.Username= GetUniqueUser(logger);
        logger.LogInformation("Test User Created : {un}", testUser);
        return testUser;
    }
}