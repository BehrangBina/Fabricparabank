using FabricParaBank.Tests.Model;
using FabricParaBank.Tests.Util;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Reqnroll;

namespace FabricParaBank.Tests.Pages;

public class RegistrationPage(IPage page,ILogger logger)
{
    private const string RegisterLink = "text=Register";
    private const string FirstNameInput = "[name='customer.firstName']";
    private const string LastNameInput = "[name='customer.lastName']";
    private const string AddressInput = "[name='customer.address.street']";
    private const string CityInput = "[name='customer.address.city']";
    private const string StateInput = "[name='customer.address.state']";
    private const string ZipCodeInput = "[name='customer.address.zipCode']";
    private const string PhoneNumberInput = "[name='customer.phoneNumber']";
    private const string SsnInput = "[name='customer.ssn']";
    private const string UsernameInput = "[name='customer.username']";
    private const string PasswordInput = "[name='customer.password']";
    private const string RepeatedPasswordInput = "[name='repeatedPassword']";
    private const string WelcomeMessage = "#rightPanel h1.title";
    private const string SubmitButton = "[value='Register']";
    public async Task ClickOnRegister()
    {
        logger.LogInformation("Click on the register button with locator {Locator}" ,RegisterLink);
        await page.ClickAsync(RegisterLink);
    }

    public async Task FillsTheUserInformation(FeatureContext featureContext)
    {
        var userData = Helper.GetTestUser(logger);
        logger.LogInformation("Fills user data with locator {Locator} value {Value}", FirstNameInput, userData.FirstName);
        await page.FillAsync(FirstNameInput, userData.FirstName);
        
        logger.LogInformation("Fills user data with locator {Locator} value {Value}", LastNameInput, userData.LastName);
        await page.FillAsync(LastNameInput, userData.LastName);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", AddressInput, userData.Address);
        await page.FillAsync(AddressInput, userData.Address);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", CityInput, userData.City);
        await page.FillAsync(CityInput, userData.City);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", StateInput, userData.State);
        await page.FillAsync(StateInput, userData.State);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", ZipCodeInput, userData.ZipCode);
        await page.FillAsync(ZipCodeInput, userData.ZipCode);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", PhoneNumberInput, userData.PhoneNumber);
        await page.FillAsync(PhoneNumberInput, userData.PhoneNumber);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", SsnInput, userData.Ssn);
        await page.FillAsync(SsnInput, userData.Ssn);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", UsernameInput, userData.Username);
        await page.FillAsync(UsernameInput, userData.Username);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", PasswordInput, userData.Password);
        await page.FillAsync(PasswordInput, userData.Password);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", RepeatedPasswordInput, userData.Password);
        await page.FillAsync(RepeatedPasswordInput, userData.Password);
        
        logger.LogInformation("adding user to the scenario context {sc}",userData);
        featureContext.Add("CurrentUser", userData);
    }

    public async Task ValidateWelcomeMessage(FeatureContext featureContext)
    {
        logger.LogInformation("ValidateWelcomeMessage");
        logger.LogInformation("Getting user name from scenario context");
        var testUser = featureContext.Get<TestUser>("CurrentUser");
        var userName = testUser.Username;
        var expectedMessage = $"Welcome {userName}";
        var successMessage = await page.Locator(WelcomeMessage).InnerTextAsync();
        successMessage.Trim()
            .Should()
            .Contain(expectedMessage,
            "because the user should see a success message after registration");
    }

    public async Task ClickOnRegisterButton()
    {
        logger.LogInformation("Click on the register button");
        await page.Locator(SubmitButton).ClickAsync();
    }


}