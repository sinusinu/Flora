using System;
using SDL2;

namespace Flora {
    public class FloraApplication {
        /// <summary>
        /// Start a new Flora application with given core and settings.
        /// </summary>
        /// <param name="app">Instance of the class that extends ApplicationCore</param>
        /// <param name="config">ApplicationConfiguration to apply</param>
        public FloraApplication(ApplicationCore app, ApplicationConfiguration config) {
            // init SDL and friends
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO);
            SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG);
            SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3 | SDL_mixer.MIX_InitFlags.MIX_INIT_OGG);

            // open mixer audio
            if (SDL_mixer.Mix_OpenAudio(SDL_mixer.MIX_DEFAULT_FREQUENCY, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) < 0) {
                string error = SDL.SDL_GetError();
                SDL_mixer.Mix_Quit();
                SDL_image.IMG_Quit();
                SDL.SDL_Quit();
                throw new Exception("Failed to initialize Flora: Mix_OpenAudio Failed (" + error + ")");
            }
            
            // create window
            var window = SDL.SDL_CreateWindow(config.windowTitle, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, config.width, config.height, (SDL.SDL_WindowFlags)config.windowFlags);
            if (window == IntPtr.Zero) {
                string error = SDL.SDL_GetError();
                SDL_mixer.Mix_Quit();
                SDL_image.IMG_Quit();
                SDL.SDL_Quit();
                throw new Exception("Failed to initialize Flora: SDL_CreateWindow returned NULL (" + error + ")");
            }

            // create renderer
            var renderer = SDL.SDL_CreateRenderer(window, -1, (SDL.SDL_RendererFlags)config.renderFlags);
            if (renderer == IntPtr.Zero) {
                string error = SDL.SDL_GetError();
                SDL.SDL_DestroyWindow(window);
                SDL_mixer.Mix_Quit();
                SDL_image.IMG_Quit();
                SDL.SDL_Quit();
                throw new Exception("Failed to initialize Flora: SDL_CreateRenderer returned NULL (" + error + ")");
            }

            // init flora things
            Flora.Gfx.Gfx.Init(window, renderer);
            Flora.Input.Input.Init();
            Flora.Audio.Audio.Init();

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
                            if (e.key.repeat > 0) break;
                            if (Input.Input.handler != null) Input.Input.handler.OnKeyDown((Input.KeyCode)e.key.keysym.scancode);
                            break;
                        case SDL.SDL_EventType.SDL_KEYUP:
                            if (Input.Input.handler != null) Input.Input.handler.OnKeyUp((Input.KeyCode)e.key.keysym.scancode);
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            if (Input.Input.handler != null) Input.Input.handler.OnMouseDown((Input.MouseButton)e.button.button, e.button.x, e.button.y);
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                            if (Input.Input.handler != null) Input.Input.handler.OnMouseUp((Input.MouseButton)e.button.button, e.button.x, e.button.y);
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
                            if (Input.Input.handler != null) Input.Input.handler.OnMouseMove(e.motion.x, e.motion.y);
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
                                    Gfx.Gfx.UpdateView();
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
            SDL_mixer.Mix_Quit();
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }
    }
}
