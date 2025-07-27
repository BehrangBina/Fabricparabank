using System.Globalization;
using FabricParaBank.Tests.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Reqnroll;

namespace FabricParaBank.Tests.Pages;

public class AccountPage(IPage page,ILogger logger)
{

    private const string AccountTitle = "#openAccountForm .title";
    private const string AccountDropdown = "#type";
    private const string OpenAccountButton = "input[value='Open New Account']";
    private const string AccountId = "#fromAccountId";
    private const string AmountTextInput = "#amount";
    private const string TransferButton = "#transferForm input[value='Transfer']";
    private const string ToAccountNumber = "#transferForm #toAccountId";
    private const string FromAccountNumber = "#transferForm #fromAccountId";

    private const string NewAccountNumber = "#openAccountResult #newAccountId";
    // Payee information
    private const string PayeeName = "input[name='payee.name']";
    private const string StreetAddress = "input[name='payee.address.street']";
    private const string City = "input[name='payee.address.city']";
    private const string State = "input[name='payee.address.state']";
    private const string ZipCode = "input[name='payee.address.zipCode']";
    private const string PhoneNumber = "input[name='payee.phoneNumber']";
    private const string AccountNumber = "input[name='payee.accountNumber']";
    private const string VerifyAccount = "input[name='verifyAccount']";
    private const string Amount = "input[name='amount']";
    private const string SendPaymentButton = "input[value='Send Payment']";
    // transfer validation
    private const string TransferTitle = "#showResult .title";
    private const string TransferAmountValue = "#showResult #amountResult";
    private const string TransferFromValue = "#showResult #fromAccountIdResult";
    // Payment validation
    private const string PaymentFromAccountValue = "#billpayResult #fromAccountId";
    private const string PaymentAmountValue = "#billpayResult #amount";
    private const string PaymentPayeeName = "#billpayResult #payeeName";
    // account overview
    private const string AccountOverViewRows = "#showOverview #accountTable tbody tr";
    
    
    public async Task CreateAccountAndValidateIt(string accountType, ScenarioContext scenarioContext)
    {
        accountType = accountType.ToUpper();
        logger.LogInformation("Selecting account type: {AccountType}", accountType);
        var accountTypeInt = accountType.ToUpper() switch
        {
            "SAVINGS" => 1,
            "CHECKING" => 0,
            _ => throw new ArgumentOutOfRangeException($"Account Type: {accountType} not implemented")
        };

        await page.SelectOptionAsync(AccountDropdown, $"{accountTypeInt}");
        var dropdownText =await page.Locator(AccountDropdown).TextContentAsync();
        dropdownText.Should().Contain(accountType, $"because the {accountType}");
        logger.LogInformation("Validating {accountType} page",  accountType);
        await ValidateTitle();
        await StoreAccountNumber(scenarioContext);
        await ClickOnNewAccount();
        logger.LogInformation("Account created successfully with type {accountType}", accountType);
        await page.Locator(NewAccountNumber).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

        var accountNumber = await page.Locator(NewAccountNumber).TextContentAsync();
        accountNumber.Should().NotBeNullOrWhiteSpace("because the account number should not be empty");
        logger.LogInformation("New account number: {accountNumber}", accountNumber);
        scenarioContext.Add(nameof(SharedData.FromAccountId), accountNumber);
    }
    public async Task TransferAmount(string amount, ScenarioContext scenarioContext)
    {
        var amountSelector =  page.Locator(AmountTextInput);
        logger.LogInformation("Validating amount: {amountSelector}", amountSelector);
        scenarioContext.Add(nameof(SharedData.TransferAmount), amount);
        await amountSelector.FillAsync(amount);
        await TransferFromNewAccount(scenarioContext);
    }

    private async Task TransferFromNewAccount(ScenarioContext scenarioContext)
    {
        logger.LogInformation("Selecting billing for the new account");
        var toAccountNumber = scenarioContext.Get<string>(nameof(SharedData.ToAccountId));
        var fromAccountNumber = scenarioContext.Get<string>(nameof(SharedData.FromAccountId));

        logger.LogInformation("Transferring to  {toAccountNumber}", toAccountNumber);
        await page.SelectOptionAsync(FromAccountNumber, new SelectOptionValue { Label = fromAccountNumber });
        
        logger.LogInformation("selecting to account number: {newAccountNumber}", toAccountNumber);

        await page.SelectOptionAsync(ToAccountNumber, new SelectOptionValue { Label = toAccountNumber });
        var toAccountNumberTxt = await page.EvaluateAsync<string>($"() => document.querySelector('{ToAccountNumber}').value");
       
        if (toAccountNumberTxt.Contains(toAccountNumber))
        {
            await page.SelectOptionAsync(ToAccountNumber, new SelectOptionValue { Index = 0 });
        }
        else
        {
            await page.SelectOptionAsync(ToAccountNumber, new SelectOptionValue { Index = 1 });
        }

        logger.LogInformation("Clicking on transfer button {btn}", TransferButton);
        toAccountNumberTxt =  await page.EvaluateAsync<string>($"() => document.querySelector('{ToAccountNumber}').value");
        toAccountNumberTxt.Should().NotBeNullOrWhiteSpace("to account number should not be empty");
        await page.ClickAsync(TransferButton);
    }
    public async Task PayTheBillUsingTheNewAccount(ScenarioContext scenarioContext ,string billAmount)
    {
        logger.LogInformation("Generating random payee user");
        var payeeUser = scenarioContext.Get<TestUser>(nameof(SharedData.CurrentUser));
        logger.LogInformation("Filling payee information: {payeeUser}", payeeUser);
         
        logger.LogInformation("Filling payee name: {PayeeName}", payeeUser.Username);
        await page.FillAsync(PayeeName, payeeUser.Username);
        
        logger.LogInformation("Filling street address: {StreetAddress}", payeeUser.Address);
        await page.FillAsync(StreetAddress, payeeUser.Address);
        logger.LogInformation("Filling city: {City}", payeeUser.City);
        await page.FillAsync(City, payeeUser.City);
        logger.LogInformation("Filling state: {State}", payeeUser.State);
        await page.FillAsync(State, payeeUser.State);
        logger.LogInformation("Filling zip code: {ZipCode}", payeeUser.ZipCode);
        await page.FillAsync(ZipCode, payeeUser.ZipCode);
        logger.LogInformation("Filling phone number: {PhoneNumber}", payeeUser.PhoneNumber);
        await page.FillAsync(PhoneNumber, payeeUser.PhoneNumber);
        
        var accountId = scenarioContext.Get<string>(nameof(SharedData.FromAccountId));
        logger.LogInformation("Filling account number: {AccountNumber}", accountId);
        await page.FillAsync(AccountNumber, accountId);
        await page.FillAsync(VerifyAccount, accountId);
        
        logger.LogInformation("Filling amount: {Amount}", billAmount);
        await page.FillAsync(Amount, Convert.ToDecimal(billAmount).ToString(CultureInfo.CurrentCulture));
        
        logger.LogInformation("Clicking on send payment button {SendPaymentButton}", SendPaymentButton);
        scenarioContext.Add(nameof(SharedData.PayeeUser), payeeUser);
        await page.ClickAsync(SendPaymentButton);
    }
    
    public async Task PayTheBillUsingTheNewAccount(ScenarioContext scenarioContext)
    {
        logger.LogInformation("Generating random payee user");
        var payeeUser = scenarioContext.Get<TestUser>(nameof(SharedData.CurrentUser));
        logger.LogInformation("Filling payee information: {payeeUser}", payeeUser);
         
        logger.LogInformation("Filling payee name: {PayeeName}", payeeUser.Username);
        await page.FillAsync(PayeeName, payeeUser.Username);
        
        logger.LogInformation("Filling street address: {StreetAddress}", payeeUser.Address);
        await page.FillAsync(StreetAddress, payeeUser.Address);
        logger.LogInformation("Filling city: {City}", payeeUser.City);
        await page.FillAsync(City, payeeUser.City);
        logger.LogInformation("Filling state: {State}", payeeUser.State);
        await page.FillAsync(State, payeeUser.State);
        logger.LogInformation("Filling zip code: {ZipCode}", payeeUser.ZipCode);
        await page.FillAsync(ZipCode, payeeUser.ZipCode);
        logger.LogInformation("Filling phone number: {PhoneNumber}", payeeUser.PhoneNumber);
        await page.FillAsync(PhoneNumber, payeeUser.PhoneNumber);
        
        var accountId = scenarioContext.Get<string>(nameof(SharedData.FromAccountId));
        logger.LogInformation("Filling account number: {AccountNumber}", accountId);
        await page.FillAsync(AccountNumber, accountId);
        await page.FillAsync(VerifyAccount, accountId);
        
        var amount= scenarioContext.Get<string>(nameof(SharedData.TransferAmount));
        logger.LogInformation("Filling amount: {Amount}", amount);
        await page.FillAsync(Amount, Convert.ToInt32(amount). ToString("F2"));
        
        logger.LogInformation("Clicking on send payment button {SendPaymentButton}", SendPaymentButton);
        scenarioContext.Add(nameof(SharedData.PayeeUser), payeeUser);
        await page.ClickAsync(SendPaymentButton);
    }
    private async Task StoreAccountNumber(ScenarioContext scenarioContext)
    {
        var accountNumber = await page.Locator(AccountId).TextContentAsync();
        if (string.IsNullOrEmpty(accountNumber))
        {
            Thread.Sleep(300);
            accountNumber = await page.Locator(AccountId).TextContentAsync();
        }
        accountNumber.Should().NotBeNullOrWhiteSpace();
        logger.LogInformation("Validating {accountNumber}", accountNumber);
        scenarioContext.Add(nameof(SharedData.ToAccountId), accountNumber);
    }

    private async Task ValidateTitle()
    {
        var selector = page.Locator(AccountTitle);
        logger.LogInformation("Validating title: {titleSelector}", AccountTitle);
        const string expectedText = "Open New Account";
        var actualText =await selector.TextContentAsync();
        actualText.Should()
            .NotBeEmpty("title should not be empty")
            .And
            .Contain(expectedText, "because we should be on the Open New Account page");
    }
    private async Task ClickOnNewAccount()
    {
        var locator =  page.Locator(OpenAccountButton);
        logger.LogInformation("Clicking on the Open New Account page");
        await locator.ClickAsync();
    }

    public async Task ValidatePaymentProcessed(ScenarioContext scenarioContext)
    {
      
        var fromAccountId = scenarioContext.Get<string>(nameof(SharedData.FromAccountId));
        var toAccountId = scenarioContext.Get<string>(nameof(SharedData.ToAccountId));
         
        // Expected data
        var expectedAccounts = new[]
        {
            new { Id = toAccountId, Balance = "$415.50", Available = "415.50" },
            new { Id = fromAccountId, Balance = "$100.00", Available = "$100.00" },
            new { Id = "Total", Balance = "$515.50", Available = "0" }
        };
        await page.WaitForSelectorAsync(AccountOverViewRows, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
        var rows = await page.QuerySelectorAllAsync(AccountOverViewRows);
        
        //total balance
        var totalRow = rows.Last();
        var totalBalance = await ((await totalRow.QuerySelectorAsync("td:nth-child(2)"))!).TextContentAsync();
        
        logger.LogInformation("Validating the first row");
        
        var rowId = 0;
        var firstRow = rows[rowId];
        var firstAccountId = await ((await firstRow.QuerySelectorAsync("td:nth-child(1)"))!).TextContentAsync();
        var firstBalance = await ((await firstRow.QuerySelectorAsync("td:nth-child(2)"))!).TextContentAsync();
        firstAccountId!.Trim().Should().NotBeNullOrWhiteSpace().And.Contain(expectedAccounts[rowId].Id,
            "because the first account id should match the expected id");
        firstBalance!.Trim().Should().NotBeNullOrWhiteSpace().And.Contain(expectedAccounts[rowId].Balance,
            "because the first account balance should match the expected balance"); 
        
        rowId = 1;
        logger.LogInformation("Validating the second row");
        var secondRow = rows[rowId];
        var secondAccountId = await ((await secondRow.QuerySelectorAsync("td:nth-child(1)"))!).TextContentAsync();
        var secondBalance = await ((await secondRow.QuerySelectorAsync("td:nth-child(2)"))!).TextContentAsync();
        secondAccountId!.Trim().Should().NotBeNullOrWhiteSpace().And.Contain(expectedAccounts[rowId].Id,
            "because the second account id should match the expected id");
        secondBalance!.Trim().Should().NotBeNullOrWhiteSpace().And.Contain(expectedAccounts[rowId].Balance,
            "because the second account balance should match the expected balance");
        
        rowId = 2;
        logger.LogInformation("Validating the third row");
        var thirdRow = rows[rowId];
        var thirdAccountId = await ((await thirdRow.QuerySelectorAsync("td:nth-child(2)"))!).TextContentAsync();
        thirdAccountId.Should().NotBeNullOrEmpty().And.Contain(totalBalance,"because the total balance should match");
    }

    public async Task ValidateTransferCompleted(ScenarioContext scenarioContext)
    {
        var accountId = scenarioContext.Get<string>(nameof(SharedData.FromAccountId));
        var currentUser = scenarioContext.Get<TestUser>(nameof(SharedData.CurrentUser));
        var transferAmount = scenarioContext.Get<string>(nameof(SharedData.TransferAmount));
     
        const string expectedTitle = "Transfer Complete!";
        var transferTitle = await page.Locator(TransferTitle).TextContentAsync();
        transferTitle.Should().Contain(expectedTitle, "because we should be on the Transfer Complete page");
        
        await page.WaitForSelectorAsync(TransferAmountValue, new PageWaitForSelectorOptions
            { State = WaitForSelectorState.Visible });
        var currentAmount = await page.Locator(TransferAmountValue).TextContentAsync();
        currentAmount.Should().Contain(transferAmount, "because the transfer amount should match the expected amount");

        var fromAccountId = await page.Locator(TransferFromValue).TextContentAsync();
        fromAccountId.Should().Contain(accountId, "because the from account id should match the expected account id");
        
        logger.LogInformation("Transfer completed successfully for user {user} with account {accountId} and amount {amount}",
            currentUser.Username, accountId, transferAmount);
        scenarioContext.Add(nameof(SharedData.TransferCompleted), true);
    }

    public async Task ValidateBillPayment(ScenarioContext scenarioContext)
    {
        logger.LogInformation("Validating bill payment");
 
        var expectedPayeeUser = scenarioContext.Get<TestUser>(nameof(SharedData.PayeeUser));
        var expectedFromAccountId = scenarioContext.Get<string>(nameof(SharedData.FromAccountId));
        var expectedAmount = scenarioContext.Get<string>(nameof(SharedData.BillPaymentAmount));
        
        var fromAccountId = await page.Locator(PaymentFromAccountValue).TextContentAsync();
        if(string.IsNullOrEmpty(fromAccountId))
        {
            Thread.Sleep(300);
            fromAccountId = await page.Locator(PaymentFromAccountValue).TextContentAsync();
        }
        fromAccountId.Should().Contain(expectedFromAccountId, "because the from account id should match the expected account id");
        
        var amount = await page.Locator(PaymentAmountValue).TextContentAsync();
        amount.Should().Contain(expectedAmount, "because the payment amount should match the expected amount");
        
        var payeeName = await page.Locator(PaymentPayeeName).TextContentAsync();
        payeeName.Should().Contain(expectedPayeeUser.Username, "because the payee name should match the expected payee name");
        
        logger.LogInformation("Bill payment validated successfully for user {user} with account {accountId} and amount {amount}",
            expectedPayeeUser.Username, expectedFromAccountId, expectedAmount);
    }
}