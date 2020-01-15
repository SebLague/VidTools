using System.Collections.Generic;
using UnityEngine;
using Visualization;
using Visualization.MeshGeneration;
using static Visualization.Manager;

public static class Visualizer {

    public static Color activeColour = Color.black;
    public static Style activeStyle = Style.Unlit;

    public static void SetColour (Color colour) {
        activeColour = colour;
    }

    public static void SetColourAndStyle (Color colour, Style style) {
        activeColour = colour;
        activeStyle = style;
    }

    public static void DrawSphere (Vector3 centre, float radius) {
        CreateVisualElement (sphereMesh, centre, Quaternion.identity, Vector3.one * radius);
    }

    public static void DrawLine (Vector3 start, Vector3 end, float thickness) {
        float thicknessFactor = 1 / 25f;
        Mesh mesh = CreateOrRecycleMesh ();
        CylinderMesh.GenerateMesh (mesh);
        Vector3 centre = (start + end) / 2;
        var rot = Quaternion.FromToRotation (Vector3.up, (start - end).normalized);
        Vector3 scale = new Vector3 (thickness * thicknessFactor, (start - end).magnitude, thickness * thicknessFactor);
        CreateVisualElement (mesh, centre, rot, scale);
    }

    public static void DrawRay (Vector3 start, Vector3 dir, float thickness) {
        DrawLine (start, start + dir, thickness);
    }

    public static void DrawRing (Vector3 centre, Vector3 normal, float startAngle, float angle, float innerRadius, float outerRadius) {
        Mesh mesh = CreateOrRecycleMesh ();
        RingMesh.GenerateMesh (mesh, angle, innerRadius, outerRadius);

        //float localYAngle = (startAngle - angle / 2); // centre angle
        float localYAngle = 0;
        var rot = Quaternion.AngleAxis (localYAngle, normal) * Quaternion.FromToRotation (Vector3.up, normal);

        CreateVisualElement (mesh, centre, rot, Vector3.one);
    }

    public static void DrawDisc (Vector3 centre, Vector3 normal, float radius) {
        DrawRing (centre, normal, 0, 0, 0, radius);
    }

    public static void DrawArc (Vector3 centre, Vector3 normal, float startAngle, float angle, float radius) {
        DrawRing (centre, normal, startAngle, angle, 0, radius);
    }

    public static void DrawPolygon (IEnumerable<Vector2> points) {
        Mesh mesh = CreateOrRecycleMesh ();
        var meshData = Triangulator.Triangulate (points, null, null);
        mesh.vertices = meshData.verts;
        mesh.triangles = meshData.triangles;
        mesh.RecalculateBounds ();
        mesh.RecalculateNormals ();
        CreateVisualElement (mesh, Vector3.zero, Quaternion.identity, Vector3.one);
    }
    public static void DrawPolygon (params Vector2[] points) {
        DrawPolygon (new List<Vector2> (points));
    }
}