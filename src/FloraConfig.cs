using System.Reflection;

namespace Flora;

public struct FloraConfig {
    /// <summary>
    /// Initial width of the window.
    /// </summary>
    public int width;

    /// <summary>
    /// Initial height of the window.
    /// </summary>
    public int height;

    /// <summary>
    /// Title of the window.
    /// </summary>
    public string windowTitle;

    /// <summary>
    /// Create ApplicationConfiguration with default configuration.
    /// </summary>
    public FloraConfig() : this(640, 480, Assembly.GetCallingAssembly().GetName().Name) { }

    /// <summary>
    /// Create ApplicationConfiguration with given configuration.
    /// </summary>
    public FloraConfig(int width, int height, string windowTitle) {
        this.width = width;
        this.height = height;
        this.windowTitle = windowTitle;
    }
}