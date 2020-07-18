using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour {

	public MoleculeSettings settings;

	float radius;
	float desiredHeight;
	Transform[] otherMolecules;
	[HideInInspector]
	public float surfaceHeight;

	Vector2 velocity;

	void Start () {
		radius = transform.localScale.x;
		desiredHeight = transform.position.magnitude;

		var allMolecules = FindObjectsOfType<Molecule> ();
		int j = 0;
		otherMolecules = new Transform[allMolecules.Length - 1];
		for (int i = 0; i < allMolecules.Length; i++) {
			if (allMolecules[i] != this) {
				otherMolecules[j] = allMolecules[i].transform;
				j++;
			}
		}
	}

	void Update () {

		Vector2 avoidForce = Vector2.zero;
		Vector2 dir = transform.position.normalized;
		Vector2 pos = transform.position;
		int k = 0;

		for (int i = 0; i < otherMolecules.Length; i++) {
			Vector2 offset = (Vector2) otherMolecules[i].position - pos;
			float sqrDst = offset.sqrMagnitude;
			if (sqrDst < radius * radius * 5) {
				avoidForce -= offset.normalized / Mathf.Max (sqrDst, radius) * settings.avoidStrength;
				k++;
			}
		}
		Debug.Log (k + "  " + radius);
		//avoidForce -= dir / Mathf.Max ((dir - pos).sqrMagnitude, radius) * settings.avoidStrength;
		//avoidForce += dir / Mathf.Max ((dir * surfaceHeight - pos).sqrMagnitude, radius) * settings.avoidStrength;

		//float dstFromDesiredHeight = desiredHeight - transform.position.magnitude;
		//Vector2 heightStrength = transform.position.normalized * dstFromDesiredHeight;

		var accel = Vector2.ClampMagnitude (avoidForce, settings.avoidanceWeight) - dir * settings.gravity;
		velocity = accel * Time.deltaTime;
		transform.position += (Vector3) velocity * Time.deltaTime;
		//float t = transform.position.magnitude;
		transform.position = (Vector2) transform.position.normalized * Mathf.Clamp (transform.position.magnitude, surfaceHeight, 1);
	}
}