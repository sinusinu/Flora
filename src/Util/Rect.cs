using SDL2;

namespace Flora.Util;

/// <summary>
/// It's a box!
/// </summary>
public class Rect {
    public float x;
    public float y;
    public float w;
    public float h;

    public Rect() : this(0, 0, 0, 0) {}

    public Rect(float x, float y, float w, float h) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }

    internal SDL.SDL_Rect ToSDLRect() {
        SDL.SDL_Rect rect;
        rect.x = (int)x;
        rect.y = (int)y;
        rect.w = (int)w;
        rect.h = (int)h;
        return rect;
    }

    internal SDL.SDL_FRect ToSDLFRect() {
        SDL.SDL_FRect rect;
        rect.x = x;
        rect.y = y;
        rect.w = w;
        rect.h = h;
        return rect;
    }

    /// <summary>
    /// Check if given two rectangles intersect.
    /// </summary>
    /// <returns>true if intersects, false otherwise</returns>
    public static bool Intersect(Rect r1, Rect r2) {
        return r1.x < r2.x + r2.w && r1.x + r1.w > r2.x && r1.y < r2.y + r2.h && r1.y + r1.h > r2.y;
    }

    /// <summary>
    /// Check if given rectangle contains given point.
    /// </summary>
    /// <returns></returns>
    public static bool Contains(Rect r, int x, int y) {
        return r.x < x && x < r.x + r.w && r.y < y && y < r.y + r.h;
    }
}