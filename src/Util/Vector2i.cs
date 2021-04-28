using System;

namespace Flora.Util {
    /// <summary>
    /// 2D integer vector.
    /// </summary>
    public class Vector2i {
        public int x;
        public int y;

        /// <summary>
        /// Create new integer vector with value of (0, 0).
        /// </summary>
        /// <returns></returns>
        public Vector2i() : this(0, 0) {}

        /// <summary>
        /// Create new integer vector with given values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2i(int x, int y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Copy existing integer vector.
        /// </summary>
        /// <param name="original"></param>
        public Vector2i(Vector2i original) {
            this.x = original.x;
            this.y = original.y;
        }

        /// <summary>
        /// Adds vector a and b.
        /// </summary>
        /// <returns>a + b</returns>
        public static Vector2i Add(Vector2i a, Vector2i b) {
            return new Vector2i(a.x + b.x, a.y + b.y);
        }

        /// <summary>
        /// Divides vector a with b.
        /// </summary>
        /// <returns>a / b</returns>
        public static Vector2i Divide(Vector2i a, Vector2i b) {
            return new Vector2i(a.x / b.x, a.y / b.y);
        }
        
        /// <summary>
        /// Multiplies vector a with b.
        /// </summary>
        /// <returns>a * b</returns>
        public static Vector2i Multiply(Vector2i a, Vector2i b) {
            return new Vector2i(a.x * b.x, a.y * b.y);
        }

        /// <summary>
        /// Subtracts vector b from a.
        /// </summary>
        /// <returns>a - b</returns>
        public static Vector2i Subtract(Vector2i a, Vector2i b) {
            return new Vector2i(a.x - b.x, a.y - b.y);
        }
    }
}