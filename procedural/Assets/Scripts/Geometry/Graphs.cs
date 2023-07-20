using System.Collections.Generic;
using UnityEngine;

public static class Graphs {

    public static Poly CreatePoly(params int[] indices) =>
        new Poly { indices = new List<int>(indices) };

    public static Graph<Quad> CreateGrid(int width, int height) {
        var mesh = new Graph<Quad>();

        for (int y = 0; y < height + 1; y++) {
            for (int x = 0; x < width + 1; x++) {
                mesh.vertices.Add(new Vertex(new Vector3(x, 0, y), new Vector2(x / (float)width, y / (float)height)));
            }
        }

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                mesh.polygons.Add(new Quad(
                    (y + 0) * (width + 1) + (x + 1),
                    (y + 0) * (width + 1) + (x + 0),
                    (y + 1) * (width + 1) + (x + 0),
                    (y + 1) * (width + 1) + (x + 1)
                ));
            }
        }

        return mesh;
    }

    public static Graph<Tri> Triangulate(this Graph<Quad> input) {
        Graph<Tri> output = new Graph<Tri>();
        output.vertices.AddRange(input.vertices);

        for (int i = 0; i < input.polygons.Count; i++) {
            ref var p = ref input.polygons[i];
            var dist02 = Vector3.Distance(input.vertices[p.i0].pos, input.vertices[p.i2].pos);
            var dist13 = Vector3.Distance(input.vertices[p.i1].pos, input.vertices[p.i3].pos);
            if (dist02 < dist13) {
                output.polygons.Add(new Tri(p.i0, p.i1, p.i2));
                output.polygons.Add(new Tri(p.i0, p.i2, p.i3));
            } else {
                output.polygons.Add(new Tri(p.i0, p.i1, p.i3));
                output.polygons.Add(new Tri(p.i1, p.i2, p.i3));
            }
        }

        return output;
    }

    public static Graph<TPoly> SplitFaces<TPoly>(this Graph<TPoly> input)
        where TPoly : struct, IStaticPolygon<TPoly> {

        var polyIndexCount = IStaticPolygon<TPoly>.polyIndexCount;
        var outIndices = new int[polyIndexCount];

        var output = new Graph<TPoly>();
        for (int k = 0; k < input.polygons.Count; k++) {
            ref var inPoly = ref input.polygons[k];

            for (int i = 0; i < polyIndexCount; i++) {
                output.vertices.Add(input.vertices[inPoly[i]]);
                outIndices[i] = (k * polyIndexCount) + i;
            }

            output.polygons.Add(IStaticPolygon<TPoly>.Create(outIndices));
        }

        return output;
    }

    public static void SetColor<TPoly>(this Graph<TPoly> graph, ref TPoly poly, Color color)
         where TPoly : struct, IStaticPolygon<TPoly> {

        var polyIndexCount = IStaticPolygon<TPoly>.polyIndexCount;

        for (int i = 0; i < polyIndexCount; i++) {
            ref var v = ref graph.vertices[poly[i]];
            v.color = color;
        }
    }

    public static Vertex SampleCenter<TPoly>(this Graph<TPoly> graph, ref TPoly poly)
       where TPoly : struct, IStaticPolygon<TPoly> {

        var polyIndexCount = IStaticPolygon<TPoly>.polyIndexCount;
        var scale = 1.0f / polyIndexCount;

        Vertex center = default;

        for (int i = 0; i < polyIndexCount; i++) {
            ref Vertex input = ref graph.vertices[poly[i]];
            center += input;
        }

        return (center * scale).normalized;
    }
}
