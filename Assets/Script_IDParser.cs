using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Script_IDParser : MonoBehaviour {

	private GameObject mainCamera;
	private List<List<GameObject>> targets;
	private GameObject target;
	private bool toggle;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void Construct (GameObject mainCameraGO, List<List<GameObject>> targetsGO) {
		mainCamera = mainCameraGO;
		targets = targetsGO;
	}

	public void ToggleIDTargetOnOff (bool tggle) {
		toggle = tggle;
		if (toggle) {
			mainCamera.GetComponent<Script_MainCamera> ().SetTargetAndResetOffset (target);
		} else {
			mainCamera.GetComponent<Script_MainCamera> ().SetTargetAndResetOffset (null);
		}
	}

	public bool SetIdIfValid (string targetId) {
		if (targetId == "") {
			target = null;
			ToggleIDTargetOnOff (toggle);
			return false;
		} else {
			foreach (List<GameObject> golist in targets) {
				foreach (GameObject go in golist) {
					if (targetId == go.GetComponent<Script_WorldLocationAndId> ().GetId ()) {
						target = go;
						ToggleIDTargetOnOff (toggle);
						return true;
					}
				}
			}
			ToggleIDTargetOnOff (toggle);
			return false;
		}
	}
}
