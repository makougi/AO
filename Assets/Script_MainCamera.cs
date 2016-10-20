using UnityEngine;
using System.Collections;

public class Script_MainCamera : MonoBehaviour {

	public GameObject controller;

	private float size;
	private float fieldOfView;
	private float delay;
	private GameObject target;
	private Vector2 offset;

	// Use this for initialization
	void Start () {
		GetComponent<Camera> ().fieldOfView = 150;
		GetComponent<Camera> ().orthographicSize = 50;
		delay = 0;
	}

	// Update is called once per frame
	void Update () {
		if (target) {
			transform.position = new Vector3 (target.transform.position.x + offset.x, transform.position.y, target.transform.position.z + offset.y);
			controller.GetComponent<Script_Controller> ().UpdateUIElementPositions ();
		}
	}

	public void SetTarget (GameObject trgt) {
		target = trgt;
	}

	public void ZoomIn () {
		if (Time.time > delay) {
			if (Camera.main.orthographic) {
				size = GetComponent<Camera> ().orthographicSize;
				size--;
				if (size >= 1) {
					GetComponent<Camera> ().orthographicSize--;
				}
			} else {
				fieldOfView = GetComponent<Camera> ().fieldOfView;
				if (fieldOfView > 0.0005) {
					GetComponent<Camera> ().fieldOfView = fieldOfView / ((180 - fieldOfView) / 2000 + 1);
				}
			}
			delay = Time.time + 0.03f;
			controller.GetComponent<Script_Controller> ().UpdateUIElementPositions ();
		}
	}

	public void ZoomOut () {
		if (Time.time > delay) {
			if (Camera.main.orthographic) {
				size = GetComponent<Camera> ().orthographicSize;
				size++;
				if (size <= 200) {
					GetComponent<Camera> ().orthographicSize++;
				}
			} else {
				fieldOfView = GetComponent<Camera> ().fieldOfView;
				if (fieldOfView < 175) {
					GetComponent<Camera> ().fieldOfView = fieldOfView * ((180 - fieldOfView) / 2000 + 1);
				}
			}
			delay = Time.time + 0.03f;
			controller.GetComponent<Script_Controller> ().UpdateUIElementPositions ();
		}
	}
}