using System;
using SDL2;

namespace Flora {
    public class FloraApplication {
        public FloraApplication(ApplicationCore app, ApplicationConfiguration config) {
            // init SDL and friends
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG);
            
            // create window
            var window = SDL.SDL_CreateWindow(config.windowTitle, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, config.width, config.height, (SDL.SDL_WindowFlags)config.windowFlags);
            if (window == IntPtr.Zero) {
                string error = SDL.SDL_GetError();
                SDL.SDL_Quit();
                throw new Exception("Failed to initialize Flora: SDL_CreateWindow returned NULL (" + error + ")");
            }

            // create renderer
            var renderer = SDL.SDL_CreateRenderer(window, -1, (SDL.SDL_RendererFlags)config.renderFlags);
            if (renderer == IntPtr.Zero) {
                string error = SDL.SDL_GetError();
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_Quit();
                throw new Exception("Failed to initialize Flora: SDL_CreateRenderer returned NULL (" + error + ")");
            }

            // init flora gfx
            Flora.Gfx.Gfx.Init(window, renderer);

            // call core prepare
            app.Prepare();

            // main loop stuff
            SDL.SDL_Event e;
            bool run = true;
            
            // delta calculating stuff
            ulong freq = SDL.SDL_GetPerformanceFrequency();
            ulong last, now = SDL.SDL_GetPerformanceCounter();
            float delta = 0f;

            // SDL main loop
            while (run) {
                // do events
                while (SDL.SDL_PollEvent(out e) != 0) {
                    switch (e.type) {
                        case SDL.SDL_EventType.SDL_QUIT:
                            run = false;
                            break;
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            if (Flora.Input.Input.handler != null) Flora.Input.Input.handler.OnKeyDown((int)e.key.keysym.scancode);
                            break;
                        case SDL.SDL_EventType.SDL_KEYUP:
                            if (Flora.Input.Input.handler != null) Flora.Input.Input.handler.OnKeyUp((int)e.key.keysym.scancode);
                            break;
                        case SDL.SDL_EventType.SDL_WINDOWEVENT:
                            switch (e.window.windowEvent) {
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                                    app.Resume();
                                    break;
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                                    app.Pause();
                                    break;
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                                    app.Resize(e.window.data1, e.window.data2);
                                    break;
                            }
                            break;
                    }
                }

                // calculate delta
                last = now;
                now = SDL.SDL_GetPerformanceCounter();
                delta = (float)(((now - last) * 1000) / (float)freq);

                // call core render
                app.Render(delta);
            }

            // call core dispose
            app.Dispose();

            // cleanup things
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);

            // cleanup SDL and friends
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }
    }
}
