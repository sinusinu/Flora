# Flora
Flora is a WIP .NET game development framework built atop of SDL2-CS.

---

## **Still under development!**
Flora is still in **very, very early** stage of development - not even all of the planned features are still finished! Due to that, Flora is currently only provided as a project to reference. Something like package will not be available until enough developments are made into Flora.

---

## About

Flora aims to be a just-enough game dev framework that does the dirty works for you - without too much restriction and over-packed features. It is heavily influenced by [libGDX](https://github.com/libgdx/libgdx/), and took a lot of concepts from it.

It is built on .NET 5 and SDL2, so while it could be cross-platform, currently only Windows is considered as a supported platform. With some modification, it will probably run on Linux or macOS, but those platforms are not officially supported for now.

## Documentation

Currently no separate docs are provided. Some are documented using [documentation comments](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments).

## Quick Start Guide (temporary)

1. ```git clone``` this repository
2. Create new .NET project somewhere else using ```dotnet new console```
3. Add reference to Flora using ```dotnet add reference path\to\this\repo```
4. Build project once using ```dotnet build```
5. Copy SDL2.dll, SDL_image.dll, SDL_mixer.dll, SDL_ttf.dll and all of its dependency DLLs into build directory (where the exe file is located)
6. Setup basic codes (See the example project for more info)

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

7. Run project using ```dotnet run```
8. (Optional) Setup Post-build event for auto-copying SDL DLLs into build folder in case of running ```dotnet clean``` or something (See FloraExample.csproj file inside example project for more info)

## License

Flora is distributed under the terms of MIT License.