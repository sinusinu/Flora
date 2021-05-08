[English](README.md)

# Flora
Flora는 현재 개발 중인 .NET 2D 게임 개발 프레임워크이며 SDL2-CS를 기반으로 제작되었습니다.

---

## **아직 개발 중!**
Flora는 아직 **개발 초기 단계**입니다! Flora를 지금 상업적 게임 개발에 사용하는 것은  **권장되지 않습니다.**

---

## 정보

Flora는 쓸데없이 복잡한 기능과 과도한 제약 없이 귀찮은 일만 적당히 도맡아 해주는 작고 단순한 게임 개발 프레임워크를 목표로 개발되고 있습니다. [libGDX](https://github.com/libgdx/libgdx/)의 영향을 많이 받았으며, 일부 요소를 차용해왔습니다.

.NET 5와 SDL2로 개발되어 이론적으로는 크로스 플랫폼 지원이 가능하나, 현재 정식으로 지원하는 플랫폼은 Windows 뿐입니다. 다소의 수정을 통해 Linux 또는 macOS에서 실행할 수도 있겠으나, 아직 공식 지원할 생각은 없습니다.

## 문서

현재 별도로 제공되는 문서는 없습니다. 일부 함수 등에는 [Documentation comments](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/language-specification/documentation-comments)로 간단한 설명이 기록되어 있습니다.

## 사용법

아직 개발 초기인 관계로 Flora는 소스 코드의 형태로만 제공하고 있습니다. 쉽게 사용할 수 있는 패키지의 제공은 Flora가 충분히 개발된 후에 고려될 예정입니다.

지금 바로 시도해보고 싶으시다면, 아래의 임시 빠른 시작 가이드를 참조해주세요.

## 빠른 시작 가이드 (임시)

1. ```git clone```으로 Flora 프로젝트를 다운로드합니다.
2. ```git submodule update --init```으로 submodule을 받아옵니다.
3. 어딘가 다른 디렉토리에 ```dotnet new console```으로 새 .NET 프로젝트를 생성합니다.
4. 새 프로젝트의 타겟 플랫폼을 x86으로 설정합니다. Flora는 x86 이외의 플랫폼을 지원하지 않습니다.
5. ```dotnet add reference path\to\flora\Flora.csproj```로 Flora를 새 프로젝트에 참조로 추가합니다.
6. ```dotnet build```로 한 번 빌드합니다.
7. [SDL2.dll](https://www.libsdl.org/download-2.0.php), [SDL_image.dll](https://www.libsdl.org/projects/SDL_image/), [SDL_ttf.dll](https://www.libsdl.org/projects/SDL_ttf/), [soloud_x86.dll](https://sol.gfxile.net/soloud/downloads.html) 및 그 의존 DLL을 모두 다운로드하여 빌드 디렉토리(exe 파일이 생성되는 곳)에 배치합니다.
8. 간단한 예제 코드를 작성합니다.

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
    }
}
```

9. ```dotnet run```으로 프로젝트를 실행합니다.
10. (선택) 프로젝트를 정리하는 등의 상황을 대비하여 빌드 후 이벤트를 통해 SDL DLL 파일들을 빌드할 때마다 자동으로 빌드 디렉토리에 복사되도록 설정합니다.

## 라이선스

Flora는 MIT 라이선스 하에 배포됩니다.