using FabricParaBank.Tests.Util.ConfigManager;
using Microsoft.Playwright;

namespace FabricParaBank.Tests.Util;

public abstract class PlaywrightTestBase : IAsyncLifetime
{
    protected IPlaywright Playwright { get; private set; } = null!;
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected TestSettings Settings { get; private set; } = ConfigLoader.Load();


    public async Task InitializeAsync()
    {
        Settings = ConfigLoader.Load();

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var browserType = Settings.Browser.Type.ToLower() switch
        {
            "firefox" => Playwright.Firefox,
            "webkit" => Playwright.Webkit,
            _ => Playwright.Chromium
        };

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = Settings.Browser.Headless,
            SlowMo = Settings.Browser.SlowMotion ? Settings.Browser.SlowMotionDelay : null
        };

        Browser = await browserType.LaunchAsync(launchOptions);

        var contextOptions = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = Settings.Browser.Viewport.Width,
                Height = Settings.Browser.Viewport.Height
            }
        };

        Context = await Browser.NewContextAsync(contextOptions);
        Page = await Context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}