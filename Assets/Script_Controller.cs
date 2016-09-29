using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Script_Controller : MonoBehaviour {

	public GameObject clock;
	public GameObject schedule;
	public GameObject gameObjectChatText;
	public GameObject inputField;
	public GameObject airplane;
	public GameObject approach;
	public GameObject genericText;
	public GameObject ground;
	public GameObject mainCamera;
	public GameObject worldPanel;
	Dictionary<int, GameObject> airplanesDictionary;
	List<GameObject> airplanesList;
	Dictionary<string, Vector3> beaconsDictionary;
	List<GameObject> genericTexts;
	List<int> airplanesTooClose;
	Script_ScheduledFlight flight;
	Script_Colors colors;
	bool colorsIsInstantiated;
	int landed;
	int counter;
	bool airplaneTextsOffset;


	// Use this for initialization
	void Start () {
		colors = ScriptableObject.CreateInstance<Script_Colors> ();
		colorsIsInstantiated = true;
		airplanesTooClose = new List<int> ();
		beaconsDictionary = new Dictionary<string, Vector3> ();
		genericTexts = new List<GameObject> ();
		CreateBeacons ();
		landed = 0;
		QualitySettings.antiAliasing = 0;
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
		approach = Instantiate (approach);
		approach.SetActive (true);
		counter = 0;
		airplanesDictionary = new Dictionary<int, GameObject> ();
		airplanesList = new List<GameObject> ();
		GetComponent<Script_Parser> ().SetAirplanesDictionary (airplanesDictionary);
		GetComponent<Script_Parser> ().SetBeaconsDictionary (beaconsDictionary);
		switchDisplay ("radar");

		CreateTestFlight ();
	}
	
	// Update is called once per frame
	void Update () {
		counter++;
		if (counter > 10) {
			CheckFlight ();
			counter = 0;
		}
	}

	public void switchDisplay (string displayName) {
		if (displayName == "radar") {
			ground.SetActive (false);
			approach.GetComponent<SpriteRenderer> ().enabled = true;
			approach.GetComponent<Script_Approach> ().approachText.SetActive (true);
			foreach (GameObject go in genericTexts) {
				go.SetActive (true);
			}
			mainCamera.GetComponent<Camera> ().orthographic = true;
		}
		if (displayName == "satellite") {
			ground.SetActive (true);
			approach.GetComponent<SpriteRenderer> ().enabled = false;
			approach.GetComponent<Script_Approach> ().approachText.SetActive (false);
			foreach (GameObject go in genericTexts) {
				go.SetActive (false);
			}
			mainCamera.GetComponent<Camera> ().orthographic = false;
		}
	}

	public void toggleAirplaneTextsOffset (bool active) {
		if (active) {
			airplaneTextsOffset = true;
			foreach (GameObject ap in airplanesList) {
				ap.GetComponent<Script_Airplane> ().getAirplaneText ().GetComponent<Script_AirplaneText> ().RandomizeOffset (true);
			}
		} else {
			airplaneTextsOffset = false;
			foreach (GameObject ap in airplanesList) {
				ap.GetComponent<Script_Airplane> ().getAirplaneText ().GetComponent<Script_AirplaneText> ().RandomizeOffset (false);
			}
		}
		
	}

	public void ProcessOutOfFuel (int airplaneId) {
		inputField.GetComponent<Script_InputField> ().setGameOver (true);
		gameObjectChatText.GetComponent<Script_ChatText> ().StartNewLine ("<color=red>");
		gameObjectChatText.GetComponent<Script_ChatText> ().EnableBold ();
		gameObjectChatText.GetComponent<Script_ChatText> ().AddText ("Airplane " + airplaneId + " out of fuel.");
		gameObjectChatText.GetComponent<Script_ChatText> ().AddText ("YOU ARE FIRED");
		gameObjectChatText.GetComponent<Script_ChatText> ().DisableBold ();
		gameObjectChatText.GetComponent<Script_ChatText> ().EndLine ();
	}

	public void ProcessAirplanesTooClose (int airplaneId) {
		airplanesTooClose.Add (airplaneId);
		if (airplanesTooClose.Count == 2) {
			inputField.GetComponent<Script_InputField> ().setGameOver (true);
			gameObjectChatText.GetComponent<Script_ChatText> ().StartNewLine ("<color=red>");
			gameObjectChatText.GetComponent<Script_ChatText> ().EnableBold ();
			gameObjectChatText.GetComponent<Script_ChatText> ().AddText ("Airplanes " + airplanesTooClose [0] + " and " + airplanesTooClose [1] + " too close.");
			gameObjectChatText.GetComponent<Script_ChatText> ().AddText ("YOU ARE FIRED");
			gameObjectChatText.GetComponent<Script_ChatText> ().DisableBold ();
			gameObjectChatText.GetComponent<Script_ChatText> ().EndLine ();
		}
	}

	public Dictionary<string, Vector3> GetBeaconsDictionary () {
		return beaconsDictionary;
	}

	public Dictionary<int, GameObject> GetAirplanesDictionary () {
		return airplanesDictionary;
	}

	void CreateBeacons () {
		string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		int amount = 20;
		while (amount > 0) {
			amount--;
			while (true) {
				string beaconId = "" + letters [UnityEngine.Random.Range (0, letters.Length)] + letters [UnityEngine.Random.Range (0, letters.Length)] + letters [UnityEngine.Random.Range (0, letters.Length)];				
				if (!beaconsDictionary.ContainsKey (beaconId)) {
					Vector3 beaconPosition = new Vector3 (UnityEngine.Random.Range (-80f, 80f), 0, UnityEngine.Random.Range (-40f, 40f));
					beaconsDictionary.Add (beaconId, beaconPosition);
					GameObject genericT = Instantiate (genericText);
					genericT.GetComponent<Text> ().text = beaconId;
//					genericText.transform.position = new Vector3 (beaconPosition.x, beaconPosition.z, beaconPosition.y);
//					genericText.transform.parent = worldPanel.transform;
					genericT.transform.SetParent (worldPanel.transform);
					genericTexts.Add (genericT);
					genericT.GetComponent<Script_GenericText> ().SetBeaconPosition (beaconPosition);

					break;
				}
			}
		}
	}

	public void addLanded () {
		landed++;
		clock.GetComponent<Script_Clock> ().SetLanded (landed);
	}

	public void RemoveAirplane (int i) {
		airplanesList.Remove (airplanesDictionary [i]);
		airplanesDictionary.Remove (i);
		schedule.GetComponent<Script_Schedule> ().RemoveIdFromActiveIds (i);
	}

	void CreateTestFlight () {
		GameObject ap = Instantiate (airplane);
		ap.transform.position = new Vector3 (0, 0, 0);
		ap.GetComponent<Script_Airplane> ().setControllerAndChatTextGameObjects (this.gameObject, gameObjectChatText);
		ap.GetComponent<Script_AirplaneSpeed> ().setChatText (gameObjectChatText);
		ap.GetComponent<Script_Airplane> ().SetUpValues (1111, 2000, 240, 0);
		ap.transform.eulerAngles = new Vector3 (0, 0, 0);
		airplanesDictionary.Add (1111, ap);
		airplanesList.Add (ap);
		schedule.GetComponent<Script_Schedule> ().AddToActiveIds (1111);
	}

	void CheckFlight () {
		flight = schedule.GetComponent<Script_Schedule> ().dequeueFlightIfReady ();
		if (flight) {
			GameObject ap = Instantiate (airplane);
			int tempValueDirection = 0;
			int tempValuePosition = 40;
			if (flight.getEntryPoint () == "A") {
				ap.transform.position = new Vector3 (tempValuePosition, 0, -tempValuePosition);
				tempValueDirection = 0;
			} else if (flight.getEntryPoint () == "B") {
				tempValueDirection = 90;
				ap.transform.position = new Vector3 (-tempValuePosition, 0, -tempValuePosition);
			} else if (flight.getEntryPoint () == "C") {
				tempValueDirection = 180;
				ap.transform.position = new Vector3 (-tempValuePosition, 0, tempValuePosition);
			} else if (flight.getEntryPoint () == "D") {
				tempValueDirection = 270;
				ap.transform.position = new Vector3 (tempValuePosition, 0, tempValuePosition);
			} else if (flight.getEntryPoint () == "E") {
				tempValueDirection = 45;
				ap.transform.position = new Vector3 (-tempValuePosition, 0, -tempValuePosition);
			}
			ap.GetComponent<Script_Airplane> ().setControllerAndChatTextGameObjects (this.gameObject, gameObjectChatText);
			ap.GetComponent<Script_Airplane> ().SetUpValues (flight.getId (), flight.getAltitude (), flight.getSpeed (), tempValueDirection);
			ap.GetComponent<Script_AirplaneSpeed> ().setChatText (gameObjectChatText);
			ap.transform.eulerAngles = new Vector3 (0, tempValueDirection, 0);
			airplanesDictionary.Add (flight.getId (), ap);
			airplanesList.Add (ap);
		}
	}

	public string PickARandomColor () {
		if (!colorsIsInstantiated) {
			CreateColorsInstance ();
		}
		return colors.PickARandomColor ();
	}

	void CreateColorsInstance () {
		colors = ScriptableObject.CreateInstance<Script_Colors> ();
	}

	public bool getAirplaneTextsOffset () {
		return airplaneTextsOffset;
	}
}
