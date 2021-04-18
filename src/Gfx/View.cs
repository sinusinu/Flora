using System;
using SDL2;

namespace Flora.Gfx {
    /// <summary>
    /// Utility for setting device-independant resolution.
    /// </summary>
    public class View {
        internal int ratioCorrectedWidth;
        internal int ratioCorrectedHeight;
        internal int givenWidth;
        internal int givenHeight;
        internal float centerX;
        internal float centerY;
        internal float offsetX;
        internal float offsetY;
        internal bool letterBox;

        internal float zoom = 1f;

        /// <summary>
        /// Create new view with given size.
        /// </summary>
        /// <param name="width">Width of the view</param>
        /// <param name="height">Height of the view</param>
        /// <param name="letterBox">Determine if outside of given view should be obscured with letterbox</param>
        public View(int width, int height, bool letterBox) {
            if (!Gfx.isGfxInitialized) throw new InvalidOperationException("Gfx is not initailized");

            this.givenWidth = width;
            this.givenHeight = height;
            this.letterBox = letterBox;

            CalculateAppliedSize();
        }

        internal void CalculateAppliedSize() {
            if (letterBox) {
                ratioCorrectedWidth = givenWidth;
                ratioCorrectedHeight = givenHeight;
                centerX = 0;
                centerY = 0;
                return;
            } else {
                var (clientWidth, clientHeight) = Gfx.GetClientSize();

                float givenSizeAspectRatio = givenWidth / (float)givenHeight;
                float clientSizeAspectRatio = clientWidth / (float)clientHeight;

                if (clientSizeAspectRatio - 0.005f > givenSizeAspectRatio) {
                    // client is horizontally stretched
                    float ratio = givenSizeAspectRatio / clientSizeAspectRatio;
                    ratioCorrectedWidth = (int)(givenWidth / ratio);
                    ratioCorrectedHeight = givenHeight;
                    centerX = (ratioCorrectedWidth - givenWidth) / 2;
                    centerY = 0;
                } else if (clientSizeAspectRatio + 0.005f < givenSizeAspectRatio) {
                    // client is vertically stretched
                    float ratio = clientSizeAspectRatio / givenSizeAspectRatio;
                    ratioCorrectedWidth = givenWidth;
                    ratioCorrectedHeight = (int)(givenHeight / ratio);
                    centerX = 0;
                    centerY = (ratioCorrectedHeight - givenHeight) / 2;
                } else {
                    // almost same aspect ratio
                    ratioCorrectedWidth = givenWidth;
                    ratioCorrectedHeight = givenHeight;
                    centerX = 0;
                    centerY = 0;
                }
            }
        }

        public void SetPosition(float x, float y) { offsetX = x; offsetY = y; }
        public void SetPositionX(float x) { offsetX = x; }
        public void SetPositionY(float y) { offsetY = y; }
        public void SetZoom(float zoom) { this.zoom = zoom; }

        public (float, float) GetPosition() { return (offsetX, offsetY); }
        public float GetPositionX() { return offsetX; }
        public float GetPositionY() { return offsetY; }
        public float GetZoom() { return zoom; }
    }
}