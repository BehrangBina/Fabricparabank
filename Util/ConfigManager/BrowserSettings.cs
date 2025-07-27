namespace FabricParaBank.Tests.Util.ConfigManager;

public class BrowserSettings
{
    public string Type { get; set; } = "chromium";
    public bool Headless { get; set; }
    public bool SlowMotion { get; set; }
    public int SlowMotionDelay { get; set; }
    public required ViewportSettings Viewport { get; set; }
}