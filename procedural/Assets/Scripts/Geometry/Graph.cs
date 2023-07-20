using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Graph<TPoly> where TPoly : struct, IPolygon {
    public Buffer<Vertex> vertices = new Buffer<Vertex>(4);
    public Buffer<TPoly> polygons = new Buffer<TPoly>(4);
}
