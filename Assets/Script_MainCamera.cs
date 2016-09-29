using UnityEngine;
using System.Collections;

public class Script_MainCamera : MonoBehaviour {

	float fieldOfView;
	float delay;


	// Use this for initialization
	void Start () {
		GetComponent<Camera> ().fieldOfView = 150;
		delay = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ZoomIn () {
		if (Time.time > delay) {
			fieldOfView = GetComponent<Camera> ().fieldOfView;
			if (fieldOfView > 0.0005) {
				GetComponent<Camera> ().fieldOfView = fieldOfView / ((180 - fieldOfView) / 2000 + 1);
			}
			delay = Time.time + 0.03f;
		}
	}

	public void ZoomOut () {
		if (Time.time > delay) {
			fieldOfView = GetComponent<Camera> ().fieldOfView;
			if (fieldOfView < 175) {
				GetComponent<Camera> ().fieldOfView = fieldOfView * ((180 - fieldOfView) / 2000 + 1);
			}
			delay = Time.time + 0.03f;
		}
	}
}
