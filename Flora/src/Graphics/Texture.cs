using SDL;
using StbiSharp;

namespace Flora;

/// <summary>
/// Image to draw.
/// </summary>
public unsafe class Texture : IDisposable {
    internal SDL_Texture* texture;
    
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    // i love microoptimizations!!
    internal float invW;
    internal float invH;

    public enum ScaleModeOpts {
        Nearest     = SDL_ScaleMode.SDL_SCALEMODE_NEAREST,
        Linear      = SDL_ScaleMode.SDL_SCALEMODE_LINEAR,
        PixelArt    = SDL_ScaleMode.SDL_SCALEMODE_PIXELART,
    }
    public ScaleModeOpts ScaleMode { get; set; }

    public enum BlendModeOpts : uint {
        None                = SDL_BlendMode.SDL_BLENDMODE_NONE,
        Blend               = SDL_BlendMode.SDL_BLENDMODE_BLEND,
        BlendPremultiplied  = SDL_BlendMode.SDL_BLENDMODE_BLEND_PREMULTIPLIED,
        Add                 = SDL_BlendMode.SDL_BLENDMODE_ADD,
        AddPremultiplied    = SDL_BlendMode.SDL_BLENDMODE_ADD_PREMULTIPLIED,
        Mod                 = SDL_BlendMode.SDL_BLENDMODE_MOD,
        Mul                 = SDL_BlendMode.SDL_BLENDMODE_MUL,
    }
    public BlendModeOpts BlendMode { get; set; }

    internal Texture(Application app, string path, ScaleModeOpts scaleMode) {
        StbiImage? stbiImage = null;

        using (var stream = File.OpenRead(path))
        using (var ms = new MemoryStream()) {
            stream.CopyTo(ms);
            stbiImage = Stbi.LoadFromMemory(ms, 4);
        }

        texture = SDL3.SDL_CreateTexture(app.Window.sdlRenderer, SDL_PixelFormat.SDL_PIXELFORMAT_ABGR8888, 0, stbiImage.Width, stbiImage.Height);

        fixed (byte* dataRef = stbiImage.Data) {
            SDL3.SDL_UpdateTexture(texture, null, new nint(dataRef), stbiImage.Width * 4);
        }

        BlendMode = BlendModeOpts.Blend;
        SDL3.SDL_SetTextureBlendMode(texture, SDL_BlendMode.SDL_BLENDMODE_BLEND);
        ScaleMode = scaleMode;

        Width = stbiImage.Width;
        Height = stbiImage.Height;

        invW = 1f / Width;
        invH = 1f / Height;

        stbiImage.Dispose();
        stbiImage = null;
    }

    internal Texture(SDL_Texture* texture) {
        this.texture = texture;

        SDL_BlendMode blendMode;
        SDL_ScaleMode scaleMode;
        SDL3.SDL_GetTextureBlendMode(texture, &blendMode);
        SDL3.SDL_GetTextureScaleMode(texture, &scaleMode);
        BlendMode = (BlendModeOpts)blendMode;
        ScaleMode = (ScaleModeOpts)scaleMode;

        float w = 0; float h = 0;
        SDL3.SDL_GetTextureSize(texture, &w, &h);
        Width = (int)w;
        Height = (int)h;

        invW = 1f / Width;
        invH = 1f / Height;
    }

#region Dispose
    private bool _disposed = false;

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;

        /* if (disposing) {} */

        SDL3.SDL_DestroyTexture(texture);

        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Texture() => Dispose(false);
#endregion Dispose
}