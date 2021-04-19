# Flora Example Project

This project contains short codes that demonstrates how to use basic functionalities of Flora.

Some required files are missing due to legal issues, so you must:
1) Place [SDL2.dll](https://www.libsdl.org/download-2.0.php), [SDL_image.dll](https://www.libsdl.org/projects/SDL_image/), [SDL_mixer.dll](https://www.libsdl.org/projects/SDL_mixer/), [SDL_ttf.dll](https://www.libsdl.org/projects/SDL_ttf/) and all of its dependency DLLs inside ```extlibs``` directory.
2) Place any ```bgm.mp3```, ```se.mp3```, ```font.ttf``` into ```res``` directory. Use anything available (for ```font.ttf```, you might want to use a font that contains Korean glyphs if you want to see the text "이것은 테스트입니다." getting printed.)

You can change which "core" to run by modifying ```Program.cs```.

```csharp
BasicCore core = new BasicCore();
//TextureCore core = new TextureCore();
//InputCore core = new InputCore();
//AudioCore core = new AudioCore();
//ViewCore core = new ViewCore();
//ControllerCore core = new ControllerCore();
//FontCore core = new FontCore();
```

Before running, build project once to copy the required files.

```
dotnet build
```

To run:

```
dotnet run
```