using SDL;

namespace Flora;

/// <summary>
/// Camera that can move, rotate, scale.
/// </summary>
public class Camera {
    private Application app;
    private Stack<CameraState> stateStack = new();

    internal Camera(Application app) {
        this.app = app;
    }

    public float X { get; set; } = 0f;
    public float Y { get; set; } = 0f;
    public float Rotation { get; set; } = 0f;
    public float ScaleX { get; set; } = 1f;
    public float ScaleY { get; set; } = 1f;

    /// <summary>
    /// Push current state of camera into stack.
    /// </summary>
    public void PushState() {
        stateStack.Push(new(X, Y, Rotation, ScaleX, ScaleY));
    }

    /// <summary>
    /// Pop a camera state from stack and apply.
    /// </summary>
    public void PopState() {
        if (stateStack.Count == 0) return;
        var lastState = stateStack.Pop();
        X = lastState.x;
        Y = lastState.y;
        Rotation = lastState.rotation;
        ScaleX = lastState.scaleX;
        ScaleY = lastState.scaleY;
    }

    /// <summary>
    /// Reset current camera state.
    /// </summary>
    public void Reset() {
        X = 0f;
        Y = 0f;
        Rotation = 0f;
        ScaleX = 1f;
        ScaleY = 1f;
    }

    internal int requestedViewportWidth = 0;
    internal int requestedViewportHeight = 0;
    internal int actualViewportWidth = 0;
    internal int actualViewportHeight = 0;

    internal unsafe void UpdateActualViewportSizes() {
        if (requestedViewportWidth == 0 || requestedViewportHeight == 0) {
            // no viewport is set, fetch from SDL3
            int nx = 0; int ny = 0;
            SDL3.SDL_GetRenderOutputSize(app.Window.sdlRenderer, &nx, &ny);
            actualViewportWidth = nx;
            actualViewportHeight = ny;
        } else {
            // viewport is set, use viewport sizes
            actualViewportWidth = requestedViewportWidth;
            actualViewportHeight = requestedViewportHeight;
        }
    }

    private record CameraState(float x, float y, float rotation, float scaleX, float scaleY);
}