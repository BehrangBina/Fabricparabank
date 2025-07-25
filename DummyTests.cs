using FabricParaBank.Tests.Util;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FabricParaBank.Tests;

public class DummyTests(ITestOutputHelper output) : PlaywrightTestBase

{
    private readonly ILogger _logger = output.ToLogger<DummyTests>();

    [Fact]
    public async Task Test1()
    {
        await Page.GotoAsync(Settings.BaseUrl);
    }
}