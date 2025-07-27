using FabricParaBank.Tests.Util.ConfigManager;

namespace FabricParaBank.Tests.Util;

public class ApiTestHelper
{
    private static string BaseUrl() => ConfigLoader.Load().BaseUrl;
    public static string GetAccountTransactionsUrl(int accountId, string amount)
    {
        return $"{BaseUrl()}/parabank/services/bank/accounts/{accountId}/transactions/amount/{amount}";
    }
}