using Bogus;
using FabricParaBank.Tests.Model;
using Microsoft.Extensions.Logging;

namespace FabricParaBank.Tests.Util;

public abstract class Helper
{
    private static string GetUniqueUser(ILogger logger)
    {
        const string username = "BehrangB";
        var uniqueName = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var userName = $"{username}-{uniqueName}";
        logger.LogInformation("User Name Set : {un}", userName);
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
            .RuleFor(u => u.Ssn, f => f.Random.Replace("###-##-####"))
            .RuleFor(u => u.Password, f => f.Internet.Password(10, true, prefix: "!Aa1"));
        var testUser = userFaker.Generate();
        testUser.Username = GetUniqueUser(logger);
        logger.LogInformation("Test User Created : {un}", testUser);
        return testUser;
    }

    public static PayeeUser CreateRandomPayee(string amount)
    {
        var faker = new Faker<PayeeUser>()
            .RuleFor(p => p.PayeeName, f => f.Name.FullName())
            .RuleFor(p => p.Address, f => f.Address.StreetAddress())
            .RuleFor(p => p.City, f => f.Address.City())
            .RuleFor(p => p.State, f => f.Address.StateAbbr())
            .RuleFor(p => p.ZipCode, f => f.Address.ZipCode())
            .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.AccountNumber, f => f.Finance.Account())
            .RuleFor(p => p.VerifyAccountNumber, (f, p) => p.AccountNumber) // match for verification
            .RuleFor(p => p.Amount,   Convert.ToDecimal(amount));

        return faker.Generate();
    }
}