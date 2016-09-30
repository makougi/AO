using UnityEngine;
using System.Collections;

public class Script_PlusButton : MonoBehaviour {

	public GameObject MainCamera;

	private bool buttonDown;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (buttonDown) {
			MainCamera.GetComponent<Script_MainCamera> ().ZoomIn ();
		}
	}

	public void OnButtonDown () {
		buttonDown = true;
	}

	public void OnButtonUp () {
		buttonDown = false;
	}
}
