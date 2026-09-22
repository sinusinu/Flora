using SDL;

namespace Flora;

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

    public static implicit operator SDL_Rect(Rect rect) => new SDL_Rect() {
        x = (int)rect.x,
        y = (int)rect.y,
        w = (int)rect.w,
        h = (int)rect.h,
    };

    public static implicit operator SDL_FRect(Rect rect) => new SDL_FRect() {
        x = rect.x,
        y = rect.y,
        w = rect.w,
        h = rect.h,
    };

    public override string ToString() {
        return $"[ {x}, {y}, {w}, {h} ]";
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