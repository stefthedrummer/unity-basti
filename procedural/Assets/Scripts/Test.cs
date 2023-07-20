using System.Linq;
using UnityEngine;
using Unity.Mathematics;

[ExecuteAlways]
public class Test : MonoBehaviour {

    public Mesh mesh;

    private void OnEnable() {
        Graph<Quad> grid = Graphs.CreateGrid(128, 128);
        {
            for (int i = 0; i < grid.vertices.Count; i++) {
                ref Vertex v = ref grid.vertices[i];

                v.pos += Hash.Float2(v.pos.XZ()).ToXZ() * 0.2f;

                var cell = Voronoi.Sample((float2)v.uv * 16);
                var biomDistord = Distord.Sample((float2)v.uv * 16);
                var biom = Voronoi.Sample((cell.cellPoint + biomDistord) * 0.125f);

                var biomColor = ColorMap.Map(Hash.Float(biom.cellPoint));
                var cellColor = math.abs(Hash.Float(cell.cellPoint));

                v.color = biomColor * cellColor;
            }
        }


        Graph<Tri> terrain = grid.Triangulate().SplitFaces();
        {
            for (int k = 0; k < terrain.polygons.Count; k++) {
                ref Tri poly = ref terrain.polygons[k];
                var center = terrain.SampleCenter(ref poly);
                terrain.SetColor(ref poly, center.color);
            }
        }

        GetComponent<MeshFilter>().sharedMesh = terrain.CreateMesh();
    }

    void Update() {
    }
}
