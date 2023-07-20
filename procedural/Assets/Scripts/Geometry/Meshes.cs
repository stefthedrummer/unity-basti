using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

unsafe public static class Meshes {

    public static Mesh CreateMesh(this Graph<Tri> graph) {
        Mesh mesh = new Mesh();

        mesh.vertices = graph.vertices.Select(v => v.pos).ToArray();
        mesh.normals = graph.vertices.Select(v => v.normal).ToArray();
        mesh.colors = graph.vertices.Select(v => v.color).ToArray();

        int[] indices = new int[graph.polygons.Count * 3];
        for (int i = 0; i < graph.polygons.Count; i++) {
            var polygon = graph.polygons[i];
            indices[i * 3 + 0] = polygon[0];
            indices[i * 3 + 1] = polygon[1];
            indices[i * 3 + 2] = polygon[2];
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetIndices(
            indices: indices,
            topology: MeshTopology.Triangles,
            submesh: 0
        );
        mesh.RecalculateBounds();

        return mesh;
    }

    public static Mesh CreateMesh(this Graph<Quad> graph) =>
        CreateLineMesh(graph, PolygonToLines);

    private static int[] PolygonToLines<TPoly>(TPoly input)
          where TPoly : struct, IPolygon {

        var n = IStaticPolygon<TPoly>.polyIndexCount;
        var output = new int[n * 2];

        for (int i = 0; i < n - 1; i++) {
            output[i * 2 + 0] = input[i];
            output[i * 2 + 1] = input[i + 1];
        }
        output[(n - 1) * 2 + 0] = input[n - 1];
        output[(n - 1) * 2 + 1] = input[0];

        return output;
    }

    private static Mesh CreateLineMesh<TPoly>(
        Graph<TPoly> graph,
        Func<TPoly, int[]> indices2lines)

        where TPoly : struct, IPolygon {
        Mesh mesh = new Mesh();

        mesh.vertices = graph.vertices.Select(v => v.pos).ToArray();
        mesh.normals = graph.vertices.Select(v => v.normal).ToArray();
        mesh.colors = graph.vertices.Select(v => v.color).ToArray();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetIndices(
            indices: graph.polygons.SelectMany(indices2lines).ToArray(),
            topology: MeshTopology.Lines,
            submesh: 0
        );
        mesh.RecalculateBounds();

        return mesh;
    }
}