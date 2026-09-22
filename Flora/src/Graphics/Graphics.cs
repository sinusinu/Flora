using System.Numerics;
using SDL;

namespace Flora;

public unsafe sealed class Graphics {
    private const float RadToDeg = 0.0174533f;

    private Application app;

    internal bool isDrawing = false;

    public Color ClearColor { get; set; } = new(0f, 0f, 0f, 1f);
    public Color RenderColor { get; set; } = new(1f, 1f, 1f, 1f);

    public Camera Camera { get; init; }
    // TODO: better not do alloc on render loop...
    private List<DrawCommand> drawCommandBuffer = new(1024);
    private readonly int[] fixedIndices = [ 0, 1, 2, 2, 1, 3 ];

    internal Graphics(Application application) {
        app = application;
        Camera = new(app);
    }

    public enum VSyncOpts { Disabled, Enabled, Adaptive }
    private VSyncOpts _vsync;
    public VSyncOpts VSync {
        get => _vsync;
        set {
            _vsync = value;
            switch (value) {
                case VSyncOpts.Disabled: SDL3.SDL_SetRenderVSync(app.Window.sdlRenderer, SDL3.SDL_RENDERER_VSYNC_DISABLED); break;
                case VSyncOpts.Enabled:  SDL3.SDL_SetRenderVSync(app.Window.sdlRenderer, 1); break;
                case VSyncOpts.Adaptive: SDL3.SDL_SetRenderVSync(app.Window.sdlRenderer, SDL3.SDL_RENDERER_VSYNC_ADAPTIVE); break;
            }
        }
    }

    public Texture CreateTexture(string path, Texture.ScaleModeOpts scaleMode = Texture.ScaleModeOpts.Linear) {
        return new Texture(app, path, scaleMode);
    }

    public Font CreateFont(string path, float size, Texture.ScaleModeOpts scaleMode = Texture.ScaleModeOpts.Linear) {
        return new Font(app, path, size, scaleMode);
    }

    public enum ViewportOpts {
        Stretch = SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_STRETCH,
        Letterbox = SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_LETTERBOX,
        Overscan = SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_OVERSCAN,
        IntegerScale = SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_INTEGER_SCALE,
    }

    /// <summary>Camera rotation does not apply!</summary>
    public Rect GetVisibleArea() {
        if (Camera.ScaleX == 0 || Camera.ScaleY == 0) return new Rect(0, 0, 0, 0);
        int w = 0; int h = 0; SDL_RendererLogicalPresentation rlp;
        SDL3.SDL_GetRenderLogicalPresentation(app.Window.sdlRenderer, &w, &h, &rlp);
        switch (rlp) {
            case SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_DISABLED:
                (w, h) = app.Window.WindowSize;
                return new Rect((-(w / 2) / Camera.ScaleX) + Camera.X, (-(h / 2) / Camera.ScaleY) + Camera.Y, w / Camera.ScaleX, h / Camera.ScaleY);
            case SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_STRETCH:
            case SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_LETTERBOX:
            case SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_INTEGER_SCALE:
                return new Rect((-(w / 2) / Camera.ScaleX) + Camera.X, (-(h / 2) / Camera.ScaleY) + Camera.Y, w / Camera.ScaleX, h / Camera.ScaleY);
            case SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_OVERSCAN:
                (int ww, int wh) = app.Window.WindowSize;
                float presentAspectRatio = w / (float)h;
                float windowAspectRatio = ww / (float)wh;
                if (MathF.Abs(presentAspectRatio - windowAspectRatio) < 0.01f) {
                    // almost same aspect ratio - prob equals stretch
                    return new Rect((-(w / 2) / Camera.ScaleX) + Camera.X, (-(h / 2) / Camera.ScaleY) + Camera.Y, w / Camera.ScaleX, h / Camera.ScaleY);
                } else if (presentAspectRatio < windowAspectRatio) {
                    // present width is preserved
                    float actualPresentHeight = (float)Math.Round(w / windowAspectRatio);
                    return new Rect((-(w / 2) / Camera.ScaleX) + Camera.X, (-(actualPresentHeight / 2) / Camera.ScaleY) + Camera.Y, w / Camera.ScaleX, actualPresentHeight / Camera.ScaleY);
                } else if (presentAspectRatio > windowAspectRatio) {
                    // present height is preserved
                    float actualPresentWidth = (float)Math.Round(h * windowAspectRatio);
                    return new Rect((-(actualPresentWidth / 2) / Camera.ScaleX) + Camera.X, (-(h / 2) / Camera.ScaleY) + Camera.Y, actualPresentWidth / Camera.ScaleX, h / Camera.ScaleY);
                }
                // probably shouldn't reach here
                return new Rect(0, 0, 0, 0);
            default:
                return new(0, 0, 0, 0);
        }
    }

    public void SetViewport(int width, int height, ViewportOpts opts) {
        if (width < 1 || height < 1) { ClearViewport(); return; }
        Camera.requestedViewportWidth = width;
        Camera.requestedViewportHeight = height;
        SDL3.SDL_SetRenderLogicalPresentation(app.Window.sdlRenderer, width, height, (SDL_RendererLogicalPresentation)opts);
        Camera.UpdateActualViewportSizes();
    }

    public void ClearViewport() {
        Camera.requestedViewportWidth = 0;
        Camera.requestedViewportHeight = 0;
        SDL3.SDL_SetRenderLogicalPresentation(app.Window.sdlRenderer, 0, 0, SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_DISABLED);
        Camera.UpdateActualViewportSizes();
    }

    public void Begin() {
        isDrawing = true;

        drawCommandBuffer.Clear();

        SDL3.SDL_SetRenderDrawColorFloat(app.Window.sdlRenderer, ClearColor.r, ClearColor.g, ClearColor.b, ClearColor.a);
        SDL3.SDL_RenderClear(app.Window.sdlRenderer);
        SDL3.SDL_SetRenderDrawColorFloat(app.Window.sdlRenderer, 1f, 1f, 1f, 1f);
    }

    public void Draw(Texture texture, float x, float y) => Draw(texture, x, y, texture.Width, texture.Height, 0f, 0f, 0f, 0f, 0f, texture.Width, texture.Height);
    public void Draw(Texture texture, float x, float y, float w, float h) => Draw(texture, x, y, w, h, 0f, 0f, 0f, 0f, 0f, texture.Width, texture.Height);
    public void Draw(Texture texture, float x, float y, float w, float h, float rotation) => Draw(texture, x, y, w, h, rotation, w / 2f, h / 2f, 0f, 0f, texture.Width, texture.Height);
    public void Draw(Texture texture, float x, float y, float w, float h, float rotation, float pivotX, float pivotY) => Draw(texture, x, y, w, h, rotation, pivotX, pivotY, 0f, 0f, texture.Width, texture.Height);
    
    public void Draw(Texture texture, float x, float y, float w, float h, float rotation, float pivotX, float pivotY, float srcX, float srcY, float srcW, float srcH, Flip flip = Flip.None) {
        if (!isDrawing) throw new InvalidOperationException("Draw calls must be placed inbetween Begin and End calls");

        Vector2[] src = {
            new Vector2(x,     y),
            new Vector2(x + w, y),
            new Vector2(x,     y + h),
            new Vector2(x + w, y + h),
        };

        float worldPivotX = x + pivotX;
        float worldPivotY = y + pivotY;

        Vector2[] dst = new Vector2[4];

        // local transform
        Matrix3x2 localTransform =
            Matrix3x2.CreateTranslation(-worldPivotX, -worldPivotY) *                                        // move pivot to origin
            Matrix3x2.CreateRotation(rotation * RadToDeg) *                                                  // apply local rotation
            Matrix3x2.CreateTranslation(worldPivotX, worldPivotY);                                           // revert

        // world transform
        Matrix3x2 worldTransform =
            Matrix3x2.CreateTranslation(-Camera.X, -Camera.Y) *                                              // apply camera position
            Matrix3x2.CreateRotation(-Camera.Rotation * RadToDeg) *                                          // apply world rotation
            Matrix3x2.CreateScale(Camera.ScaleX, Camera.ScaleY) *                                            // apply camera scale
            Matrix3x2.CreateTranslation(Camera.actualViewportWidth / 2f, Camera.actualViewportHeight / 2f);  // center viewport

        for (int i = 0; i < 4; i++) {
            dst[i] = Vector2.Transform(src[i], localTransform * worldTransform);
        }

        float uvTLrx, uvTLry, uvTRrx, uvTRry, uvBLrx, uvBLry, uvBRrx, uvBRry;

        if (srcX == 0 && srcY == 0 && srcW == texture.Width && srcH == texture.Height) {
            uvTLrx = 0f;
            uvTLry = 0f;
            uvTRrx = 1f;
            uvTRry = 0f;
            uvBLrx = 0f;
            uvBLry = 1f;
            uvBRrx = 1f;
            uvBRry = 1f;
        } else {
            uvTLrx =  srcX         * texture.invW;
            uvTLry =  srcY         * texture.invH;
            uvTRrx = (srcX + srcW) * texture.invW;
            uvTRry =  srcY         * texture.invH;
            uvBLrx =  srcX         * texture.invW;
            uvBLry = (srcY + srcH) * texture.invH;
            uvBRrx = (srcX + srcW) * texture.invW;
            uvBRry = (srcY + srcH) * texture.invH;
        }

        float uvTLx, uvTLy, uvTRx, uvTRy, uvBLx, uvBLy, uvBRx, uvBRy;
        switch (flip) {
            case Flip.Horizontal: uvTLx = uvTRrx; uvTLy = uvTRry; uvTRx = uvTLrx; uvTRy = uvTLry; uvBLx = uvBRrx; uvBLy = uvBRry; uvBRx = uvBLrx; uvBRy = uvBLry; break;
            case Flip.Vertical:   uvTLx = uvBLrx; uvTLy = uvBLry; uvTRx = uvBRrx; uvTRy = uvBRry; uvBLx = uvTLrx; uvBLy = uvTLry; uvBRx = uvTRrx; uvBRy = uvTRry; break;
            case Flip.Both:       uvTLx = uvBRrx; uvTLy = uvBRry; uvTRx = uvBLrx; uvTRy = uvBLry; uvBLx = uvTRrx; uvBLy = uvTRry; uvBRx = uvTLrx; uvBRy = uvTLry; break;
            default:              uvTLx = uvTLrx; uvTLy = uvTLry; uvTRx = uvTRrx; uvTRy = uvTRry; uvBLx = uvBLrx; uvBLy = uvBLry; uvBRx = uvBRrx; uvBRy = uvBRry; break;
        }

        var dc = new DrawCommand() {
            Texture = texture.texture,
            Vertices = [
                new() {
                    position = new() { x = dst[0].X, y = dst[0].Y },
                    color = RenderColor,
                    tex_coord = new() { x = uvTLx, y = uvTLy }
                },
                new() {
                    position = new() { x = dst[1].X, y = dst[1].Y },
                    color = RenderColor,
                    tex_coord = new() { x = uvTRx, y = uvTRy }
                },
                new() {
                    position = new() { x = dst[2].X, y = dst[2].Y },
                    color = RenderColor,
                    tex_coord = new() { x = uvBLx, y = uvBLy }
                },
                new() {
                    position = new() { x = dst[3].X, y = dst[3].Y },
                    color = RenderColor,
                    tex_coord = new() { x = uvBRx, y = uvBRy }
                },
            ],
        };

        drawCommandBuffer.Add(dc);
    }

    internal void DrawRawTexture(SDL_Texture* texture, float x, float y, float w, float h, float rotation, float pivotX, float pivotY, float srcX, float srcY, float srcW, float srcH, Flip flip = Flip.None) {
        if (!isDrawing) throw new InvalidOperationException("Draw calls must be placed inbetween Begin and End calls");

        float tw = 0f; float th = 0f;
        SDL3.SDL_GetTextureSize(texture, &tw, &th);

        Vector2[] src = {
            new Vector2(x,     y),
            new Vector2(x + w, y),
            new Vector2(x,     y + h),
            new Vector2(x + w, y + h),
        };

        float worldPivotX = x + pivotX;
        float worldPivotY = y + pivotY;

        Vector2[] dst = new Vector2[4];

        // local transform
        Matrix3x2 localTransform =
            Matrix3x2.CreateTranslation(-worldPivotX, -worldPivotY) *                                        // move pivot to origin
            Matrix3x2.CreateRotation(rotation * RadToDeg) *                                                  // apply local rotation
            Matrix3x2.CreateTranslation(worldPivotX, worldPivotY);                                           // revert

        // world transform
        Matrix3x2 worldTransform =
            Matrix3x2.CreateTranslation(-Camera.X, -Camera.Y) *                                              // apply camera position
            Matrix3x2.CreateRotation(-Camera.Rotation * RadToDeg) *                                          // apply world rotation
            Matrix3x2.CreateScale(Camera.ScaleX, Camera.ScaleY) *                                            // apply camera scale
            Matrix3x2.CreateTranslation(Camera.actualViewportWidth / 2f, Camera.actualViewportHeight / 2f);  // center viewport

        for (int i = 0; i < 4; i++) {
            dst[i] = Vector2.Transform(src[i], localTransform * worldTransform);
        }

        float uvTLrx, uvTLry, uvTRrx, uvTRry, uvBLrx, uvBLry, uvBRrx, uvBRry;

        float invW = 1f / tw;
        float invH = 1f / th;

        uvTLrx =  srcX         * invW;
        uvTLry =  srcY         * invH;
        uvTRrx = (srcX + srcW) * invW;
        uvTRry =  srcY         * invH;
        uvBLrx =  srcX         * invW;
        uvBLry = (srcY + srcH) * invH;
        uvBRrx = (srcX + srcW) * invW;
        uvBRry = (srcY + srcH) * invH;

        float uvTLx, uvTLy, uvTRx, uvTRy, uvBLx, uvBLy, uvBRx, uvBRy;
        switch (flip) {
            case Flip.Horizontal: uvTLx = uvTRrx; uvTLy = uvTRry; uvTRx = uvTLrx; uvTRy = uvTLry; uvBLx = uvBRrx; uvBLy = uvBRry; uvBRx = uvBLrx; uvBRy = uvBLry; break;
            case Flip.Vertical:   uvTLx = uvBLrx; uvTLy = uvBLry; uvTRx = uvBRrx; uvTRy = uvBRry; uvBLx = uvTLrx; uvBLy = uvTLry; uvBRx = uvTRrx; uvBRy = uvTRry; break;
            case Flip.Both:       uvTLx = uvBRrx; uvTLy = uvBRry; uvTRx = uvBLrx; uvTRy = uvBLry; uvBLx = uvTRrx; uvBLy = uvTRry; uvBRx = uvTLrx; uvBRy = uvTLry; break;
            default:              uvTLx = uvTLrx; uvTLy = uvTLry; uvTRx = uvTRrx; uvTRy = uvTRry; uvBLx = uvBLrx; uvBLy = uvBLry; uvBRx = uvBRrx; uvBRy = uvBRry; break;
        }

        var dc = new DrawCommand() {
            Texture = texture,
            Vertices = [
                new() {
                    position = new() { x = dst[0].X, y = dst[0].Y },
                    color = RenderColor,
                    tex_coord = new() { x = uvTLx, y = uvTLy }
                },
                new() {
                    position = new() { x = dst[1].X, y = dst[1].Y },
                    color = RenderColor,
                    tex_coord = new() { x = uvTRx, y = uvTRy }
                },
                new() {
                    position = new() { x = dst[2].X, y = dst[2].Y },
                    color = RenderColor,
                    tex_coord = new() { x = uvBLx, y = uvBLy }
                },
                new() {
                    position = new() { x = dst[3].X, y = dst[3].Y },
                    color = RenderColor,
                    tex_coord = new() { x = uvBRx, y = uvBRy }
                },
            ],
        };

        drawCommandBuffer.Add(dc);
    }

    public void End() {
        isDrawing = false;

        // draw everything in drawCommandBuffer
        fixed (int* indices = fixedIndices)
        foreach (var dc in drawCommandBuffer) {
            fixed (SDL_Vertex* verts = dc.Vertices)
            SDL3.SDL_RenderGeometry(app.Window.sdlRenderer, dc.Texture, verts, 4, indices, 6);
        }

        SDL3.SDL_RenderPresent(app.Window.sdlRenderer);
    }

    // since we're manually setting tex_coord in SDL_Vertex, this doesn't have to be SDL compatible
    public enum Flip {
        None,
        Horizontal,
        Vertical,
        Both,
    }

    private struct DrawCommand {
        internal required SDL_Texture* Texture { get; init; }
        internal required SDL_Vertex[] Vertices { get; init; }
    }
}