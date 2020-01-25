using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour {
    public bool active;
    public float moveSpeed = 3;
    public float smoothMoveTime = .1f;
    public bool controlRelativeToDir;
    Vector2 velocity;
    Vector2 smoothV;
    Vector3 lastActiveMousePos;
    Vector3 mouseOffset;

    void Update () {
        if (active) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - mouseOffset;
            Vector2 offset = mousePos - transform.position;
            float angle = Mathf.Atan2 (offset.y, offset.x) * Mathf.Rad2Deg - 90;
            transform.eulerAngles = Vector3.forward * angle;

            Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
            if (input != Vector2.zero) {
                input = input.normalized;
            }
            Vector2 targetVelocity = Vector3.up * input.y * moveSpeed + Vector3.right * input.x * moveSpeed;
            if (controlRelativeToDir) {
                targetVelocity = transform.up * input.y * moveSpeed + transform.right * input.x * moveSpeed;
            }
            velocity = Vector2.SmoothDamp (velocity, targetVelocity, ref smoothV, smoothMoveTime);
            transform.position += (Vector3) velocity * Time.deltaTime;
            lastActiveMousePos = mousePos;
        }

        if (Input.GetKeyDown (KeyCode.P)) {
            active = !active;
            if (active) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
                mouseOffset = mousePos - lastActiveMousePos;
            }
        }
    }
}