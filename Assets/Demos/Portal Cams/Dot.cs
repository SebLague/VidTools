using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Dot : MonoBehaviour {

    public Transform portal;
    public Vector2 arrowSettings;
    public Color arrowCol;
    public Color arcCol;
    public Vector2 arcRadii;
    public bool showArrow;
    public bool showAngle;
    public TMPro.TextMeshPro text;
    public bool showText;
    public float len = 2.9f;

    void Update () {
        float dot = Vector2.Dot (portal.up, (transform.position - portal.position) / len);
        var s = string.Format ("{0:0.0}", dot).Replace (",", ".");
        if (!s.Contains ("-")) {
            s = "+" + s;
        }
        s.Replace("-","–");
        text.text = s;
        if (!showText) {
            text.text = "";
        }
        Visualizer.SetColour (Color.white);
        Visualizer.DrawSphere (transform.position, transform.localScale.x);

        Visualizer.SetColour (arrowCol);
        if (showArrow)
            Visualizer.DrawArrow2D (portal.position, transform.position + (portal.position - transform.position).normalized * (transform.localScale.x), arrowSettings.x, arrowSettings.y, true, transform.position.z);
        Visualizer.SetColour (arcCol);
        float angle = -Vector2.SignedAngle (portal.up, transform.position - portal.position);
        if (showAngle)
            Visualizer.DrawRing (portal.position + Vector3.forward * -.2f, -Vector3.forward, Mathf.Atan2 (portal.up.x, portal.up.y) * Mathf.Rad2Deg, angle, arcRadii.x, arcRadii.y);
    }
}