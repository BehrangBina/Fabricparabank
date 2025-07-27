using System.Globalization;
using System.Text.Json;
using FabricParaBank.Tests.Model;
using FabricParaBank.Tests.Util;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Reqnroll;
using Xunit.Abstractions;

namespace FabricParaBank.Tests.StepDefinitions;

[Binding]
public class ApiStepDefinitions(ScenarioContext ScenarioContext, ITestOutputHelper output)
{
    private readonly ILogger _logger = output.ToLogger<ApiStepDefinitions>();

    [Given("the banking workflow has been completed for the user")]
    public void GivenTheBankingWorkflowHasBeenCompletedForTheUser()
    {
        _logger.LogInformation("Starting the banking workflow for the user");
        _logger.LogInformation("Verifying account creating status");
        ScenarioContext.Get<bool>("AccountCreated")
            .Should()
            .BeTrue("because the account should have been created successfully in the previous step");
    }

    [Then("the details displayed in the JSON response are valid")]
    public void ThenTheDetailsDisplayedInTheJsonResponseAreValid()
    {
        _logger.LogInformation("Validating the JSON response details");
        var transactionsResp = ScenarioContext.Get<string>(nameof(SharedData.TransactionResponseString));
        var transactions = JsonSerializer.Deserialize<List<TransactionResponse>>(transactionsResp) ??
                           throw new JsonException($"Failed to deserialize response content: {transactionsResp}");
        
        var fromAccountId = ScenarioContext.Get<string>(nameof(SharedData.FromAccountId));
        var transferAmount = ScenarioContext.Get<string>(nameof(SharedData.TransferAmount));
        _logger.LogInformation("Current Response: {Response}", transactionsResp);
        
        _logger.LogInformation(
            "Validating transactions for FromAccountId: {FromAccountId}, TransferAmount: {TransferAmount}",
            fromAccountId, transferAmount);

        _logger.LogInformation("There should be 2 records in the response");
        transactions.Should()
            .NotBeNullOrEmpty("because there should be at least one transaction in the response")
            .And.HaveCount(2,
                "because two transactions should be present: one for the transfer and one for the deposit");
        
        transactions[0].AccountId.ToString().Should().Be(fromAccountId, "because the first transaction should match the FromAccountId");
        transactions[0].Amount.ToString(CultureInfo.InvariantCulture).Should().Contain(transferAmount, "because the first transaction should match the TransferAmount");
        transactions[1].AccountId.ToString().Should().Be(fromAccountId, "because the second transaction should match the ToAccountId");
        transactions[1].Amount.ToString(CultureInfo.InvariantCulture).Should().Contain(transferAmount, "because the second transaction should match the TransferAmount");
        
        _logger.LogInformation("All transaction details are valid");
        _logger.LogInformation("Transaction validation completed successfully");
    }

    [When("I Search transaction from the newly account created")]
    public async Task WhenISearchTransactionFromTheNewlyAccountCreated()
    {
        var accountIdStr = ScenarioContext.Get<string>(nameof(SharedData.FromAccountId));
        var amount = ScenarioContext.Get<string>(nameof(SharedData.TransferAmount));
        var accountId = Convert.ToInt32(accountIdStr);

        _logger.LogInformation("Searching transactions for account ID: {AccountId}", accountId);

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        var url = ApiTestHelper.GetAccountTransactionsUrl(accountId, amount);
        _logger.LogInformation("Making API call to URL: {Url}", url);
        var response = await httpClient.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();

        ScenarioContext.Add(nameof(SharedData.TransactionResponseString), responseContent);
    }
}