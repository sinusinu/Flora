using SDL2;

namespace Flora.Input;

public enum ControllerButton {
    Invalid = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_INVALID,
    A = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A,
    B = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B,
    X = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X,
    Y = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y,
    Back = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK,
    Guide = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE,
    Start = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START,
    LeftStick = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK,
    RightStick = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK,
    LeftShoulder = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER,
    RightShoulder = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER,
    DpadUp = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP,
    DpadDown = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN,
    DpadLeft = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT,
    DpadRight = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT
}