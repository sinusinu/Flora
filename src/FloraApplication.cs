using System;
using SDL2;

namespace Flora;

public class FloraApplication {
    bool run = true;

    /// <summary>
    /// Start a new Flora application with given core, settings and flags.
    /// </summary>
    /// <param name="core">Instance of the class that extends ApplicationCore</param>
    /// <param name="config">ApplicationConfiguration to apply</param>
    public FloraApplication(FloraCore core, FloraConfig config) {
        core._floraApplication = this;

        // init SDL and friends
        uint sdlInitFlag = SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_GAMECONTROLLER;
        SDL.SDL_Init(sdlInitFlag);
        SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG);
        SDL_ttf.TTF_Init();
        
        // create window
        SDL.SDL_WindowFlags windowFlags = (SDL.SDL_WindowFlags)config.windowFlags;
        var window = SDL.SDL_CreateWindow(config.windowTitle, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, config.width, config.height, windowFlags);
        if (window == IntPtr.Zero) {
            string error = SDL.SDL_GetError();
            SDL_ttf.TTF_Quit();
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
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
            throw new Exception("Failed to initialize Flora: SDL_CreateRenderer returned NULL (" + error + ")");
        }

        // init flora things
        Flora.Gfx.Gfx.Init(window, renderer);
        Flora.Input.Input.Init();
        Flora.Input.Controller.Init();
        Flora.Audio.Audio.Init();

        // call core prepare
        core.Prepare();

        // main loop stuff
        SDL.SDL_Event e;
        
        // delta calculating stuff
        ulong freq = SDL.SDL_GetPerformanceFrequency();
        ulong last, now = SDL.SDL_GetPerformanceCounter();
        float delta = 0f;
        bool shouldCompensatePause = false;
        
        Input.IInputHandler h = null;
        Input.IControllerHandler ch = null;

        // SDL main loop
        while (run) {
            ch = null;
            // do events
            while (SDL.SDL_PollEvent(out e) != 0) {
                switch (e.type) {
                    case SDL.SDL_EventType.SDL_QUIT:
                        run = false;
                        // use goto to completely break out of main loop, preventing frame update happening on closing frame
                        goto Cleanup;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        if (e.key.repeat > 0) break;
                        if (Input.Input.handler.TryGetTarget(out h)) {
                            h.OnKeyDown((Input.KeyCode)e.key.keysym.scancode);
                            h = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        if (Input.Input.handler.TryGetTarget(out h)) {
                            h.OnKeyUp((Input.KeyCode)e.key.keysym.scancode);
                            h = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        if (Input.Input.handler.TryGetTarget(out h)) {
                            (int x, int y) = Gfx.Gfx.ConvertScreenPointToViewPoint(e.button.x, e.button.y);
                            h.OnMouseDown((Input.MouseButton)e.button.button, x, y);
                            h = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                        if (Input.Input.handler.TryGetTarget(out h)) {
                            (int x, int y) = Gfx.Gfx.ConvertScreenPointToViewPoint(e.button.x, e.button.y);
                            h.OnMouseUp((Input.MouseButton)e.button.button, x, y);
                            h = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEMOTION:
                        if (Input.Input.handler.TryGetTarget(out h)) {
                            (int x, int y) = Gfx.Gfx.ConvertScreenPointToViewPoint(e.button.x, e.button.y);
                            h.OnMouseMove(x, y);
                            h = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                        if (Input.Input.handler.TryGetTarget(out h)) {
                            h.OnMouseWheel(e.wheel.x, e.wheel.y);
                            h = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION:
                        if (Input.Controller.isControllerInitialized && Input.Controller.handler.TryGetTarget(out ch)) {
                            ch.OnAxisMotion(e.caxis.which, (Input.ControllerAxis)e.caxis.axis, Input.Controller.ShortToFloat(e.caxis.axisValue));
                            ch = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                        if (Input.Controller.isControllerInitialized && Input.Controller.handler.TryGetTarget(out ch)) {
                            ch.OnButtonDown(e.cbutton.which, (Input.ControllerButton)e.cbutton.button);
                            ch = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP:
                        if (Input.Controller.isControllerInitialized && Input.Controller.handler.TryGetTarget(out ch)) {
                            ch.OnButtonUp(e.cbutton.which, (Input.ControllerButton)e.cbutton.button);
                            ch = null;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                        if (Input.Controller.isControllerInitialized) {
                            // workaround for SDL_CONTROLLERDEVICEADDED event data being inaccurate
                            Input.Controller.ReadyReportNewController();
                            Input.Controller.RefreshControllers();
                            if (Input.Controller.handler.TryGetTarget(out ch)) {
                                //Input.Controller.handler.OnControllerAdded(e.cdevice.which);
                                Input.Controller.ReportNewController();
                                ch = null;
                            }
                        }
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                        if (Input.Controller.isControllerInitialized) {
                            Input.Controller.RefreshControllers();
                            if (Input.Controller.handler.TryGetTarget(out ch)) {
                                ch.OnControllerRemoved(e.cdevice.which);
                                ch = null;
                            }
                        }
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
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
                                // stall execution while window is minimized to prevent weird bugs happening
                                shouldCompensatePause = true;
                                while (SDL.SDL_WaitEvent(out e) != 0) {
                                    if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT && e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED) {
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }

            if (shouldCompensatePause) {
                // since execution gets stalled while the window is minimized,
                // the first frame after stalling will have a delta time of, like, multiples of seconds. 
                // for updating game logic, delta time of that large should be avoided.
                // so for the first frame after stalling, we just repeat the last delta.
                now = SDL.SDL_GetPerformanceCounter();
                shouldCompensatePause = false;
            } else {
                // calculate delta
                last = now;
                now = SDL.SDL_GetPerformanceCounter();
                delta = (float)(((now - last)) / (float)freq);
            }

            // call core render
            core.Render(delta);
        }

        Cleanup:

        // cleanup core
        core.Cleanup();

        // cleanup soloud
        Flora.Audio.Audio.Deinit();

        // cleanup things
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);

        // cleanup SDL and friends
        SDL_ttf.TTF_Quit();
        SDL_image.IMG_Quit();
        SDL.SDL_Quit();
    }

    internal void Exit() {
        run = false;
    }
}