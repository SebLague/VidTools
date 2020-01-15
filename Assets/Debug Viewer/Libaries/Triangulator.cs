using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using UnityEngine;

// From https://github.com/eppz/Triangle.NET
// With some modifications
public class Triangulator {

    public static MeshData2D Triangulate (IEnumerable<Vector2> outline, IEnumerable<Vector2> innerPoints, IEnumerable<IEnumerable<Vector2>> holes) {

        Polygon polygon = new Polygon ();

        // Outline
        if (outline != null) {
            polygon.Add (new Contour (PointsToVertices (outline, 0)));
        }
        // Inner points
        if (innerPoints != null) {
            polygon.Points.AddRange (PointsToVertices (innerPoints, polygon.Points.Count));
        }

        // Holes
        if (holes != null) {
            foreach (var holePoints in holes) {
                polygon.Add (new Contour (PointsToVertices (holePoints, polygon.Points.Count)), true);
            }
        }
        var triangulation = polygon.Triangulate ();

        var triangles = new int[triangulation.Triangles.Count * 3];
        var points = new Vector2[polygon.Points.Count];
        var verts = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++) {
            points[i] = new Vector2 ((float) polygon.Points[i].x, (float) polygon.Points[i].y);
            verts[i] = new Vector3 (points[i].x, points[i].y);
        }

        int triangleIndex = 0;
        foreach (var tri in triangulation.Triangles) {
            for (int i = 0; i < 3; i++) {
                triangles[triangleIndex * 3 + i] = tri.GetVertex (2 - i).index;
            }
            triangleIndex++;
        }

        return new MeshData2D (points, triangles, verts);
    }

    //public static void CreateMesh(MeshData2D data2D, float vertexZ = 0) {   
    //}

    public class MeshData2D {
        public Vector2[] points;
        public Vector3[] verts;
        public int[] triangles;

        public MeshData2D (Vector2[] points, int[] triangles, Vector3[] verts) {
            this.points = points;
            this.triangles = triangles;
            this.verts = verts;
        }
    }

    static IEnumerable<Vertex> PointsToVertices (IEnumerable<Vector2> points, int startI = 0) {
        var verts = new Vertex[points.Count ()];
        int i = 0;
        foreach (var p in points) {
            var vertex = new Vertex (p.x, p.y);
            vertex.index = startI + i;
            verts[i] = vertex;
            i++;
        }
        return verts;
    }

}