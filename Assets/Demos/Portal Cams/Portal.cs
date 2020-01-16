using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Portal : MonoBehaviour {

    public float radius = .1f;
    public Color dotCol = Color.black;
    public Vector2 arrowDst;
    public Vector2 arrowSize;
    public Color arrowCol;

    void Update () {
        Visualizer.SetColour (dotCol);
        Visualizer.DrawSphere (transform.position - transform.right * transform.localScale.x / 2, radius);
        Visualizer.DrawSphere (transform.position + transform.right * transform.localScale.x / 2, radius);

        Visualizer.SetColourAndStyle (arrowCol, Visualization.Style.UnlitAlpha);
        Visualizer.DrawArrow2D (transform.position + transform.up * arrowDst.x, transform.position + transform.up * (arrowDst.x + arrowDst.y), arrowSize.x, arrowSize.y, true, transform.position.z);
        Visualizer.activeStyle = Visualization.Style.Unlit;
    }
}