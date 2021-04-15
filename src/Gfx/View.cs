using System;
using SDL2;

namespace Flora.Gfx {
    public class View {
        internal int ratioCorrectedWidth;
        internal int ratioCorrectedHeight;
        internal int givenWidth;
        internal int givenHeight;
        internal int offsetX;
        internal int offsetY;
        internal bool letterBox;

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
                offsetX = 0;
                offsetY = 0;
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
                    offsetX = (ratioCorrectedWidth - givenWidth) / 2;
                    offsetY = 0;
                } else if (clientSizeAspectRatio + 0.005f < givenSizeAspectRatio) {
                    // client is vertically stretched
                    float ratio = clientSizeAspectRatio / givenSizeAspectRatio;
                    ratioCorrectedWidth = givenWidth;
                    ratioCorrectedHeight = (int)(givenHeight / ratio);
                    offsetX = 0;
                    offsetY = (ratioCorrectedHeight - givenHeight) / 2;
                } else {
                    // almost same aspect ratio
                    ratioCorrectedWidth = givenWidth;
                    ratioCorrectedHeight = givenHeight;
                    offsetX = 0;
                    offsetY = 0;
                }
            }
        }
    }
}