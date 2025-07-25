using FabricParaBank.Tests.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

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
    private const string SSNInput = "[name='customer.ssn']";
    private const string UsernameInput = "[name='customer.username']";
    private const string PasswordInput = "[name='customer.password']";
    private const string RepeatedPasswordInput = "[name='repeatedPassword']";

    public async Task ClickOnRegister()
    {
        logger.LogInformation("Click on the register button with locator {Locator}" ,RegisterLink);
        await page.ClickAsync(RegisterLink);
    }

    public async Task FillsTheUserInformation()
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

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", SSNInput, userData.SSN);
        await page.FillAsync(SSNInput, userData.SSN);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", UsernameInput, userData.Username);
        await page.FillAsync(UsernameInput, userData.Username);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", PasswordInput, userData.Password);
        await page.FillAsync(PasswordInput, userData.Password);

        logger.LogInformation("Fills user data with locator {Locator} value {Value}", RepeatedPasswordInput, userData.Password);
        await page.FillAsync(RepeatedPasswordInput, userData.Password);

    }
}