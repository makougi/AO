using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Script_IDParser : MonoBehaviour {

	private bool toggle;
	private string id;
	private GameObject target;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void ToggleIDTargetOnOff (bool tggle) {
		toggle = tggle;
	}

	public bool SetIdIfValid (string targetId) {
		foreach (List<GameObject> golist in GetComponent<Script_Controller> ().GetTargets ()) {
			foreach (GameObject go in golist) {
				//if go.GetComponent
			}
		}
		return false;
	}
}
