using System.Runtime.InteropServices;
using SDL;

namespace Flora;

/// <summary>
/// Provide things related to the window.
/// </summary>
public sealed unsafe class Window {
    public enum WindowModeOpts { Windowed, Fullscreen }

    private Application app;
    internal SDL_Window* sdlWindow;
    internal SDL_Renderer* sdlRenderer;

    private string _windowTitle = "A Flora Application";
    public string WindowTitle {
        get => _windowTitle;
        set {
            _windowTitle = value;
            SDL3.SDL_SetWindowTitle(sdlWindow, value);
        }
    }

    public int WindowWidth { get { int width = 0; SDL3.SDL_GetWindowSize(sdlWindow, &width, null); return width; } }
    public int WindowHeight { get { int height = 0; SDL3.SDL_GetWindowSize(sdlWindow, null, &height); return height; } }
    public (int, int) WindowSize { get { int width = 0; int height = 0; SDL3.SDL_GetWindowSize(sdlWindow, &width, &height); return (width, height); } }

    public WindowModeOpts WindowMode { get; internal set; }

    internal Window(Application app, SDL_Window* window, SDL_Renderer* renderer) {
        this.app = app;
        sdlWindow = window;
        sdlRenderer = renderer;
    }

    /// <summary>
    /// Set window icon to specified image.
    /// </summary>
    /// <param name="path">Path to an image file. Supported types are: BMP, JPG, PNG.</param>
    public void SetIcon(string path) {
        if (!File.Exists(path)) throw new FileNotFoundException($"{path} does not exist!");
        using (FileStream fs = File.OpenRead(path)) SetIcon(fs);
    }

    /// <summary>
    /// Set window icon to specified image.
    /// </summary>
    /// <param name="stream">Stream of an image file. Supported types are: BMP, JPG, PNG.</param>
    public void SetIcon(Stream stream) {
        // TODO: do some error handling like a decent human being?
        SDL_Surface* surface;
        MemoryStream ms;

        if (stream is MemoryStream) {
            ms = (MemoryStream)stream;
        } else {
            ms = new MemoryStream();
            stream.CopyTo(ms);
        }

        // convert MemoryStream to SDL_IOStream*
        if (!ms.TryGetBuffer(out var buf)) buf = new ArraySegment<byte>(ms.ToArray());
        var handle = GCHandle.Alloc(buf.Array, GCHandleType.Pinned);
        byte* bufPtr = (byte*)handle.AddrOfPinnedObject() + buf.Offset;
        var ios = SDL3.SDL_IOFromConstMem((nint)bufPtr, (nuint)buf.Count);

        surface = SDL3.SDL_LoadSurface_IO(ios, false);

        SDL3.SDL_CloseIO(ios);
        handle.Free();
        ms.Dispose();

        SDL3.SDL_SetWindowIcon(sdlWindow, surface);
        SDL3.SDL_DestroySurface(surface);
    }
    
    /// <summary>
    /// Change window to windowed mode.
    /// </summary>
    /// <param name="width">Must be a positive integer</param>
    /// <param name="height">Must be a positive integer</param>
    public void SetWindowed(int width, int height) {
        if (width <= 0 || height <= 0) throw new InvalidOperationException("Width and Height must be >0");

        WindowMode = WindowModeOpts.Windowed;
        
        SDL3.SDL_SetWindowFullscreen(sdlWindow, false);
        SDL3.SDL_SetWindowSize(sdlWindow, width, height);
        SDL3.SDL_SyncWindow(sdlWindow);
    }
    
    /// <summary>
    /// Change window to borderless fullscreen mode.<br/>
    /// Note: Flora does not support exclusive fullscreen mode.
    /// </summary>
    public void SetFullscreen() {
        WindowMode = WindowModeOpts.Fullscreen;
        
        SDL3.SDL_SetWindowFullscreen(sdlWindow, true);
        SDL3.SDL_SyncWindow(sdlWindow);
    }
}