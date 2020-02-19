using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ArrowTest3D : MonoBehaviour {
    public Transform target;
    public float radius = 0.5f;
    public float headRadius;

    void Update () {
        Visualizer.SetColourAndStyle (Color.red, Visualization.Style.Standard);
        Visualizer.DrawArrow3D (transform.position, target.position, radius, headRadius);
    }
}