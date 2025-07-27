using FabricParaBank.Tests.Model;
using FabricParaBank.Tests.Pages;
using FabricParaBank.Tests.Util;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Reqnroll;
using Xunit.Abstractions;

namespace FabricParaBank.Tests.StepDefinitions;


[Binding]
public class AccountManagementStepDefinitions(ScenarioContext ScenarioContext,ITestOutputHelper output) : PlaywrightTestBase
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
        await DisposeAsync(); 
    }

    [When("I register a new user with a unique username")]
    public async Task WhenIRegisterANewUserWithAUniqueUsername()
    {
       await _registrationPage!.ClickOnRegister();
       await _registrationPage.FillsTheUserInformation(ScenarioContext);
       await _registrationPage.ClickOnRegisterButton();
    }

    [Then("I can see welcome message on the screen")]
    public async Task ThenICanSeeWelcomeMessageOnTheScreen()
    {
        await _registrationPage!.ValidateWelcomeMessage(ScenarioContext);
    }

    [When("I log in using the newly registered user credentials")]
    public async Task WhenILogInUsingTheNewlyRegisteredUserCredentials()
    {
       await _loginPage!.PerformLogin(ScenarioContext);
    }


    [Then("I can validate global navigation menu for the logged in user")]
    public async Task ThenICanValidateGlobalNavigationMenuForTheLoggedInUser(Table table)
    {
       await _loginPage!.CheckMenuNavigation(table);
    }

    [When("I create a new account of type {string}")]
    public async Task WhenICreateANewAccountOfTypeAndCaptureTheAccountNumber(string accountType)
    {
        await _loginPage!.CreateAccountTypeAndValidateIt(accountType);
    }

    [Then("I create a {string} account and validate it")]
    public async Task ThenICreateAnAccountAndValidateIt(string accountType)
    {
        await _accountPage!.CreateAccountAndValidateIt(accountType,ScenarioContext);
    }

    [When("I login using the newly registered user credentials")]
    public async Task WhenILoginUsingTheNewlyRegisteredUserCredentials()
    {
        await _loginPage!.PerformLogin(ScenarioContext);
    }

    [Then("I logout from the system")]
    public async Task ThenILogoutFromTheSystem()
    {
        await _loginPage!.Logout();
    }

    [Then("I click on {string}")]
    [When("I click on {string}")]
    public async Task ThenIClick(string linkName)
    {
        await  _loginPage!.ClickOn(linkName);
    }

    [Then("I transfer {string} from the created account")]
    public async Task ThenITransferToTheCreatedAccount(string amount)
    {
        await _accountPage!.TransferAmount(amount,ScenarioContext);
    }

    [When("I pay a bill using the new account")]
    public async Task WhenIPayABillUsingTheNewAccount()
    {
        await _accountPage!.PayTheBillUsingTheNewAccount(ScenarioContext);
    }
    [When("I pay a bill using the new account with {string} amount")]
    public async Task WhenIPayABillUsingTheNewAccountWithAmount(string billAmount)
    {
        _logger.LogInformation("Paying a bill using the new account with amount: {BillAmount}", billAmount);
        ScenarioContext.Add(nameof(SharedData.BillPaymentAmount), billAmount);
        await _accountPage!.PayTheBillUsingTheNewAccount(ScenarioContext, billAmount);
    }
   
    [Then("the payment should be processed and balance updated")]
    public async Task ThenThePaymentShouldBeProcessedAndBalanceUpdated()
    {
        await _accountPage!.ValidatePaymentProcessed(ScenarioContext);
    }

    [Then("Transfer has been successfully completed")]
    public async Task ThenTransferHasBeenSuccessfullyCompleted()
    {
        await _accountPage!.ValidateTransferCompleted(ScenarioContext);
    }

    [Then("I can verify the bill payment was successful")]
    public async Task ThenICanVerifyTheBillPaymentWasSuccessful()
    {
        await _accountPage.ValidateBillPayment(ScenarioContext);
    }
}