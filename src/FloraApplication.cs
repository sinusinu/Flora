using System;
using SDL2;

namespace Flora {
    public class FloraApplication {
        bool run = true;
        
        public enum FloraApplicationFlags : int {
            /// <summary>No flags.</summary>
            Normal = 0,
            /// <summary>Create OpenGL Context for advanced uses.</summary>
            CreateOpenGLContext = 1,
            /// <summary>Do not initialize controller routines.</summary>
            DisableController = 2
        }

        /// <summary>
        /// Start a new Flora application with given core, settings and flags.
        /// </summary>
        /// <param name="core">Instance of the class that extends ApplicationCore</param>
        /// <param name="config">ApplicationConfiguration to apply</param>
        public FloraApplication(ApplicationCore core, ApplicationConfiguration config, FloraApplicationFlags flags = FloraApplicationFlags.Normal) {
            core._floraApplication = this;

            // init SDL and friends
            uint sdlInitFlag = SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO;
            if ((flags & FloraApplicationFlags.DisableController) == 0) sdlInitFlag |= SDL.SDL_INIT_GAMECONTROLLER;
            SDL.SDL_Init(sdlInitFlag);
            SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG);
            SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3 | SDL_mixer.MIX_InitFlags.MIX_INIT_OGG);
            SDL_ttf.TTF_Init();

            // open mixer audio
            if (SDL_mixer.Mix_OpenAudio(SDL_mixer.MIX_DEFAULT_FREQUENCY, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) < 0) {
                string error = SDL.SDL_GetError();
                SDL_ttf.TTF_Quit();
                SDL_mixer.Mix_Quit();
                SDL_image.IMG_Quit();
                SDL.SDL_Quit();
                throw new Exception("Failed to initialize Flora: Mix_OpenAudio Failed (" + error + ")");
            }
            
            // create window
            SDL.SDL_WindowFlags windowFlags = (SDL.SDL_WindowFlags)config.windowFlags;
            if ((flags & FloraApplicationFlags.CreateOpenGLContext) > 0) windowFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;
            var window = SDL.SDL_CreateWindow(config.windowTitle, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, config.width, config.height, windowFlags);
            if (window == IntPtr.Zero) {
                string error = SDL.SDL_GetError();
                SDL_ttf.TTF_Quit();
                SDL_mixer.Mix_Quit();
                SDL_image.IMG_Quit();
                SDL.SDL_Quit();
                throw new Exception("Failed to initialize Flora: SDL_CreateWindow returned NULL (" + error + ")");
            }

            // create renderer
            SDL.SDL_RendererFlags rendererFlags = (SDL.SDL_RendererFlags)config.renderFlags | SDL.SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE;
            var renderer = SDL.SDL_CreateRenderer(window, -1, rendererFlags);
            if (renderer == IntPtr.Zero) {
                string error = SDL.SDL_GetError();
                SDL.SDL_DestroyWindow(window);
                SDL_ttf.TTF_Quit();
                SDL_mixer.Mix_Quit();
                SDL_image.IMG_Quit();
                SDL.SDL_Quit();
                throw new Exception("Failed to initialize Flora: SDL_CreateRenderer returned NULL (" + error + ")");
            }

            // init flora things
            Flora.Gfx.Gfx.Init(window, renderer);
            Flora.Input.Input.Init();
            if ((flags & FloraApplicationFlags.DisableController) > 0) Flora.Input.Controller.Init();
            Flora.Audio.Audio.Init();

            // call core prepare
            core.Prepare();

            // main loop stuff
            SDL.SDL_Event e;
            
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
                            // use goto to completely break out of main loop, preventing frame update happening on closing frame
                            goto Cleanup;
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
                        case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                            if (Input.Input.handler != null) Input.Input.handler.OnMouseWheel(e.wheel.x, e.wheel.y);
                            break;
                        case SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION:
                            if (Input.Controller.isControllerInitialized && Input.Controller.handler != null) Input.Controller.handler.OnAxisMotion(e.caxis.which, (Input.ControllerAxis)e.caxis.axis, Input.Controller.ShortToFloat(e.caxis.axisValue));
                            break;
                        case SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                            if (Input.Controller.isControllerInitialized && Input.Controller.handler != null) Input.Controller.handler.OnButtonDown(e.cbutton.which, (Input.ControllerButton)e.cbutton.button);
                            break;
                        case SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP:
                            if (Input.Controller.isControllerInitialized && Input.Controller.handler != null) Input.Controller.handler.OnButtonUp(e.cbutton.which, (Input.ControllerButton)e.cbutton.button);
                            break;
                        case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                        case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                            if (Input.Controller.isControllerInitialized) Input.Controller.RefreshControllers();
                            break;
                        case SDL.SDL_EventType.SDL_WINDOWEVENT:
                            switch (e.window.windowEvent) {
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                                    core.Resume();
                                    break;
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                                    core.Pause();
                                    break;
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                                    Gfx.Gfx.UpdateView();
                                    core.Resize(e.window.data1, e.window.data2);
                                    break;
                            }
                            break;
                    }
                }

                // calculate delta
                last = now;
                now = SDL.SDL_GetPerformanceCounter();
                delta = (float)(((now - last)) / (float)freq);

                // call core render
                core.Render(delta);
            }

            Cleanup:

            // cleanup things
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);

            // cleanup SDL and friends
            SDL_ttf.TTF_Quit();
            SDL_mixer.Mix_Quit();
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }

        internal void Exit() {
            run = false;
        }
    }
}
