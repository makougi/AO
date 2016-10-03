using UnityEngine;
using System.Collections;

public class Script_GenericText : MonoBehaviour {
	
	private Vector3 beaconPosition;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void SetBeaconPosition (Vector3 bp) {
		beaconPosition = bp;
	}

	public void UpdateBeaconPosition () {
		transform.position = new Vector3 (Camera.main.WorldToScreenPoint (beaconPosition).x, Camera.main.WorldToScreenPoint (beaconPosition).y, 0);
	}
}
