using FabricParaBank.Tests.Model;
using FabricParaBank.Tests.Util;
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
    // account overview
    private const string AccountOverViewRows = "#showOverview #accountTable tbody tr";
    
    
    public async Task CreateAccountAndValidateIt(string accountType, FeatureContext featureContext)
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
        await StoreAccountNumber(featureContext);
        await ClickOnNewAccount();
    }
    public async Task TransferAmount(string amount, FeatureContext featureContext)
    {
        var amountSelector =  page.Locator(AmountTextInput);
        logger.LogInformation("Validating amount: {amountSelector}", amountSelector);
        featureContext.Add("TransferAmount", amount);
        await amountSelector.FillAsync(amount);
        await TransferFromNewAccount(featureContext);
    }

    private async Task TransferFromNewAccount(FeatureContext featureContext)
    {
        logger.LogInformation("Selecting billing for the new account");
        var newAccountNumber = featureContext.Get<string>("AccountId");
        
        logger.LogInformation("Transferring to  {newAccountNumber}", newAccountNumber);
        await page.SelectOptionAsync(FromAccountNumber, new SelectOptionValue { Label = newAccountNumber });
        
        var toAccountNumber = page.Locator(ToAccountNumber);
        logger.LogInformation("Validating to account number: {toAccountNumber}", toAccountNumber);
        logger.LogInformation("selecting to account number: {newAccountNumber}", newAccountNumber);

        await page.SelectOptionAsync(ToAccountNumber, new SelectOptionValue { Label = newAccountNumber });
        var toAccountNumberTxt = await page.EvaluateAsync<string>("() => document.querySelector('#toAccountId').value");
          
        if (toAccountNumberTxt!.Contains(newAccountNumber))
        {
            await page.SelectOptionAsync(ToAccountNumber, new SelectOptionValue { Index = 1 });
        }
        else
        {
            await page.SelectOptionAsync(ToAccountNumber, new SelectOptionValue { Index = 0 });
        }

        logger.LogInformation("Clicking on transfer button {btn}", TransferButton);
        toAccountNumberTxt =  await page.EvaluateAsync<string>("() => document.querySelector('#toAccountId').value");
        toAccountNumberTxt.Should().NotBeNullOrWhiteSpace("to account number should not be empty");
        featureContext.Add("ToAccountId", toAccountNumberTxt);
        await page.ClickAsync(TransferButton);
    }

    public async Task PayTheBillUsingTheNewAccount(FeatureContext featureContext)
    {
        logger.LogInformation("Generating random payee user");
        var transferAmount = featureContext.Get<string>("TransferAmount");
        var payeeUser = Helper.CreateRandomPayee(transferAmount);
        logger.LogInformation("Filling payee information: {payeeUser}", payeeUser);
        logger.LogInformation("Filling payee name: {PayeeName}", payeeUser.PayeeName);
        await page.FillAsync(PayeeName, payeeUser.PayeeName);
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
        logger.LogInformation("Filling account number: {AccountNumber}", payeeUser.AccountNumber);
        await page.FillAsync(AccountNumber, payeeUser.AccountNumber);
        await page.FillAsync(VerifyAccount, payeeUser.VerifyAccountNumber);
        
        logger.LogInformation("Filling amount: {Amount}", payeeUser.Amount.ToString("F2"));
        await page.FillAsync(Amount, payeeUser.Amount.ToString("F2"));
        logger.LogInformation("Clicking on send payment button {SendPaymentButton}", SendPaymentButton);
        featureContext.Add("PayeeUser", payeeUser);
        await page.ClickAsync(SendPaymentButton);
    }
    private async Task StoreAccountNumber(FeatureContext featureContext)
    {
        var accountNumber = await page.Locator(AccountId).TextContentAsync();
        accountNumber.Should().NotBeNullOrWhiteSpace();
        logger.LogInformation("Validating {accountNumber}", accountNumber);
        featureContext.Add("AccountId", accountNumber);
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

    public async Task ValidatePaymentProcessed(FeatureContext featureContext)
    {
      
        var fromAccountId = featureContext.Get<string>("AccountId");
        var toAccountId = featureContext.Get<string>("ToAccountId");
         
        // Expected data
        var expectedAccounts = new[]
        {
            new { Id = fromAccountId, Balance = "$215.50", Available = "$215.50" },
            new { Id = toAccountId, Balance = "$200.00", Available = "$200.00" },
            new { Id = "Total", Balance = "$415.50", Available = "$415.50" }
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

    public async Task ValidateTransferCompleted(FeatureContext featureContext)
    {
        var accountId = featureContext.Get<string>("AccountId");
        var currentUser = featureContext.Get<TestUser>("CurrentUser");
        var transferAmount = featureContext.Get<string>("TransferAmount");

     
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
        featureContext.Add("TransferCompleted", true);
    }
}