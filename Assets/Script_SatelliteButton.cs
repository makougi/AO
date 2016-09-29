﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Script_SatelliteButton : MonoBehaviour {

	public GameObject controller;
	public GameObject radarButton;

	// Use this for initialization
	void Start () {
		GetComponent<Button> ().onClick.AddListener (() => {
			SwitchButtonOn ();
		});
		GetComponent<Image> ().color = new Color32 (185, 210, 235, 255);
	}

	// Update is called once per frame
	void Update () {

	}

	void SwitchButtonOn () {
		controller.GetComponent<Script_Controller> ().switchDisplay ("satellite");
		GetComponent<Image> ().color = new Color32 (135, 255, 135, 255);
		radarButton.GetComponent<Image> ().color = new Color32 (185, 210, 235, 255);
	}

	public void SwitchButtonOff () {
		GetComponent<Image> ().color = new Color32 (185, 210, 235, 255);
	}
}
