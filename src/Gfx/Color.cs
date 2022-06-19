using System;
using SDL2;

namespace Flora.Gfx;

public class Color : IEquatable<Color> {
    public byte r;
    public byte g;
    public byte b;
    public byte a;

    public Color(Color originalColor) {
        this.r = originalColor.r;
        this.g = originalColor.g;
        this.b = originalColor.b;
        this.a = originalColor.a;
    }

    public Color(byte r, byte g, byte b, byte a) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Color(int r, int g, int b, int a) {
        this.r = (byte)r;
        this.g = (byte)g;
        this.b = (byte)b;
        this.a = (byte)a;
    }

    public Color(float r, float g, float b, float a) {
        r = Math.Clamp(r, 0f, 1f);
        g = Math.Clamp(g, 0f, 1f);
        b = Math.Clamp(b, 0f, 1f);
        a = Math.Clamp(a, 0f, 1f);
        this.r = (byte)MathF.Floor(r * 255);
        this.g = (byte)MathF.Floor(g * 255);
        this.b = (byte)MathF.Floor(b * 255);
        this.a = (byte)MathF.Floor(a * 255);
    }

    internal SDL.SDL_Color ToSDLColor() {
        SDL.SDL_Color color = new SDL.SDL_Color();
        color.r = r;
        color.g = g;
        color.b = b;
        color.a = a;
        return color;
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

    public override bool Equals(object obj) {
        return Equals(obj as Color);
    }

    public bool Equals(Color obj) {
        if (ReferenceEquals(obj, null)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return this.r == obj.r && this.g == obj.g && this.b == obj.b && this.a == obj.a;
    }

    public override int GetHashCode() {
        return (r << 24) + (g << 16) + (b << 8) + a;
    }
}
