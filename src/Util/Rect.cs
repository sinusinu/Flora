using SDL2;

namespace Flora.Util {
    /// <summary>
    /// It's a box!
    /// </summary>
    public class Rect {
        public int x;
        public int y;
        public int w;
        public int h;

        public Rect() : this(0, 0, 0, 0) {}

        public Rect(int x, int y, int w, int h) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        internal SDL.SDL_Rect ToSDLRect() {
            SDL.SDL_Rect rect;
            rect.x = x;
            rect.y = y;
            rect.w = w;
            rect.h = h;
            return rect;
        }
    }
}