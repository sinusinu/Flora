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

        /// <summary>
        /// Check if given two rectangles intersect.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>true if intersects, false otherwise</returns>
        public static bool Intersect(Rect r1, Rect r2) {
            return r1.x < r2.x + r2.w && r1.x + r1.w > r2.x && r1.y < r2.y + r2.h && r1.y + r1.h > r2.y;
        }
    }
}