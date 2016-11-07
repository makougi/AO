using UnityEngine;
using System.Collections;

public class Script_KeyboardControls : MonoBehaviour {

	public GameObject mainCamera;

	private float mainCameraMoveSpeed;

	// Use this for initialization
	void Start () {
		mainCameraMoveSpeed = 30;
	}

	// Update is called once per frame
	void Update () {
		UpdateMainCamera ();
	}

	void UpdateMainCamera () {
		if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
			if (mainCamera.GetComponent<Camera> ().orthographic) {
				mainCameraMoveSpeed = mainCamera.GetComponent<Camera> ().orthographicSize;
			} else {
				mainCameraMoveSpeed = mainCamera.GetComponent<Camera> ().fieldOfView * 3;
			}
			if (Input.GetKey (KeyCode.UpArrow)) {
				mainCamera.GetComponent<Script_MainCamera> ().AddToOffset (new Vector2 (0, mainCameraMoveSpeed * Time.deltaTime));
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				mainCamera.GetComponent<Script_MainCamera> ().AddToOffset (new Vector2 (0, -(mainCameraMoveSpeed * Time.deltaTime)));
			}
			if (Input.GetKey (KeyCode.LeftArrow)) {
				mainCamera.GetComponent<Script_MainCamera> ().AddToOffset (new Vector2 (-(mainCameraMoveSpeed * Time.deltaTime), 0));
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				mainCamera.GetComponent<Script_MainCamera> ().AddToOffset (new Vector2 (mainCameraMoveSpeed * Time.deltaTime, 0));
			}
			GetComponent<Script_Controller> ().UpdateUIElementPositions ();
		}
	}
}
