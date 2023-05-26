using UnityEngine;
using UnityEngine.InputSystem;

namespace VidTools.Cameras
{
	public class DragCam2D : MonoBehaviour
	{
		[SerializeField] float zoomSpeed = 1f;
		[SerializeField] bool zoomToMouse = true;
		[SerializeField] Vector2 zoomRange = new Vector2(0.0001f, 1000);

		Camera cam;
		Vector2 mouseDragScreenPosOld;

		void Start()
		{
			cam = GetComponent<Camera>();
		}

		void LateUpdate()
		{
			Mouse mouse = Mouse.current;
			Vector2 mouseScreenPos = mouse.position.ReadValue();
			Vector2 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);

			// Pan
			if (mouse.middleButton.wasPressedThisFrame)
			{
				mouseDragScreenPosOld = mouseScreenPos;
			}
			if (mouse.middleButton.isPressed)
			{
				Vector2 mouseWorldPosOld = cam.ScreenToWorldPoint(mouseDragScreenPosOld);
				transform.position += (Vector3)(mouseWorldPosOld - mouseWorldPos);
				mouseDragScreenPosOld = mouseScreenPos;
			}
			Vector2 mouseWorldPosAfterPanning = cam.ScreenToWorldPoint(mouseScreenPos);


			// Zoom
			float deltaZoom = -GetScrollInput() * cam.orthographicSize * zoomSpeed * 0.1f;
			cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + deltaZoom, zoomRange.x, zoomRange.y);
			// Adjust cam pos to centre zoom on mouse
			if (zoomToMouse)
			{
				Vector2 mouseWorldPosAfterZoom = cam.ScreenToWorldPoint(mouseScreenPos);
				transform.position += (Vector3)(mouseWorldPosAfterPanning - mouseWorldPosAfterZoom);
			}

		}

		float GetScrollInput()
		{
			float scroll = Mouse.current.scroll.ReadValue().y;

			// On windows, scroll has huge values. (bug?)
			if (Mathf.Abs(scroll) >= 120)
			{
				scroll /= 120f;
			}
			return scroll;
		}

	}
}