using System.Numerics;
using SDL;

namespace Flora;

public class DrawCommand {
    public required Texture Texture { get; init; }
    public required Vertex[] Vertices { get; init; }
    public required int[] Indices { get; init; }

    public struct Vertex {
        public required Vector2 position;
        public required Color color;
        public required Vector2 uvCoords;

        internal SDL_Vertex ToSDLVertex() {
            return new SDL_Vertex() {
                position = new() {
                    x = position.X,
                    y = position.Y,
                },
                color = color,
                tex_coord = new() {
                    x = uvCoords.X,
                    y = uvCoords.Y,
                }
            };
        }
    }
}