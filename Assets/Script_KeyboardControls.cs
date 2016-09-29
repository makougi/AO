using UnityEngine;
using System.Collections;

public class Script_KeyboardControls : MonoBehaviour {

	public GameObject mainCamera;
	int mainCameraMoveSpeed;

	// Use this for initialization
	void Start () {
		mainCameraMoveSpeed = 30;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMainCamera ();
	}

	void UpdateMainCamera () {
		if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKey (KeyCode.UpArrow)) {
			mainCamera.transform.position = new Vector3 (mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z + mainCameraMoveSpeed * Time.deltaTime);
		}
		if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKey (KeyCode.DownArrow)) {
			mainCamera.transform.position = new Vector3 (mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z - mainCameraMoveSpeed * Time.deltaTime);
		}
		if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKey (KeyCode.LeftArrow)) {
			mainCamera.transform.position = new Vector3 (mainCamera.transform.position.x - mainCameraMoveSpeed * Time.deltaTime, mainCamera.transform.position.y, mainCamera.transform.position.z);
		}
		if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKey (KeyCode.RightArrow)) {
			mainCamera.transform.position = new Vector3 (mainCamera.transform.position.x + mainCameraMoveSpeed * Time.deltaTime, mainCamera.transform.position.y, mainCamera.transform.position.z);
		}
		if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && (Input.GetKey (KeyCode.KeypadPlus) || Input.GetKey (KeyCode.Plus))) {

		}
		if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && (Input.GetKey (KeyCode.KeypadMinus) || Input.GetKey (KeyCode.Minus))) {

		}
	}
}
