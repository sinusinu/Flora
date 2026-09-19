namespace Flora;

public class Color : IEquatable<Color> {
    public float r;
    public float g;
    public float b;
    public float a;

    public Color(Color originalColor) {
        r = originalColor.r;
        g = originalColor.g;
        b = originalColor.b;
        a = originalColor.a;
    }

    public Color(float r, float g, float b, float a) {
        r = Math.Clamp(r, 0f, 1f);
        g = Math.Clamp(g, 0f, 1f);
        b = Math.Clamp(b, 0f, 1f);
        a = Math.Clamp(a, 0f, 1f);
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Color(byte r, byte g, byte b, byte a) {
        this.r = r / 255f;
        this.g = g / 255f;
        this.b = b / 255f;
        this.a = a / 255f;
    }

    public static implicit operator SDL.SDL_Color(Color color) {
        byte br = (byte)MathF.Floor(color.r * 255);
        byte bg = (byte)MathF.Floor(color.g * 255);
        byte bb = (byte)MathF.Floor(color.b * 255);
        byte ba = (byte)MathF.Floor(color.a * 255);
        return new SDL.SDL_Color {
            r = br,
            g = bg,
            b = bb,
            a = ba,
        };
    }

    public static implicit operator SDL.SDL_FColor(Color color) {
        return new SDL.SDL_FColor {
            r = color.r,
            g = color.g,
            b = color.b,
            a = color.a,
        };
    }

    public static implicit operator Color(SDL.SDL_Color color) {
        return new Color(color.r / 255f, color.g / 255f, color.b / 255f, color.a / 255f);
    }

    public static implicit operator Color(SDL.SDL_FColor color) {
        return new Color(color.r, color.g, color.b, color.a);
    }

    public static bool operator ==(Color obj1, Color obj2) {
        if (ReferenceEquals(obj1, obj2)) return true;
        if (ReferenceEquals(obj1, null)) return false;
        if (ReferenceEquals(obj2, null)) return false;
        return obj1.Equals(obj2);
    }

    public static bool operator !=(Color obj1, Color obj2) {
        return !(obj1 == obj2);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        return Equals(obj as Color);
    }

    public bool Equals(Color? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(obj, null)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return r == obj.r && g == obj.g && b == obj.b && a == obj.a;
    }

    public override int GetHashCode() {
        return (((byte)MathF.Floor(r * 255)) << 24) +
               (((byte)MathF.Floor(g * 255)) << 16) +
               (((byte)MathF.Floor(b * 255)) << 8) +
               ((byte)MathF.Floor(a * 255));
    }
}