using System;

namespace Flora {
    public class ApplicationCore {
        public virtual void Prepare() {}
        public virtual void Pause() {}
        public virtual void Resume() {}
        public virtual void Resize(int width, int height) {}
        public virtual void Render(float delta) {}
        public virtual void Dispose() {}
    }
}
