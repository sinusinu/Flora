using System;
using Flora.Util;

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
        internal float _rotation = 0;

        public float positionX {
            get { return offsetX; }
            set {
                float diff = value - offsetX;
                offsetX += diff * MathF.Cos(MathUtils.DegToRad(_rotation));
                offsetY += diff * -MathF.Sin(MathUtils.DegToRad(_rotation));
                if (Gfx.activeView == this)
                    Gfx.activeViewOffsetX = offsetX;
                    Gfx.activeViewOffsetY = offsetY;
            }
        }
        
        public float positionY {
            get { return offsetY; }
            set {
                float diff = value - offsetY;
                offsetX += diff * MathF.Sin(MathUtils.DegToRad(_rotation));
                offsetY += diff * MathF.Cos(MathUtils.DegToRad(_rotation));
                if (Gfx.activeView == this)
                    Gfx.activeViewOffsetX = offsetX;
                    Gfx.activeViewOffsetY = offsetY;
            }
        }
        
        public float zoom {
            get { return _zoom; }
            set {
                if (value < 0f) value = 0f;
                _zoom = value;
                if (Gfx.activeView == this) {
                    Gfx.activeViewOffsetX = offsetX;
                    Gfx.activeViewOffsetY = offsetY;
                    Gfx.activeViewZoom = _zoom;
                }
            }
        }

        public float rotation {
            get { return _rotation; }
            set { _rotation = value % 360f; if (Gfx.activeView == this) Gfx.activeViewRotation = _rotation; }
        }

        public float visibleTop { get; private set; }
        public float visibleLeft { get; private set; }
        public float visibleWidth { get; private set; }
        public float visibleHeight { get; private set; }

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
                
                visibleTop = 0;
                visibleLeft = 0;
                visibleWidth = givenWidth;
                visibleHeight = givenHeight;
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

                visibleTop = (givenHeight / 2) - centerY;
                visibleLeft = (givenWidth / 2) - centerX;
                visibleWidth = ratioCorrectedWidth;
                visibleHeight = ratioCorrectedHeight;
            }
        }
    }
}