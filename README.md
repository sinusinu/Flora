# Flora
Flora is a WIP .NET 2D game development framework built atop of SDL2-CS.

---

## **Still under development!**
Flora is still in **very, very early** stage of development! It is definitely **NOT** ready for production use.

---

## About

Flora aims to be a just-enough game dev framework that does the dirty works for you - without too much restriction and over-packed features. It is heavily influenced by [libGDX](https://github.com/libgdx/libgdx/), and took a lot of concepts from it.

It is built on .NET 5 and SDL2, so while it could be cross-platform, currently only Windows is considered as a supported platform. With some modification, it will probably run on Linux or macOS, but those platforms are not officially supported for now.

## Documentation

Currently no separate docs are provided. Some are documented using [documentation comments](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments).

## How to use

Due to it being still under development, Flora is currently only provided as a source project. Providing package won't be considered until enough developments are made into Flora.

If you want to try it anyway, Please take a read on temporary Quick Start Guide below.

## Quick Start Guide (temporary)

1. ```git clone``` this repository
2. fetch submodules using ```git submodule update --init```
3. Create new .NET project somewhere else using ```dotnet new console```
4. Add Flora as reference to your project using ```dotnet add reference path\to\flora\Flora.csproj```
5. Build project once using ```dotnet build```
6. Acquire and Place [SDL2.dll](https://www.libsdl.org/download-2.0.php), [SDL_image.dll](https://www.libsdl.org/projects/SDL_image/), [SDL_mixer.dll](https://www.libsdl.org/projects/SDL_mixer/), [SDL_ttf.dll](https://www.libsdl.org/projects/SDL_ttf/) and all of its dependency DLLs into build directory (where the exe file is located)
7. Setup basic codes (See the example project for more info)

Program.cs:
```csharp
using System;
using Flora;

namespace FloraExample {
    class Program {
        static void Main(string[] args) {
            ApplicationConfiguration config = new ApplicationConfiguration();
            MyCore core = new MyCore();
            new FloraApplication(core, config);
        }
    }
}

```

MyCore.cs:
```csharp
using System;
using Flora;
using Flora.Gfx;

namespace FloraExample {
    class MyCore : ApplicationCore {
        public override void Prepare() {}
        public override void Pause() {}
        public override void Resume() {}
        public override void Resize(int width, int height) {}
        public override void Render(float delta) {
            Gfx.Begin();
            Gfx.End();
        }
        public override void Dispose() {}
    }
}
```

8. Run project using ```dotnet run```
9. (Optional) Setup Post-build event for auto-copying SDL DLLs into build folder in case of running ```dotnet clean``` or something (See ```FloraExample.csproj``` file and ```extlibs``` directory inside example project for more info)

## License

Flora is distributed under the terms of MIT License.