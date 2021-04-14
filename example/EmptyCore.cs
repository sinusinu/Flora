using System;
using Flora;
using Flora.Gfx;

namespace FloraExample {
    class EmptyCore : ApplicationCore {
        public override void Prepare() {}
        public override void Pause() {}
        public override void Resume() {}
        public override void Resize(int width, int height) {}
        public override void Render(float delta) {}
        public override void Dispose() {}
    }
}