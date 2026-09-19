using SDL;

namespace Flora;

internal unsafe struct DrawCommand {
    internal required SDL_Texture* Texture { get; init; }
    internal required SDL_Vertex[] Vertices { get; init; }
    // internal required int[] Indices { get; init; } // will use fixed indices
}