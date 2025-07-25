using Microsoft.Extensions.Configuration;

namespace FabricParaBank.Tests.Util.ConfigManager;

public static class ConfigLoader
{
    public static TestSettings Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        return config.GetSection("TestSettings").Get<TestSettings>()!;
    }
}