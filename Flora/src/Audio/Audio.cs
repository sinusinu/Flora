using SDL;

namespace Flora;

public sealed unsafe class Audio {
    private const SDL_AudioDeviceID SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK = (SDL_AudioDeviceID)0xFFFFFFFFu;

    private Application app;

    internal MIX_Mixer* sdlMixer = null;

    private bool initialized = false;

    internal Audio(Application app) {
        this.app = app;
    }

    internal void Prepare() {
        if (initialized) return;

        if (!SDL3_mixer.MIX_Init()) {
            throw new Exception("Failed to initialize SDL_mixer");
        }

        sdlMixer = SDL3_mixer.MIX_CreateMixerDevice(SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK, null);
        if (sdlMixer == null) {
            throw new Exception("Failed to create mixer device");
        }

        initialized = true;
    }

    internal void Cleanup() {
        if (!initialized) return;

        sdlMixer = null;

        // mixers will be destroyed with this call
        SDL3_mixer.MIX_Quit();

        initialized = false;
    }

    /// <summary>
    /// Global volume applied to all sounds and music.
    /// </summary>
    public float MasterVolume {
        get => SDL3_mixer.MIX_GetMixerGain(sdlMixer);
        set => SDL3_mixer.MIX_SetMixerGain(sdlMixer, Math.Clamp(value, 0f, 1f));
    }

    /// <summary>
    /// Create a <c>Sound</c> from an audio file.<br/>
    /// Supported types are: WAV, MP3, OGG.
    /// </summary>
    public Sound CreateSound(string path) {
        return new Sound(this, path);
    }

    /// <summary>
    /// Create a <c>Music</c> from an audio file.<br/>
    /// Supported types are: WAV, MP3, OGG.
    /// </summary>
    public Music CreateMusic(string path) {
        return new Music(this, path);
    }
}