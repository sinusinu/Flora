using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL;

namespace Flora;

/// <summary>
/// <c>Sound</c> decodes all of file and store it on memory. Slower and memory-heavy loading, faster playing. Good for short sound effects.<br/>
/// </summary>
public unsafe class Sound : IDisposable {
    internal MIX_Audio* sdlAudio;
    internal MIX_Track* sdlTrack;
    internal SDL_PropertiesID options;

    private GCHandle selfHandle;

    private bool _playing = false;
    public bool Playing => _playing;
    private bool _paused = false;
    public bool Paused => _paused;
    /// <summary>Called when the audio playback stops.</summary>
    public Action? Stopped = null;

    internal Sound(Audio audio, string path, bool predecode = true) {
        sdlAudio = SDL3_mixer.MIX_LoadAudio(audio.sdlMixer, path, predecode);
        if (sdlAudio is null) {
            var error = SDL3.SDL_GetError();
            throw new Exception($"Failed to load audio file {path}: {error}");
        }

        sdlTrack = SDL3_mixer.MIX_CreateTrack(audio.sdlMixer);
        if (sdlTrack is null) {
            var error = SDL3.SDL_GetError();
            throw new Exception($"Failed to create a track: {error}");
        }

        options = SDL3.SDL_CreateProperties();

        SDL3_mixer.MIX_SetTrackAudio(sdlTrack, sdlAudio);

        // for callback
        selfHandle = GCHandle.Alloc(this);
        nint userdata = GCHandle.ToIntPtr(selfHandle);
        SDL3_mixer.MIX_SetTrackStoppedCallback(sdlTrack, &TrackStoppedCallback, userdata);
    }

    /// <summary>
    /// Start the audio playback.
    /// </summary>
    public void Play() {
        _playing = true;
        _paused = false;
        SDL3_mixer.MIX_PlayTrack(sdlTrack, options);
    }

    /// <summary>
    /// Pause the audio playback. Does nothing if not playing or is already paused.
    /// </summary>
    public void Pause() {
        if (!_playing || _paused) return;
        SDL3_mixer.MIX_PauseTrack(sdlTrack);
        _paused = true;
    }

    /// <summary>
    /// Resume the paused audio playback. Does nothing if not playing or is not paused.
    /// </summary>
    public void Resume() {
        if (!_playing || !_paused) return;
        SDL3_mixer.MIX_ResumeTrack(sdlTrack);
        _paused = false;
    }

    /// <summary>
    /// Stop the audio playback.
    /// </summary>
    public void Stop() {
        SDL3_mixer.MIX_StopTrack(sdlTrack, 0);
    }

    /// <summary>
    /// Total length of the audio in milliseconds.
    /// </summary>
    public long Length => SDL3_mixer.MIX_TrackFramesToMS(sdlTrack, SDL3_mixer.MIX_GetAudioDuration(sdlAudio));

    /// <summary>
    /// Current playback position of the audio in milliseconds.
    /// </summary>
    public long Position {
        get => SDL3_mixer.MIX_TrackFramesToMS(sdlTrack, SDL3_mixer.MIX_GetTrackPlaybackPosition(sdlTrack));
        set => SDL3_mixer.MIX_SetTrackPlaybackPosition(sdlTrack, SDL3_mixer.MIX_TrackMSToFrames(sdlTrack, value));
    }

    /// <summary>
    /// Volume of this audio, ranged from 0 to 1.
    /// </summary>
    public float Volume {
        set => SDL3_mixer.MIX_SetTrackGain(sdlTrack, Math.Clamp(value, 0f, 1f));
    }

    /// <summary>
    /// Playback speed of this audio, ranged from 0.01 to 100, default is 1.
    /// </summary>
    public float Speed {
        get => SDL3_mixer.MIX_GetTrackFrequencyRatio(sdlTrack);
        set => SDL3_mixer.MIX_SetTrackFrequencyRatio(sdlTrack, Math.Clamp(value, 0.01f, 100f));
    }

    private void StoppedInternal() {
        _playing = false;
        _paused = false;
        Stopped?.Invoke();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void TrackStoppedCallback(nint userdata, MIX_Track* track) {
        try {
            var handle = GCHandle.FromIntPtr(userdata);
            if (handle.Target is Sound self) self.StoppedInternal();
        } catch (Exception) {}
    }

#region Dispose
    private bool _disposed = false;

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;

        /* if (disposing) {} */

        SDL3.SDL_DestroyProperties(options);

        SDL3_mixer.MIX_DestroyTrack(sdlTrack);
        SDL3_mixer.MIX_DestroyAudio(sdlAudio);

        selfHandle.Free();

        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Sound() => Dispose(false);
#endregion Dispose
}