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

        // Draw frustrum
        var frustrumCornerA = transform.position + dirA * viewDst;
        var frustrumCornerB = transform.position + dirB * viewDst;
        Visualizer.DrawLine (transform.position, frustrumCornerA, thickness);
        Visualizer.DrawLine (transform.position, frustrumCornerB, thickness);
        Visualizer.DrawLine (frustrumCornerA, frustrumCornerB, thickness);

        Visualizer.SetColour (viewOutline);

        var portalEdgeA = portal.transform.position - portal.transform.right * portal.transform.localScale.x / 2;
        var portalEdgeB = portal.transform.position + portal.transform.right * portal.transform.localScale.x / 2;

        // Draw lines through portal corners and create polygon of region viewed through portal
        var portalRegion = new List<Vector2> ();
        if (MathUtility.LineSegmentsIntersect (frustrumCornerA, frustrumCornerB, transform.position, transform.position + (portalEdgeA - transform.position) * viewDst)) {
            var p1 = MathUtility.PointOfLineLineIntersection (transform.position, portalEdgeA, frustrumCornerA, frustrumCornerB);
            if ((p1 - (Vector2) transform.position).magnitude >= ((Vector2) portalEdgeA - (Vector2) transform.position).magnitude) {
                Visualizer.DrawLine (transform.position, new Vector3 (p1.x, p1.y, transform.position.z), thickness);
                portalRegion.Add (p1);
                portalRegion.Add (portalEdgeA);
            }
        }
        if (MathUtility.LineSegmentsIntersect (frustrumCornerA, frustrumCornerB, transform.position, transform.position + (portalEdgeB - transform.position) * viewDst)) {
            var p2 = MathUtility.PointOfLineLineIntersection (transform.position, portalEdgeB, frustrumCornerA, frustrumCornerB);
            if ((p2 - (Vector2) transform.position).magnitude >= ((Vector2) portalEdgeB - (Vector2) transform.position).magnitude) {
                Visualizer.DrawLine (transform.position, new Vector3 (p2.x, p2.y, transform.position.z), thickness);
                portalRegion.Add (portalEdgeB);
                portalRegion.Add (p2);
            }
        }
        if (MathUtility.LineSegmentsIntersect (transform.position, frustrumCornerB, portalEdgeA, portalEdgeB)) {
            portalRegion.Add (transform.position + dirB * viewDst);
            portalRegion.Add (MathUtility.PointOfLineLineIntersection (transform.position, frustrumCornerB, portalEdgeA, portalEdgeB));
        }

        if (MathUtility.LineSegmentsIntersect (transform.position, frustrumCornerA, portalEdgeA, portalEdgeB)) {
            portalRegion.Add (transform.position + dirA * viewDst);
            portalRegion.Add (MathUtility.PointOfLineLineIntersection (transform.position, frustrumCornerA, portalEdgeA, portalEdgeB));
        }

        if (MathUtility.LineSegmentsIntersect (frustrumCornerA, frustrumCornerB, portalEdgeA, portalEdgeB)) {
            portalRegion.Add (MathUtility.PointOfLineLineIntersection (frustrumCornerA, frustrumCornerB, portalEdgeA, portalEdgeB));
        }

        Visualizer.SetColour (Color.red);
        if (portalRegion.Count >= 3) {
            Visualizer.DrawConvexHull (portalRegion, .1f);
        }

        Visualizer.activeColour = Color.red;
        Visualizer.DrawTriangle (transform.position - Vector3.forward * .1f, transform.eulerAngles.z + 180, 1, Vector3.forward);

        /*
        float j = 0;
        foreach (var p in portalRegion) {
            Visualizer.SetColour (new Color (j, j, j, 1));
            Visualizer.DrawSphere (p, .2f);
            j += (1 / (portalRegion.Count - 1f));
        }
        */

    }
}