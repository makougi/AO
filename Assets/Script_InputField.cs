using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Script_InputField : MonoBehaviour {

	public GameObject controller;
	private InputField input;
	bool gameOver;
	bool placeholderIsActive;

	// Use this for initialization
	void Start () {
		input = GetComponent<InputField> ();
		var se = new InputField.SubmitEvent ();
		se.AddListener (SubmitCommand);
		input.onEndEdit = se;
		ActivatePlaceholder ();
	}

	public void ActivatePlaceholder () {
		if (input.text == "") {
			input.text = "Enter text...";
			placeholderIsActive = true;
		}
	}

	public void DeactivatePlaceholder () {
		if (placeholderIsActive) {
			input.text = "";
			placeholderIsActive = false;
		}
	}

	private void SubmitCommand (string arg0) {
		if (controller != null) {
			if (!gameOver) {
				controller.GetComponent<Script_Parser> ().Parse (arg0);				
			}
			input.text = "";
			input.ActivateInputField ();			
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void setGameOver (bool b) {
		gameOver = b;
	}
}
