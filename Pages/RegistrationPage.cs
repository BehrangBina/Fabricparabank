using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace FabricParaBank.Tests.Pages;

public class RegistrationPage(IPage page,ILogger logger)
{
    private const string RegisterLink = "text=Register";

    public async Task ClickOnRegister()
    {
        logger.LogInformation("Click on the register button with locator {Locator}" ,RegisterLink);
        await page.ClickAsync(RegisterLink);
    }
}