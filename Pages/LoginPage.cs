using FabricParaBank.Tests.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Reqnroll;

namespace FabricParaBank.Tests.Pages;

public class LoginPage(IPage page,ILogger logger)
{
    private const string UserNameInput = "[name='username']";
    private const string PasswordInput = "[password='password']";
    private const string LoginLink = "input[type='submit'][value='Log In']";

    public async Task PerformLogin(ScenarioContext scenarioContext)
    {
        logger.LogInformation("Performing login");
        var user= scenarioContext.Get<TestUser>("CurrentUser");

        logger.LogInformation("Fill userName and password, username {un}", user.Username);
        await page.FillAsync(UserNameInput, user.Username);
        await page.FillAsync(PasswordInput, user.Password);
        logger.LogInformation("User click logged in");
        await page.ClickAsync(LoginLink);
    }

    public async Task CheckMenuNavigation(Table table)
    {
        var expectedMenuItems = table.Rows.Select(r => r[0]).ToList();
        foreach (var menuItem in expectedMenuItems)
        {
            var locator = page.Locator($"#leftPanel >> text={menuItem}");
            logger.LogInformation("Checking menu item {item}", locator);
            var isVisible = await locator.IsVisibleAsync();
            isVisible.Should().BeTrue($"Menu item '{menuItem}' should be visible in the left panel");
        }
    }

    public async Task ClickOnLink(string accountType)
    {
        var link = page.Locator($"#leftPanel >> text={accountType.Trim()}");
        logger.LogInformation("Checking menu item {item}", link);
        await link.ClickAsync();
    }
}