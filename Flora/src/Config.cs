namespace Flora;

/// <summary>
/// Default configurations to use while initializing Flora.<br/>
/// After initializing, changing values in <c>Config</c> object will do nothing.<br/>
/// Use appropriate functions to change things after the initialization.
/// </summary>
public class Config {
    public string                WindowTitle { get; init; } = "A Flora Application";
    public Window.WindowModeOpts WindowMode { get; init; } = Window.WindowModeOpts.Windowed;
    public int                   WindowWidth { get; init; } = 640;
    public int                   WindowHeight { get; init; } = 480;
    public Graphics.VSyncOpts    VSync { get; init; } = Graphics.VSyncOpts.Enabled;
    /// <summary>This is a hint; value may not be honored.</summary>
    public int                   AudioBufferSize { get; init; } = 1024;

    public Config() {}
};