namespace Flora;

/// <summary>
/// <c>Music</c> stream in audio data as it plays. Faster and memory-lean loading, heavier playing. Good for background music.<br/>
/// </summary>
public class Music : Sound {
    internal Music(Audio audio, string path) : base(audio, path, false) {}
}