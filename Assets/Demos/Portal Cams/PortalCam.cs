using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PortalCam : MonoBehaviour {
    public Transform myPortal;
    public Transform linkedPortal;
    public Transform playerCam;
    public bool mirrorPlayerCam;

    void Update () {
        if (mirrorPlayerCam) {
            var m = myPortal.localToWorldMatrix * linkedPortal.worldToLocalMatrix * playerCam.localToWorldMatrix;
            transform.position = m.GetColumn (3);
            transform.rotation = m.rotation;
        }

    }
}