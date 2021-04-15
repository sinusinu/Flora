using System;
using Flora;
using Flora.Gfx;
using Flora.Util;

namespace FloraExample {
    class ViewCore : ApplicationCore {
        Texture texture;
        View view;

        public override void Prepare() {
            texture = new Texture(Path.Relative("res/480.png"));
            
            view = new View(853, 480, false);
            Gfx.SetView(view);
        }
        
        public override void Pause() {}
        
        public override void Resume() {}
        
        public override void Resize(int width, int height) {}

        public override void Render(float delta) {
            Gfx.Begin(new Color(0x00, 0x00, 0x00, 0xFF));
            Gfx.Draw(texture, -428, 0);
            Gfx.Draw(texture, 428, 0);
            Gfx.End();
        }
        
        public override void Dispose() {}
    }
}