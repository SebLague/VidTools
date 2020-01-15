using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Portal : MonoBehaviour {

    public float radius = .1f;
    public Color dotCol = Color.black;

    void Update () {
        Visualizer.SetColour (dotCol);
        Visualizer.DrawSphere (transform.position - transform.right * transform.localScale.x / 2, radius);
        Visualizer.DrawSphere (transform.position + transform.right * transform.localScale.x / 2, radius);

    }
}