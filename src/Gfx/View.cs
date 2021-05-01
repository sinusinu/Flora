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

        internal float _zoom = 1f;

        public float positionX { get { return offsetX; } set { offsetX = value; if (Gfx.activeView == this) Gfx.activeViewOffsetX = (int)offsetX; } }
        public float positionY { get { return offsetY; } set { offsetY = value; if (Gfx.activeView == this) Gfx.activeViewOffsetY = (int)offsetY; } }
        public float zoom { get { return _zoom; } set { _zoom = value; if (Gfx.activeView == this) Gfx.activeViewZoom = _zoom; } }

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
                centerX = givenWidth / 2;
                centerY = givenHeight / 2;
            } else {
                var (clientWidth, clientHeight) = Gfx.GetClientSize();

                float givenSizeAspectRatio = givenWidth / (float)givenHeight;
                float clientSizeAspectRatio = clientWidth / (float)clientHeight;

                if (clientSizeAspectRatio - 0.005f > givenSizeAspectRatio) {
                    // client is horizontally stretched
                    float ratio = givenSizeAspectRatio / clientSizeAspectRatio;
                    ratioCorrectedWidth = (int)(givenWidth / ratio);
                    ratioCorrectedHeight = givenHeight;
                    centerX = ((ratioCorrectedWidth - givenWidth) / 2) + (givenWidth / 2);
                    centerY = givenHeight / 2;
                } else if (clientSizeAspectRatio + 0.005f < givenSizeAspectRatio) {
                    // client is vertically stretched
                    float ratio = clientSizeAspectRatio / givenSizeAspectRatio;
                    ratioCorrectedWidth = givenWidth;
                    ratioCorrectedHeight = (int)(givenHeight / ratio);
                    centerX = givenWidth / 2;
                    centerY = ((ratioCorrectedHeight - givenHeight) / 2) + (givenHeight / 2);
                } else {
                    // almost same aspect ratio
                    ratioCorrectedWidth = givenWidth;
                    ratioCorrectedHeight = givenHeight;
                    centerX = givenWidth / 2;
                    centerY = givenHeight / 2;
                }
            }
        }
    }
}