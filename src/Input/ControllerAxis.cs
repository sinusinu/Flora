using SDL2;

namespace Flora.Input {
    public enum ControllerAxis {
        Invalid = SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_INVALID,
        LeftX = SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX,
        LeftY = SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY,
        RightX = SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX,
        RightY = SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY,
        TriggerLeft = SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT,
        TriggerRight = SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT
    }
}