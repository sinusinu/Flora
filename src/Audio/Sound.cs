using System;
using SDL2;
using SoLoud;

namespace Flora.Audio;

/// <summary>
/// Audio instance that suit better for playing fire-and-forget type of short sound effects.
/// </summary>
public class Sound : IDisposable {
    internal Wav wav;

    private float _volume = 1f;
    public float Volume {
        get { return _volume; }
        set { 
            _volume = Math.Clamp(value, 0f, 1f);
            wav.setVolume(_volume);
        }
    }

    /// <summary>
    /// Create a new sound.<br/>
    /// Sound is better suited for playing fire-and-forget type of short sound clips.
    /// </summary>
    /// <param name="path">Path to the sound file. WAV/MP3/OGG are supported.</param>
    /// <param name="volume">Volume of the sound. must be between 0 to 1.</param>
    /// <returns></returns>
    public Sound(string path, float volume = 1f) {
        if (!Audio.isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");

        wav = new Wav();
        wav.load(path);
        this.Volume = volume;
    }

    /// <summary>
    /// Play the sound.
    /// </summary>
    /// <param name="singleton">If true, any of this sound playing will be halted before playing.</param>
    public void Play(bool singleton = false) {
        if (singleton) Audio.soloud.stopAudioSource(wav);
        Audio.soloud.play(wav, _volume);
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;

        /* if (disposing) {} */
        
        wav.Dispose();

        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Sound() => Dispose(false);
}