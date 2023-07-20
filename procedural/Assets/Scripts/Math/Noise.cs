using System;
using UnityEngine;
using Unity.Mathematics;

public struct Simplex {
    public static float Sample(float2 xy) => noise.snoise(xy);
}

public struct Distord {

    public static float2 Sample(float2 xy) {
        float2 cell = math.round(xy);

        float2 offset = xy - cell;
        int2 neighCells = (int2)math.sign(offset);
        float2 l = math.abs(offset);

        float2 gradient00 = Hash.Float2(cell);
        float2 gradient10 = Hash.Float2(cell + new float2(neighCells.x, 0));
        float2 gradient01 = Hash.Float2(cell + new float2(0, neighCells.y));
        float2 gradient11 = Hash.Float2(cell + neighCells);

        float2 lerpY0 = (1.0f - l.x) * gradient00 + l.x * gradient10;
        float2 lerpY1 = (1.0f - l.x) * gradient01 + l.x * gradient11;
        float2 lerp = (1.0f - l.y) * lerpY0 + l.y * lerpY1;

        return lerp;
    }
}

public struct Voronoi {

    public struct Cell {
        public float distance;
        public float2 cellPoint;
        public Color debug;
    }

    public static Cell Sample(float2 xy) {
        float2 cell = math.round(xy);

        float2 cellPoint = GetCellPoint(cell);
        float2 minNeighbourCellPoint = cellPoint;

        float2 vector = xy - cellPoint;
        float minDistSq = math.dot(vector, vector);

        SampleNeighbour(xy, cell + new float2(-1, -1), ref minDistSq, ref minNeighbourCellPoint);
        SampleNeighbour(xy, cell + new float2(0, -1), ref minDistSq, ref minNeighbourCellPoint);
        SampleNeighbour(xy, cell + new float2(+1, -1), ref minDistSq, ref minNeighbourCellPoint);

        SampleNeighbour(xy, cell + new float2(-1, 0), ref minDistSq, ref minNeighbourCellPoint);
        SampleNeighbour(xy, cell + new float2(+1, 0), ref minDistSq, ref minNeighbourCellPoint);

        SampleNeighbour(xy, cell + new float2(-1, +1), ref minDistSq, ref minNeighbourCellPoint);
        SampleNeighbour(xy, cell + new float2(0, +1), ref minDistSq, ref minNeighbourCellPoint);
        SampleNeighbour(xy, cell + new float2(+1, +1), ref minDistSq, ref minNeighbourCellPoint);

        return new Cell {
            distance = math.sqrt(minDistSq),
            cellPoint = minNeighbourCellPoint,
        };
    }

    private static void SampleNeighbour(
        float2 xy, float2 neightbourCell,
        ref float outMinDistSq, ref float2 outMinNeighbourCellPoint) {

        float2 neighbourCellPoint = GetCellPoint(neightbourCell);
        float2 vector = neighbourCellPoint - xy;
        float distSq = math.dot(vector, vector);

        outMinNeighbourCellPoint = (distSq < outMinDistSq) ? neighbourCellPoint : outMinNeighbourCellPoint;
        outMinDistSq = math.min(outMinDistSq, distSq);
    }

    private static float2 GetCellPoint(float2 cell) =>
        cell + Hash.Float2(cell) * 0.5f;

}

public struct Hash {

    const float SCALE_INT_2_FLOAT = 1.0f / int.MaxValue;

    // output: [-1, +1]
    public static float Float(float x) {
        uint bitsX = (uint)BitConverter.SingleToInt32Bits(x);
        int signedBitsX = (int)UInt(bitsX);
        return (float)signedBitsX * SCALE_INT_2_FLOAT;
    }

    // output: [-1, +1]
    public static float Float(float2 xy) {
        uint bitsX = (uint)BitConverter.SingleToInt32Bits(xy.x);
        uint bitsY = (uint)BitConverter.SingleToInt32Bits(xy.y);
        int signedBitsX = (int)UInt(bitsX ^ UInt(bitsY));
        return (float)(signedBitsX) * SCALE_INT_2_FLOAT;
    }

    // output: ([-1, +1], [-1, +1])
    public static float2 Float2(float2 xy) {
        uint bitsX = (uint)BitConverter.SingleToInt32Bits(xy.x);
        uint bitsY = (uint)BitConverter.SingleToInt32Bits(xy.y);
        int signedBitsX = (int)UInt(bitsX ^ UInt(bitsY ^ bitsX));
        int signedBitsy = (int)UInt(bitsY ^ bitsX ^ UInt(bitsX));
        return new float2(
            (float)(signedBitsX) * SCALE_INT_2_FLOAT,
            (float)(signedBitsy) * SCALE_INT_2_FLOAT
        );
    }

    // output: [0x00000000, 0xFFFFFFFF]
    public static uint UInt(uint x) {
        x = (x + 0x7ed55d16) + (x << 12);
        x = (x ^ 0xc761c23c) ^ (x >> 19);
        x = (x + 0x165667b1) + (x << 5);
        x = (x + 0xd3a2646c) ^ (x << 9);
        x = (x + 0xfd7046c5) + (x << 3);
        x = (x ^ 0xb55a4f09) ^ (x >> 16);
        return x;
    }
}


public struct ColorMap {

    // input: [-1, +1] !
    public static Color Map(float x) =>
        Color.HSVToRGB(x * 0.5f + 0.5f, 1.0f, 1.0f);
}