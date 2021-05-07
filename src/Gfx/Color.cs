using System;
using SDL2;

#pragma warning disable 0659

namespace Flora.Gfx {
    public class Color {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

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

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != typeof(Color)) return false;
            
            Color other = (Color)obj;
            return this.r == other.r && this.g == other.g && this.b == other.b && this.a == other.a;
        }

        internal SDL.SDL_Color ToSDLColor() {
            SDL.SDL_Color color = new SDL.SDL_Color();
            color.r = r;
            color.g = g;
            color.b = b;
            color.a = a;
            return color;
        }
    }
}

#pragma warning restore 0659