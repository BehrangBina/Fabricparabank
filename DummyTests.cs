using FabricParaBank.Tests.Util;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FabricParaBank.Tests;

public class DummyTests(ITestOutputHelper output)  

{
    private readonly ILogger _logger = output.ToLogger<DummyTests>();

    [Fact]
    public void Test1()
    {
       var un = Helper.GetUniqueUser(_logger);
    }
}