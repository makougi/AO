using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Script_OffsetButton : MonoBehaviour {

	public GameObject controller;
	bool toggle;

	// Use this for initialization
	void Start () {
		GetComponent<Image> ().color = new Color32 (185, 210, 235, 255);
		toggle = true;
		GetComponent<Button> ().onClick.AddListener (() => {
			ToggleOffset ();
		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ToggleOffset () {
		controller.GetComponent<Script_Controller> ().toggleAirplaneTextsOffset (toggle);
		toggle = !toggle;
		if (toggle) {
			GetComponent<Image> ().color = new Color32 (185, 210, 235, 255);
		} else {
			GetComponent<Image> ().color = new Color32 (135, 255, 135, 255);
		}
	}
}
