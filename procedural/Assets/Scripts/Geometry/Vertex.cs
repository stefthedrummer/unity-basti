using System;
using UnityEngine;
using Unity.Mathematics;

[Serializable]
public struct Vertex {
    public Vector3 pos;
    public Vector3 normal;
    public Vector2 uv;
    public Color color;
    public float4 data;

    public Vertex(Vector3 pos, Vector2 uv) {
        this.pos = pos;
        this.normal = Vector3.up;
        this.color = Color.white;
        this.uv = uv;
        this.data = default;
    }

    public static Vertex operator +(Vertex l, Vertex r) => new Vertex {
        pos = l.pos + r.pos,
        normal = l.normal + r.normal,
        color = l.color + r.color,
        uv = l.uv + r.uv,
        data = l.data + r.data
    };

    public static Vertex operator *(Vertex l, float s) => new Vertex {
        pos = l.pos * s,
        normal = l.normal * s,
        color = l.color * s,
        uv = l.uv * s,
        data = l.data * s
    };

    public Vertex normalized => new Vertex {
        pos = this.pos,
        normal = this.normal.normalized,
        color = this.color,
        uv = this.uv,
        data = this.data
    };
}
