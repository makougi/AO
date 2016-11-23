using UnityEngine;
using System.Collections;

public class Script_WorldLocationAndId : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public Vector3 GetWorldLocation () {
		if (GetComponent<Script_Airplane> ()) {
			return transform.position;
		}
		if (GetComponent<Script_Beacon> ()) {
			return GetComponent<Script_Beacon> ().GetWorldPosition ();
		}
		return new Vector3 (0, 0, 0);
	}

	public string GetId () {
		if (GetComponent<Script_Airplane> ()) {
			return GetComponent<Script_Airplane> ().GetId ();
		}
		if (GetComponent<Script_Beacon> ()) {
			return GetComponent<Script_Beacon> ().GetId ();
		}
		return null;
	}
}
