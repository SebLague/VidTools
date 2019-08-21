using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugViewer {
    const string unlitAlphaShader = "Custom/UnlitColorAlpha";
    const string defaultShader = "Standard";
    static Shader unlitColAlphaShader;
    static Material material;
    static MaterialPropertyBlock materialProperties;

    // Mesh cache
    static Queue<Mesh> inactiveMeshes;
    static Mesh sphereMesh;

    static List<DrawInfo> drawList;

    static int lastFrameIndex;

    static DebugViewer () {
        Camera.onPreCull -= Draw;
        Camera.onPreCull += Draw;

        Init ();
    }

    static void Init () {
        if (sphereMesh == null) {
            inactiveMeshes = new Queue<Mesh> ();
            materialProperties = new MaterialPropertyBlock ();
            drawList = new List<DrawInfo> ();
            sphereMesh = SphereMesh.GenerateMesh ();
            unlitColAlphaShader = Shader.Find (defaultShader);
            material = new Material (unlitColAlphaShader);
        }

        // New frame index, so clear out last frame's draw list
        if (lastFrameIndex != Time.frameCount) {
            lastFrameIndex = Time.frameCount;

            // Store all unique meshes in inactive queue to be recycled
            var usedMeshes = new HashSet<Mesh> ();
            for (int i = 0; i < drawList.Count; i++) {
                if (!usedMeshes.Contains (drawList[i].mesh)) {
                    usedMeshes.Add (drawList[i].mesh);
                    inactiveMeshes.Enqueue (drawList[i].mesh);
                }
            }

            // Clear old draw list
            drawList.Clear ();
        }
    }

    public static void DrawSphere (Vector3 centre, float radius, Color colour) {
        Init ();
        drawList.Add (new DrawInfo (sphereMesh, centre, Quaternion.identity, Vector3.one * radius, colour));
    }

    public static void DrawRing (Vector3 centre, Vector3 normal, float startAngle, float angle, float innerRadius, float outerRadius, Color colour) {
        Init ();

        Mesh mesh = CreateOrRecycleMesh ();
        RingMesh.GenerateMesh (mesh, angle, innerRadius, outerRadius);

        float localYAngle = (startAngle - angle / 2);
        var rot = Quaternion.AngleAxis (localYAngle, normal) * Quaternion.FromToRotation (Vector3.up, normal);

        drawList.Add (new DrawInfo (mesh, centre, rot, Vector3.one, colour));
    }

    // Draw all items in the drawList on each game/scene camera
    static void Draw (Camera camera) {
        if (camera) {
            for (int i = 0; i < drawList.Count; i++) {
                DrawInfo drawData = drawList[i];
                Matrix4x4 matrix = Matrix4x4.TRS (drawData.position, drawData.rotation, drawData.scale);

                materialProperties.SetColor ("_Color", drawData.colour);
                Graphics.DrawMesh (drawData.mesh, matrix, material, 0, camera, 0, materialProperties);
            }

        }
    }

    static Mesh CreateOrRecycleMesh () {
        Mesh mesh = null;
        if (inactiveMeshes.Count > 0) {
            mesh = inactiveMeshes.Dequeue ();
            mesh.Clear ();
        } else {
            mesh = new Mesh ();
        }

        return mesh;
    }

    class DrawInfo {
        public Mesh mesh;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Color colour;

        public DrawInfo (Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color colour) {
            this.mesh = mesh;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.colour = colour;
        }
    }
}