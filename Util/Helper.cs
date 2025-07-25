using Microsoft.Extensions.Logging;

namespace FabricParaBank.Tests.Util;

public abstract class Helper
{
    public static string GetUniqueUser(ILogger logger)
    {
        const string username= "BehrangB";
        var uniqueName =  (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var userName=  $"{username}-{uniqueName}";
        logger.LogInformation("User Name Set : {un}",userName);
        return userName;
    }
}