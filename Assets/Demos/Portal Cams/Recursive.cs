using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Recursive : MonoBehaviour {

    public Transform playerCam;
    public Transform linkedPortal;
    public int recursionLimit;
    public Color camCol;
    public bool drawArrows;
    public Vector3 arrowSettings;
    public float m;
    public float smoothT;
    Vector3 smoothV;

    void Update () {
        if (Input.GetKey (KeyCode.Space)) {
            var cam = Camera.main;
            var cam2 = GameObject.FindGameObjectWithTag ("Respawn").GetComponent<Camera> ();
            Vector3 c = Vector3.SmoothDamp (new Vector3 (cam.transform.position.x, cam.transform.position.y, cam.orthographicSize), new Vector3 (cam2.transform.position.x, cam2.transform.position.y, cam2.orthographicSize), ref smoothV, smoothT);
            cam.transform.position = new Vector3 (c.x, c.y, -10);
            cam.orthographicSize = c.z;
        }
        var localToWorldMatrix = playerCam.transform.localToWorldMatrix;
        Matrix4x4[] matrices = new Matrix4x4[recursionLimit];
        for (int i = 0; i < recursionLimit; i++) {
            localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            matrices[recursionLimit - i - 1] = localToWorldMatrix;
        }

        for (int i = 0; i < recursionLimit; i++) {
            Vector3 pos = matrices[i].GetColumn (3);
            float rot = matrices[i].rotation.eulerAngles.z;
            float alpha = (i + 1f) / recursionLimit;
            Visualizer.SetColour (new Color (camCol.r, camCol.g, camCol.b, alpha));
            Visualizer.activeStyle = Visualization.Style.UnlitAlpha;
            Visualizer.DrawTriangle2D (pos - Vector3.forward * .1f, rot, .5f, -Vector3.forward);
            if (drawArrows) {
                var nextPos = (i + 1 < recursionLimit) ? (Vector3) matrices[i + 1].GetColumn (3) : playerCam.transform.position;
                var dir = (nextPos - pos).normalized;
                Visualizer.SetColour (new Color (1, 1, 1, m));
                Visualizer.DrawArrow2D (pos + dir * arrowSettings.z, nextPos - dir * arrowSettings.z, arrowSettings.x, arrowSettings.y);
            }
        }
    }
}