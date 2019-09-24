using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visualisation;

[ExecuteInEditMode]
public class Test : MonoBehaviour {

    public float startAngle, angle, innerRadius, outerRadius;
    public bool draw;

    void Update () {
        if (draw) {
            Vis.DrawLine (Vector3.zero, transform.position, 1, Color.white, Style.Unlit);
            Vis.DrawLine (Vector3.up * 2, transform.position + Vector3.right * 2, 1, Color.red, Style.Standard);
            //Vis.DrawCylinder (Vector3.zero, 1, Color.white);
            //Vis.DrawRing (transform.position, transform.up, startAngle, angle, innerRadius, outerRadius, Color.red, Style.Unlit);
        }
    }
}