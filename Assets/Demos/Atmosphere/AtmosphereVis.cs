using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AtmosphereVis : MonoBehaviour {

	public float planetSize;
	public Color atmosphereCol;
	public Color planetCol;

	public Molecule moleculePrefab;

	public int numMolecules;
	public float moleculeSize;
	public Color moleculeCol;
	public float perturbStrength;
	public float turnFraction;
	public float pow;
	public int seed;

	void Start () {
		if (Application.isPlaying) {
			for (int i = 0; i < numMolecules; i++) {
				float t = Mathf.Pow (i / (numMolecules - 1f), pow);
				float angle = turnFraction * i * 2 * Mathf.PI;
				Vector2 dir = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
				Vector2 pos = dir * Mathf.Lerp (planetSize, 1, t);
				var g = Instantiate (moleculePrefab, pos, Quaternion.identity);
				g.transform.localScale = Vector3.one * moleculeSize * 0.01f;
				g.surfaceHeight = planetSize;
				//Visualizer.DrawDisc (((Vector3) pos) + Vector3.forward * -0.02f, -Vector3.forward, moleculeSize * 0.01f);
			}
		}
	}

	void Update () {
		Visualizer.SetColour (atmosphereCol);
		Visualizer.DrawDisc (Vector3.forward * 0.01f, -Vector3.forward, 1);
		Visualizer.SetColour (planetCol);
		Visualizer.DrawDisc (Vector3.forward * -0.01f, -Vector3.forward, planetSize);

		/*
		Visualizer.SetColour (moleculeCol);
		Random.InitState (seed);
		for (int i = 0; i < numMolecules; i++) {
			float t = Mathf.Pow (i / (numMolecules - 1f), pow);
			float angle = turnFraction * i * 2 * Mathf.PI;
			Vector2 dir = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
			Vector2 pos = dir * Mathf.Lerp (planetSize, 1, t) + Random.insideUnitCircle * perturbStrength * 0.01f;
			Visualizer.DrawDisc (((Vector3) pos) + Vector3.forward * -0.02f, -Vector3.forward, moleculeSize * 0.01f);
		}
		*/
	}
}