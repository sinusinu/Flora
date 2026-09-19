namespace Flora;

public sealed class Config {
    public enum WindowModeOpts { Windowed, Fullscreen }
    public enum VSyncOpts { Disabled, Enabled, Adaptive }

    public string WindowTitle { get; set; } = "A Flora Application";
    public int WindowWidth { get; set; } = 640;
    public int WindowHeight { get; set; } = 480;
    public WindowModeOpts WindowMode { get; set; } = WindowModeOpts.Windowed;

    public VSyncOpts VSync { get; set; } = VSyncOpts.Enabled;
}