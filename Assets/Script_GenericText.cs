using UnityEngine;
using System.Collections;

public class Script_GenericText : MonoBehaviour {
	
	private Vector3 beaconPosition;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		transform.position = Camera.main.WorldToScreenPoint (beaconPosition);
	}

	public void SetBeaconPosition (Vector3 bp) {
		beaconPosition = bp;
	}
}
