using SDL2;

namespace Flora.Gfx;

public enum FontHinting : int {
    Normal = SDL_ttf.TTF_HINTING_NORMAL,
    Light = SDL_ttf.TTF_HINTING_LIGHT,
    Mono = SDL_ttf.TTF_HINTING_MONO,
    None = SDL_ttf.TTF_HINTING_NONE
}