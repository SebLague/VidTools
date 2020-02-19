using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visualization;

[ExecuteInEditMode]
public class ArrowTest3D : MonoBehaviour {

    public float pointRadius;
    public float baseLength = 1;
    public float radius = 0.5f;
    public float headRadius;

    public Transform[] shape;

    [Header ("Matrix")]
    public Matrix3x3 matrix;

    void Update () {
        Vector3 iHat = Vector3.right;
        Vector3 jHat = Vector3.up;
        Vector3 kHat = Vector3.forward;

        // do matrix stuff:
        iHat = matrix.Mul (iHat);
        jHat = matrix.Mul (jHat);
        kHat = matrix.Mul (kHat);

        Visualizer.SetColourAndStyle (Palette.red, Visualization.Style.Diffuse);
        Visualizer.DrawArrow3D (Vector3.zero, iHat * baseLength, radius, headRadius);
        Visualizer.SetColour (Palette.green);
        Visualizer.DrawArrow3D (Vector3.zero, jHat * baseLength, radius, headRadius);
        Visualizer.SetColour (Palette.blue);
        Visualizer.DrawArrow3D (Vector3.zero, kHat * baseLength, radius, headRadius);

        if (shape != null) {
            var positions = new Vector3[shape.Length];
            var transformedPositions = new Vector3[shape.Length];
            for (int i = 0; i < shape.Length; i++) {
                positions[i] = shape[i].position;
                transformedPositions[i] = matrix.Mul (positions[i]);
            }

            for (int i = 0; i < shape.Length; i++) {

                Visualizer.SetColourAndStyle (Color.grey, Style.Unlit);
                Visualizer.DrawSphere (transformedPositions[i], pointRadius);
                Visualizer.DrawLine (transformedPositions[i], transformedPositions[(i + 1) % shape.Length], .5f);

                Visualizer.SetColourAndStyle (Color.white, Style.Unlit);
            }
        }
    }

    [System.Serializable]
    public struct Matrix3x3 {
        public Vector3 row1;
        public Vector3 row2;
        public Vector3 row3;

        public Vector3 Mul (Vector3 v) {
            return new Vector3 (Vector3.Dot (row1, v), Vector3.Dot (row2, v), Vector3.Dot (row3, v));
        }
    }
}