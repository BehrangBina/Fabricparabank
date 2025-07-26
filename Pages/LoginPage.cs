using FabricParaBank.Tests.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Reqnroll;

namespace FabricParaBank.Tests.Pages;

public class LoginPage(IPage page,ILogger logger)
{
    private const string UserNameInput = "[name='username']";
    private const string PasswordInput = "[name='password']";
    private const string LoginLink = "input[type='submit'][value='Log In']";
    private const string AccountPageTitle = "h1.title";

    public async Task PerformLogin(FeatureContext featureContext)
    {
        logger.LogInformation("Performing login");
        var user= featureContext.Get<TestUser>("CurrentUser");

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

    public async Task CreateAccountTypeAndValidateIt(string accountType)
    {
        var link = page.Locator($"#leftPanel >> text={accountType.Trim()}");
        logger.LogInformation("Checking menu item {item}", link);
        await link.ClickAsync();
        
        logger.LogInformation("Verifying Open Account Page");
        var title = await page.InnerTextAsync(AccountPageTitle);
        title.Should().Contain(accountType, "because we should be on the Open New Account page");
    }

    public async Task Logout()
    {
        var menuItem = "Log Out";
        var logout = page.Locator($"#leftPanel >> text={menuItem}");
        await logout.ClickAsync();
    }

    public async Task ClickOn(string linkName)
    {
       var linkLocator= page.Locator($"#leftPanel >> text={linkName}");
       logger.LogInformation("Clicking on link {link}", linkLocator);
       await linkLocator.ClickAsync();
    }
}