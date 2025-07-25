using FabricParaBank.Tests.Util;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Reqnroll;
using Xunit.Abstractions;

namespace FabricParaBank.Tests.StepDefinitions;

[Binding]
public class AccountManagementStepDefinitions(ITestOutputHelper output) : PlaywrightTestBase
{
    private ILogger _logger = output.ToLogger<AccountManagementStepDefinitions>();

    [BeforeScenario]
    public async Task Setup()
    {
        _logger.LogInformation("Setup initiated");
        await InitializeAsync();
    }

    [Given("I navigate to the Para Bank application")]
    public async Task GivenINavigateToTheParaBankApplication()
    {
        var configUrl = Settings.BaseUrl;
        _logger.LogInformation("Navigating to the Para Bank application: {url}",configUrl);
        await Page.GotoAsync(configUrl);
        var url = Page.Url;
        _logger.LogInformation("Para Bank application: {url}", url);
        url.Should().Contain(configUrl,"because Para Bank application should use the following URL");
    }
    
    
    
    [AfterScenario]
    public async Task Teardown()
    {
        await DisposeAsync(); // clean shutdown
    }
}