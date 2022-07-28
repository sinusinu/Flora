using System;
using System.Reflection;
using System.Runtime.InteropServices;
using SDL2;

namespace Flora.Audio;

/// <summary>
/// Audio instance that suit better for playing long background music.
/// </summary>
public class Music : IDisposable {
    public enum MusicState { Idle, Playing, Paused }
    
    /// <summary>
    /// State of the playback of this music.
    /// </summary>
    public MusicState State { get {
        return MusicState.Idle;
    }}
    
    internal float _volume = 1f;
    public float Volume {
        get { return _volume / (float)SDL_mixer.MIX_MAX_VOLUME; }
        set {
            _volume = Math.Clamp(value, 0f, 1f);
            // TODO: do something
        }
    }

    internal bool _looping = false;
    public bool Looping {
        get { return _looping; }
        set { 
            _looping = value;
            // TODO: do something
        }
    }

    /// <summary>
    /// Create a new music.<br/>
    /// Music is better suited for playing long background music.<br/>
    /// </summary>
    /// <param name="path">Path to the sound file. WAV/MP3/OGG are supported.</param>
    /// <param name="volume">Volume of the sound. must be between 0 to 1.</param>
    /// <returns></returns>
    public Music(string path, float volume = 1f) {
        if (!Audio.isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");

        // TODO: do something
    }

    /// <summary>
    /// Play the music.<br/>
    /// If music is paused, playback will resume.<br/>
    /// If music is playing, nothing will happen.
    /// </summary>
    public void Play() {
        // TODO: do something
    }

    /// <summary>
    /// Pause the music.<br/>
    /// If music is not playing, nothing will happen.
    /// </summary>
    public void Pause() {
        // TODO: do something
    }

    /// <summary>
    /// Stop the music.<br/>
    /// If music is neither playing or paused, nothing will happen.
    /// </summary>
    public void Stop() {
        // TODO: do something
    }

    /// <summary>
    /// Set the position of this music.
    /// </summary>
    /// <param name="position">New position in seconds</param>
    public void SetPosition(float position) {
        // TODO: do something
    }

    /// <summary>
    /// Get the position of this music.
    /// </summary>
    /// <returns>Current position in seconds</returns>
    public float GetPosition() {
        // TODO: do something
        return 0;
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;

        /* if (disposing) {} */
        
        // dispose native things

        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Music() => Dispose(false);
}