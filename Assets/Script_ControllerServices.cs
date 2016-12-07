using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Script_ControllerServices : MonoBehaviour {

	public GameObject clock;
	public GameObject inputField;

	private List<int> airplanesTooClose = new List<int> ();
	private int landed;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void ProcessOutOfFuel (int airplaneId, Script_ChatText chatTextScript) {
		inputField.GetComponent<Script_ChatInputField> ().setGameOver (true);
		chatTextScript.StartNewLine ("<color=red>");
		chatTextScript.EnableBold ();
		chatTextScript.AddText ("Airplane " + airplaneId + " out of fuel.");
		chatTextScript.AddText ("YOU ARE FIRED");
		chatTextScript.DisableBold ();
		chatTextScript.EndLine ();
	}

	public void ProcessAirplanesTooClose (int airplaneId, Script_ChatText chatTextScript) {
		airplanesTooClose.Add (airplaneId);
		if (airplanesTooClose.Count == 2) {
			inputField.GetComponent<Script_ChatInputField> ().setGameOver (true);
			chatTextScript.StartNewLine ("<color=red>");
			chatTextScript.EnableBold ();
			chatTextScript.AddText ("Airplanes " + airplanesTooClose[0] + " and " + airplanesTooClose[1] + " too close.");
			chatTextScript.AddText ("YOU ARE FIRED");
			chatTextScript.DisableBold ();
			chatTextScript.EndLine ();
		}
	}

	public string CreateThreeLetterId () {
		string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		return "" + letters[UnityEngine.Random.Range (0, letters.Length)] + letters[UnityEngine.Random.Range (0, letters.Length)] + letters[UnityEngine.Random.Range (0, letters.Length)];
	}

	public void addLanded () {
		landed++;
		clock.GetComponent<Script_Clock> ().SetLanded (landed);
	}

	public void RemoveAirplane (int i, Script_Schedule scheduleScript) {
		Script_ControllerMain controllerScript = GetComponent<Script_ControllerMain> ();
		controllerScript.GetAirplanesList ().Remove (controllerScript.GetAirplanesDictionary ()[i]);
		controllerScript.GetAirplanesDictionary ().Remove (i);
		scheduleScript.RemoveIdFromActiveIds (i);
	}
}
