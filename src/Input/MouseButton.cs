using SDL2;

namespace Flora.Input;

public enum MouseButton : uint {
    Left = SDL.SDL_BUTTON_LEFT,
    Middle = SDL.SDL_BUTTON_MIDDLE,
    Right = SDL.SDL_BUTTON_RIGHT,
    Aux1 = SDL.SDL_BUTTON_X1,
    Aux2 = SDL.SDL_BUTTON_X2
}