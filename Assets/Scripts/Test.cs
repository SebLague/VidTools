using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test : MonoBehaviour {

    public float startAngle, angle, innerRadius, outerRadius;

    void Update () {
        DebugViewer.DrawRing (transform.position, transform.up, startAngle, angle, innerRadius, outerRadius, Color.red);
    }
}