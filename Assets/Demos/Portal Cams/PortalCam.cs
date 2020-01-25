using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PortalCam : MonoBehaviour {
    public Transform myPortal;
    public Transform linkedPortal;
    public Transform playerCam;
    public float targetT;
    public float timeT;
    float smoothV;

    [Range (0, 1)]
    public float t;
    public bool mirrorPlayerCam;

    void Update () {
        if (mirrorPlayerCam) {
            var m = myPortal.localToWorldMatrix * linkedPortal.worldToLocalMatrix * playerCam.localToWorldMatrix;
            //transform.SetPositionAndRotation (Vector3.Lerp (playerCam.position, m.GetColumn (3), t), Quaternion.Slerp (playerCam.rotation, m.rotation, t));
            transform.SetPositionAndRotation (m.GetColumn (3), m.rotation);

            //GetComponent<Cam> ().SetTime (t, playerCam.GetComponent<Cam> ());
            if (Application.isPlaying) {
                t = Mathf.SmoothDamp (t, targetT, ref smoothV, timeT);
            }
            transform.position = Vector3.Lerp (transform.position, myPortal.position, t);
            transform.position += Vector3.forward * -.07f;
        } else {
            //
        }
        GetComponent<Cam> ().SetTime (1, null);
    }
}