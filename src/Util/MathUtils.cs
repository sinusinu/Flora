using System;

namespace Flora.Util { 
    public class MathUtils {
        /// <summary>
        /// Convert Radian angle to Degree angle.
        /// </summary>
        public static float RadToDeg(float rad) { return rad * 180f / MathF.PI; }
        
        /// <summary>
        /// Convert Degree angle to Radian angle.
        /// </summary>
        public static float DegToRad(float deg) { return deg * MathF.PI / 180f; }
    }
}