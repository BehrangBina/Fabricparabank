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
    private RegistrationPage? _registrationPage;
    private LoginPage? _loginPage;
    private AccountPage? _accountPage;
    [BeforeScenario]
    public async Task Setup()
    {
        _logger.LogInformation("Setup initiated");
        await InitializeAsync();
        _registrationPage = new RegistrationPage(Page, _logger);
        _loginPage = new LoginPage(Page, _logger);
        _accountPage = new AccountPage(Page, _logger);
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
       await _registrationPage!.ClickOnRegister();
       await _registrationPage.FillsTheUserInformation(scenarioContext);
    }

    [Then("I can see welcome message on the screen")]
    public async Task ThenICanSeeWelcomeMessageOnTheScreen()
    {
        await _registrationPage!.ValidateWelcomeMessage(scenarioContext);
    }

    [When("I log in using the newly registered user credentials")]
    public async Task WhenILogInUsingTheNewlyRegisteredUserCredentials()
    {
       await _loginPage!.PerformLogin(scenarioContext);
    }


    [Then("I can validate global navigation menu for the logged in user")]
    public async Task ThenICanValidateGlobalNavigationMenuForTheLoggedInUser(Table table)
    {
       await _loginPage!.CheckMenuNavigation(table);
    }

    [When("I create a new account of type {string}")]
    public async Task WhenICreateANewAccountOfTypeAndCaptureTheAccountNumber(string accountType)
    {
        await _loginPage!.ClickOnLink(accountType);
    }

}