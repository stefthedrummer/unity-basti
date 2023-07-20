using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public interface IPolygon {
    public int this[int i] { get; set; }
}
public interface IStaticPolygon<TPoly> : IPolygon {
    public static int polyIndexCount;
    public static Func<int[], TPoly> Create;
}

[Serializable]
unsafe public struct Tri : IStaticPolygon<Tri> {
    public fixed int indices[3];

    static Tri() {
        IStaticPolygon<Tri>.polyIndexCount = 3;
        IStaticPolygon<Tri>.Create = indices => new Tri(indices[0], indices[1], indices[2]);
    }

    public Tri(int i0, int i1, int i2) {
        this.indices[0] = i0;
        this.indices[1] = i1;
        this.indices[2] = i2;
    }

    public int i0 => indices[0];
    public int i1 => indices[1];
    public int i2 => indices[2];

    public int this[int i] { get => indices[i]; set => indices[i] = value; }
}


[Serializable]
unsafe public struct Quad : IStaticPolygon<Quad> {
    public fixed int indices[4];

    static Quad() {
        IStaticPolygon<Quad>.polyIndexCount = 4;
        IStaticPolygon<Quad>.Create = indices => new Quad(indices[0], indices[1], indices[2], indices[3]);
    }

    public Quad(int i0, int i1, int i2, int i3) {
        this.indices[0] = i0;
        this.indices[1] = i1;
        this.indices[2] = i2;
        this.indices[3] = i3;
    }

    public int i0 => indices[0];
    public int i1 => indices[1];
    public int i2 => indices[2];
    public int i3 => indices[3];

    public int this[int i] { get => indices[i]; set => indices[i] = value; }
}

[Serializable]
public struct Poly : IPolygon {
    public List<int> indices;

    public int this[int i] { get => indices[i]; set => indices[i] = value; }
}
