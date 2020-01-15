using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Cam : MonoBehaviour {

    public Portal portal;
    public float thickness = 0.1f;
    public float fov = 60;
    public float viewDst = 10;
    public Color frustrumCol = Color.white;
    public Color viewOutline = Color.white;

    void Update () {
        Visualizer.SetColour (frustrumCol);
        float angle = transform.eulerAngles.z;
        Vector3 dirA = new Vector3 (Mathf.Cos ((angle - fov / 2 + 90) * Mathf.Deg2Rad), Mathf.Sin ((angle - fov / 2 + 90) * Mathf.Deg2Rad));
        Vector3 dirB = new Vector3 (Mathf.Cos ((angle + fov / 2 + 90) * Mathf.Deg2Rad), Mathf.Sin ((angle + fov / 2 + 90) * Mathf.Deg2Rad));
        Visualizer.DrawRay (transform.position, dirA * viewDst, thickness);
        Visualizer.DrawRay (transform.position, dirB * viewDst, thickness);
        Visualizer.DrawLine (transform.position + dirA * viewDst, transform.position + dirB * viewDst, thickness);

        Visualizer.SetColour (viewOutline);

        var portalEdgeA = portal.transform.position - portal.transform.right * portal.transform.localScale.x / 2;
        var portalEdgeB = portal.transform.position + portal.transform.right * portal.transform.localScale.x / 2;

        if (MathUtility.LineSegmentsIntersect (transform.position + dirA * viewDst, transform.position + dirB * viewDst, transform.position, transform.position + (portalEdgeA - transform.position) * viewDst)) {
            var p1 = MathUtility.PointOfLineLineIntersection (transform.position, portalEdgeA, transform.position + dirA * viewDst, transform.position + dirB * viewDst);
            Visualizer.DrawLine (transform.position, new Vector3 (p1.x, p1.y, transform.position.z), thickness);
        }
        if (MathUtility.LineSegmentsIntersect (transform.position + dirA * viewDst, transform.position + dirB * viewDst, transform.position, transform.position + (portalEdgeB - transform.position) * viewDst)) {
            var p2 = MathUtility.PointOfLineLineIntersection (transform.position, portalEdgeB, transform.position + dirA * viewDst, transform.position + dirB * viewDst);
            Visualizer.DrawLine (transform.position, new Vector3 (p2.x, p2.y, transform.position.z), thickness);
        }

        Visualizer.SetColour (Color.red);
        Visualizer.DrawPolygon (transform.position, portalEdgeA, portalEdgeB);

    }
}