using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visualization;

[ExecuteInEditMode]
public class Test : MonoBehaviour {

    public float startAngle, angle, innerRadius, outerRadius;
    public bool draw;

    void Update () {
        if (draw) {
            Debug.Log("Draw");
            Visualizer.activeStyle = Style.Unlit;
            Visualizer.SetColour (Color.white);
            Visualizer.DrawLine (Vector3.zero, transform.position, 1);
            Visualizer.SetColour (Color.red);
            Visualizer.DrawLine (Vector3.up * 2, transform.position + Vector3.right * 2, 1);
            Visualizer.DrawSphere (Vector3.zero, .5f);
            //Vis.DrawCylinder (Vector3.zero, 1, Color.white);
            //Vis.DrawRing (transform.position, transform.up, startAngle, angle, innerRadius, outerRadius, Color.red, Style.Unlit);
        }
    }
}