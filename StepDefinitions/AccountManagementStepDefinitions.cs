using FabricParaBank.Tests.Pages;
using FabricParaBank.Tests.Util;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Reqnroll;
using Xunit.Abstractions;

namespace FabricParaBank.Tests.StepDefinitions;

[Binding]
public class AccountManagementStepDefinitions(ScenarioContext scenarioContext,ITestOutputHelper output) : PlaywrightTestBase
{
    private readonly ILogger _logger = output.ToLogger<AccountManagementStepDefinitions>();
    private RegistrationPage registrationPage;
    [BeforeScenario]
    public async Task Setup()
    {
        _logger.LogInformation("Setup initiated");
        await InitializeAsync();
        registrationPage = new RegistrationPage(Page, _logger);
    }

    [Given("I navigate to the Para Bank application")]
    public async Task GivenINavigateToTheParaBankApplication()
    {
        var configUrl = Settings.BaseUrl;
        
        _logger.LogInformation("Navigating to the Para Bank application: {url}",configUrl);
        await Page.GotoAsync(configUrl);
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        var url = Page.Url;
        _logger.LogInformation("Para Bank application: {url}", url);
        url.Should().Contain(configUrl,"because Para Bank application should use the following URL");
    }
    
    
    
    [AfterScenario]
    public async Task Teardown()
    {
        await DisposeAsync(); // clean shutdown
    }

    [When("I register a new user with a unique username")]
    public async Task WhenIRegisterANewUserWithAUniqueUsername()
    {
       await registrationPage.ClickOnRegister();
       await registrationPage.FillsTheUserInformation();
    }
}