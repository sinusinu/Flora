using System;
using Flora;
using Flora.Gfx;
using Flora.Util;

namespace FloraExample {
    class ViewCore : ApplicationCore {
        Texture texture;
        View view;

        float brrr;
        float viewPosX;
        float viewPosY;
        float viewZoom;

        public override void Prepare() {
            texture = new Texture(Path.Relative("res/480.png"));
            
            view = new View(853, 480, false);
            Gfx.SetView(view);
        }
        
        public override void Pause() {}
        
        public override void Resume() {}
        
        public override void Resize(int width, int height) {}

        public override void Render(float delta) {
            brrr += delta * 0.005f;
            viewPosX = MathF.Sin(brrr) * 40f;
            viewPosY = MathF.Cos(brrr) * 40f;
            viewZoom = (MathF.Sin(brrr) * 0.2f) + 0.8f;

            view.SetPosition(viewPosX, viewPosY);
            view.SetZoom(viewZoom);

            Gfx.Begin(new Color(0x00, 0x00, 0x00, 0xFF));
            
            Gfx.Draw(texture, -427, 0);
            Gfx.Draw(texture, 426, 0);
            Gfx.Draw(texture, 1279, 0);
            
            Gfx.SetColor(0, 255, 255, 255);
            Gfx.DrawRect(new Rect(-5 + 427, -5 + 240, 10, 10), true);
            Gfx.DrawRect(new Rect(10 + 427, -40 + 240, 20, 80), true);
            Gfx.DrawLine(-20 + 427, -20 + 240, 20 + 427, -20 + 240);
            Gfx.SetColor(255, 255, 255, 255);
            
            Gfx.End();
        }
        
        public override void Dispose() {}
    }
}