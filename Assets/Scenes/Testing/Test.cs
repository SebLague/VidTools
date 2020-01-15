using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visualization;

[ExecuteInEditMode]
public class Test : MonoBehaviour {

    public float startAngle, angle, innerRadius, outerRadius;
    public float lineWidth = 1;
    public float headSize = 1;
    public bool draw;

    void Update () {
        if (draw) {
            //Visualizer.DrawRing (transform.position, transform.up, 0, 360, 0, 4);
            Visualizer.DrawArrow2D (Vector3.zero, transform.position, lineWidth, headSize, true);
            //Visualizer.DrawDisc (transform.position, transform.up, 5);
            //Visualizer.DrawRing (transform.position, transform.up, transform.eulerAngles.y, 30, 1, 4);
            //Visualizer.DrawSphere (transform.position, .25f);
            //Visualizer.DrawTriangle (transform.position, transform.eulerAngles.y, .5f, transform.up);
            /*
            Debug.Log("Draw");
            
            Visualizer.activeStyle = Style.Unlit;
            Visualizer.SetColour (Color.white);
            Visualizer.DrawLine (Vector3.zero, transform.position, 1);
            Visualizer.SetColour (Color.red);
            Visualizer.DrawLine (Vector3.up * 2, transform.position + Vector3.right * 2, 1);
            Visualizer.DrawSphere (Vector3.zero, .5f);
            */
            //Vis.DrawCylinder (Vector3.zero, 1, Color.white);
            //Vis.DrawRing (transform.position, transform.up, startAngle, angle, innerRadius, outerRadius, Color.red, Style.Unlit);
        }
    }
}