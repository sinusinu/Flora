using System.Runtime.InteropServices;
using System.Text;
using SDL;

namespace Flora;

public sealed unsafe class Application {
    private Core Core { get; init; }
    internal Config Config { get; init; }
    internal Graphics Gfx { get; init; }
    internal Input Input { get; init; }
    public Window Window { get; private set; } = null!;

    private bool run = false;
    private bool skipUpdate = false;

    // event pump stuff
    const int EventsPerPeep = 64;
    SDL_Event[] events = new SDL_Event[EventsPerPeep];

    // delta calculating stuff
    ulong freq = SDL3.SDL_GetPerformanceFrequency();
    ulong last, now = SDL3.SDL_GetPerformanceCounter();
    float delta;

    Dictionary<uint, nint> gamepads = new();

    bool windowMinimized = false;

    private Application(Core core, Config? config) {
        Core = core;
        Config = config ?? new Config();

        Gfx = new Graphics(this);
        Input = new Input(this);

        core.App = this;
        core.Gfx = Gfx;
        core.Input = Input;
    }

    private void Start() {
        SDL3.SDL_SetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS", "1");

        var sdlFlags = SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_AUDIO | SDL_InitFlags.SDL_INIT_GAMEPAD;
        if (!SDL3.SDL_Init(sdlFlags)) {
            throw new Exception("Failed to initialize SDL3");
        }

        if (!SDL3_ttf.TTF_Init()) {
            var error = SDL3.SDL_GetError();
            SDL3.SDL_Quit();
            throw new Exception($"Failed to initialize SDL3_ttf: {error}");
        }

        var windowFlags = SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY;
        var sdlWindow = SDL3.SDL_CreateWindow(Config.WindowTitle, Config.WindowWidth, Config.WindowHeight, windowFlags);
        if (sdlWindow == null) {
            var error = SDL3.SDL_GetError();
            SDL3_ttf.TTF_Quit();
            SDL3.SDL_Quit();
            throw new Exception($"Failed to create window: {error}");
        }

        var sdlRenderer = SDL3.SDL_CreateRenderer(sdlWindow, (Utf8String)null);
        if (sdlRenderer == null) {
            var error = SDL3.SDL_GetError();
            SDL3.SDL_DestroyWindow(sdlWindow);
            SDL3_ttf.TTF_Quit();
            SDL3.SDL_Quit();
            throw new Exception($"Failed to create renderer: {error}");
        }

        Window = new Window(this, sdlWindow, sdlRenderer);
        
        // set initial configs
        Window.SetWindowMode(Config.WindowMode, Config.WindowWidth, Config.WindowHeight);
        Gfx.SetVSync(Config.VSync);
        Gfx.Camera.UpdateActualViewportSizes();
        
        Core.Prepare();

        run = true;

        while (run) {
            PollEvents();

            if (skipUpdate) goto EndLoop;

            // calculate delta
            last = now;
            now = SDL3.SDL_GetPerformanceCounter();
            delta = (now - last) / (float)freq;

            Core.Render(delta);

        EndLoop:
            continue;
        }

        Core.Cleanup();

        // close all open gamepads
        foreach (var kv in gamepads) {
            SDL3.SDL_CloseGamepad((SDL_Gamepad*)kv.Value);
        }

        SDL3.SDL_DestroyRenderer(sdlRenderer);
        SDL3.SDL_DestroyWindow(sdlWindow);
        SDL3.SDL_Quit();
    }

    private void PollEvents() {
        SDL3.SDL_PumpEvents();
    
        int eventsRead;

        do {
            eventsRead = SDL3.SDL_PeepEvents(events, SDL_EventAction.SDL_GETEVENT, SDL_EventType.SDL_EVENT_FIRST, SDL_EventType.SDL_EVENT_LAST);
            for (int i = 0; i < eventsRead; i++) HandleEvent(events[i]);
        } while (eventsRead == EventsPerPeep);
    }

    private void HandleEvent(SDL_Event ev) {
        switch (ev.Type) {
            case SDL_EventType.SDL_EVENT_QUIT:
                Exit();
                break;
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
                if (ev.key.repeat) break;
                Core.OnKeyDown((Keycode)ev.key.key, (Scancode)ev.key.scancode);
                break;
            case SDL_EventType.SDL_EVENT_KEY_UP:
                if (ev.key.repeat) break;
                Core.OnKeyUp((Keycode)ev.key.key, (Scancode)ev.key.scancode);
                break;
            case SDL_EventType.SDL_EVENT_TEXT_INPUT:
                Core.OnTextInput(Marshal.PtrToStringUTF8((nint)ev.text.text) ?? "");
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
                Core.OnPointerDown(PointerType.Mouse, 0, ev.button.button, ev.button.x, ev.button.y);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
                Core.OnPointerUp(PointerType.Mouse, 0, ev.button.button, ev.button.x, ev.button.y);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                Core.OnPointerMove(PointerType.Mouse, 0, ev.motion.x, ev.motion.y, ev.motion.xrel, ev.motion.yrel);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
                Core.OnPointerWheel(PointerType.Mouse, 0, ev.wheel.mouse_x, ev.wheel.mouse_y, ev.wheel.x, ev.wheel.y);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
                Core.OnGamepadAxis((uint)ev.gaxis.which, (GamepadAxis)ev.gaxis.axis, ev.gaxis.value);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
                Core.OnGamepadDown((uint)ev.gbutton.which, (GamepadButton)ev.gbutton.button);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
                Core.OnGamepadUp((uint)ev.gbutton.which, (GamepadButton)ev.gbutton.button);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_ADDED:
                nint newGamepad = (nint)SDL3.SDL_OpenGamepad(ev.jdevice.which);
                gamepads.Add((uint)ev.jdevice.which, newGamepad);
                Core.OnGamepadAdded((uint)ev.jdevice.which);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_REMOVED:
                Core.OnGamepadRemoved((uint)ev.jdevice.which);
                SDL_Gamepad* closedGamepad = (SDL_Gamepad*)gamepads[(uint)ev.jdevice.which];
                SDL3.SDL_CloseGamepad(closedGamepad);
                gamepads.Remove((uint)ev.jdevice.which);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_GAINED:
                Core.Resume();
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_LOST:
                Core.Pause();
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                Gfx.Camera.UpdateActualViewportSizes();
                Core.Resize(ev.window.data1, ev.window.data2);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_MINIMIZED:
                windowMinimized = true;
                Core.Pause();
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_RESTORED:
                if (windowMinimized) {
                    Core.Resume();
                    windowMinimized = false;
                }
                break;
        }
    }

    /// <summary>
    /// Schedule the exit of the application. Note that the exit will not happen immediately.
    /// </summary>
    public void Exit() {
        run = false;
        skipUpdate = true;
    }
    
    public static void Run<T>(T core, Config? config = null) where T : Core {
        Application application = new Application(core, config);
        application.Start();
    }
}