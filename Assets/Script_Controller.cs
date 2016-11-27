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
	public GameObject beacon;
	public GameObject ground;
	public GameObject ground1;
	public GameObject ground2;
	public GameObject ground3;
	public GameObject ground4;
	public GameObject ground5;
	public GameObject ground6;
	public GameObject ground7;
	public GameObject ground8;
	public GameObject mainCamera;
	public GameObject dIPanel;

	private Script_ScheduledFlight flight;
	private Dictionary<int, GameObject> airplanesDictionary;
	private List<GameObject> airplanesList;
	private Dictionary<string, GameObject> beaconsDictionary;
	private List<GameObject> beaconsList;
	private Dictionary<string, GameObject> approachesDictionary;
	private List<GameObject> approachesList;
	private List<int> airplanesTooClose;
	private bool airplaneTextsOffset;
	private int landed;
	private int counter;
	private List<List<GameObject>> targets;
	private string activeDisplayName;

	// Use this for initialization
	void Start () {
		approachesDictionary = new Dictionary<string, GameObject> ();
		approachesList = new List<GameObject> ();
		schedule.GetComponent<Script_Schedule> ().SetApproaches (approachesList);
		airplanesTooClose = new List<int> ();
		beaconsDictionary = new Dictionary<string, GameObject> ();
		beaconsList = new List<GameObject> ();
		CreateBeacons ();
		landed = 0;
		QualitySettings.antiAliasing = 0;
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
		CreateApproaches ();
		counter = 0;
		airplanesDictionary = new Dictionary<int, GameObject> ();
		airplanesList = new List<GameObject> ();
		GetComponent<Script_ChatParser> ().SetAirplanesDictionary (airplanesDictionary);
		GetComponent<Script_ChatParser> ().SetAirplanesList (airplanesList);
		GetComponent<Script_ChatParser> ().SetBeaconsDictionary (beaconsDictionary);
		GetComponent<Script_ChatParser> ().SetApproachesDictionary (approachesDictionary);
		switchDisplay ("radar");
		targets = new List<List<GameObject>> ();
		targets.Add (beaconsList);
		targets.Add (airplanesList);


		CreateTestFlight ();

	}

	// Update is called once per frame
	void Update () {
		if (Time.time < 1) {
			UpdateUIElementPositions ();
		}
		counter++;
		if (counter > 10) {
			CheckFlight ();
			counter = 0;
		}
	}

	private void CreateApproaches () {
		int approachesToBeCreated = 2;
		foreach (GameObject bcn in beaconsList) {
			GameObject aprch = Instantiate (approach);
			int direction = UnityEngine.Random.Range (0, 36);
			string approachId;
			if (direction < 10) {
				approachId = bcn.GetComponent<Script_Beacon> ().GetId () + "0" + direction;
			} else {
				approachId = bcn.GetComponent<Script_Beacon> ().GetId () + direction;
			}
			aprch.GetComponent<Script_Approach> ().SetupDirection (direction * 10);
			aprch.GetComponent<Script_Approach> ().SetId (approachId);
			aprch.GetComponent<Script_Approach> ().SetupPosition (bcn.GetComponent<Script_Beacon> ().GetWorldPosition ());
			aprch.GetComponent<Script_Approach> ().RunSecondaryStart (this.gameObject);
			aprch.SetActive (true);
			approachesDictionary.Add (approachId, aprch);
			approachesList.Add (aprch);
			approachesToBeCreated--;
			if (approachesToBeCreated == 0) {
				return;
			}
		}
	}

	public void UpdateUIElementPositions () {
		foreach (GameObject go in beaconsList) {
			go.GetComponent<Script_Beacon> ().UpdateBeaconPosition ();
		}
		foreach (GameObject go in airplanesList) {
			go.GetComponent<Script_AirplaneMain> ().UpdateAirplaneUIElementUIPositions ();
		}
		foreach (GameObject go in approachesList) {
			go.GetComponent<Script_Approach> ().UpdateUIPosition ();
		}
	}

	public void switchDisplay (string displayName) {
		activeDisplayName = displayName;
		foreach (GameObject go in airplanesList) {
			go.GetComponent<Script_AirplaneMain> ().ChangeDisplayName (displayName);
		}
		if (displayName == "radar") {
			ground.SetActive (false);
			foreach (GameObject go in approachesList) {
				go.GetComponent<SpriteRenderer> ().enabled = true;
				go.GetComponent<Script_Approach> ().approachText.SetActive (true);
			}
			foreach (GameObject go in beaconsList) {
				go.SetActive (true);
			}
			mainCamera.GetComponent<Camera> ().orthographic = true;
			UpdateUIElementPositions ();
		}
		if (displayName == "satellite") {
			ground.SetActive (true);
			foreach (GameObject go in approachesList) {
				go.GetComponent<SpriteRenderer> ().enabled = false;
				go.GetComponent<Script_Approach> ().approachText.SetActive (false);
			}
			foreach (GameObject go in beaconsList) {
				go.SetActive (false);
			}
			mainCamera.GetComponent<Camera> ().orthographic = false;
			UpdateUIElementPositions ();
		}
	}

	public void toggleAirplaneTextsOffset (bool active) {
		if (active) {
			airplaneTextsOffset = true;
			foreach (GameObject ap in airplanesList) {
				ap.GetComponent<Script_AirplaneMain> ().GetAirplaneText ().GetComponent<Script_AirplaneText> ().RandomizeOffset (true);
			}
		} else {
			airplaneTextsOffset = false;
			foreach (GameObject ap in airplanesList) {
				ap.GetComponent<Script_AirplaneMain> ().GetAirplaneText ().GetComponent<Script_AirplaneText> ().RandomizeOffset (false);
			}
		}
	}

	public void ProcessOutOfFuel (int airplaneId) {
		inputField.GetComponent<Script_ChatInputField> ().setGameOver (true);
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
			inputField.GetComponent<Script_ChatInputField> ().setGameOver (true);
			gameObjectChatText.GetComponent<Script_ChatText> ().StartNewLine ("<color=red>");
			gameObjectChatText.GetComponent<Script_ChatText> ().EnableBold ();
			gameObjectChatText.GetComponent<Script_ChatText> ().AddText ("Airplanes " + airplanesTooClose[0] + " and " + airplanesTooClose[1] + " too close.");
			gameObjectChatText.GetComponent<Script_ChatText> ().AddText ("YOU ARE FIRED");
			gameObjectChatText.GetComponent<Script_ChatText> ().DisableBold ();
			gameObjectChatText.GetComponent<Script_ChatText> ().EndLine ();
		}
	}

	public Dictionary<string, GameObject> GetBeaconsDictionary () {
		return beaconsDictionary;
	}

	public Dictionary<int, GameObject> GetAirplanesDictionary () {
		return airplanesDictionary;
	}

	private string CreateThreeLetterId () {
		string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		return "" + letters[UnityEngine.Random.Range (0, letters.Length)] + letters[UnityEngine.Random.Range (0, letters.Length)] + letters[UnityEngine.Random.Range (0, letters.Length)];
	}

	private Vector3 CreateBeaconPosition (List<GameObject> beacons) {
		Vector3 beaconPosition = new Vector3 (0, 0, 0);
		while (beaconPosition == new Vector3 (0, 0, 0)) {
			beaconPosition = new Vector3 (UnityEngine.Random.Range (-100f, 100f), 0, UnityEngine.Random.Range (-100f, 100f));
			foreach (GameObject go in beacons) {
				if (Vector3.Distance (beaconPosition, go.GetComponent<Script_Beacon> ().GetWorldPosition ()) < 10) {
					beaconPosition = new Vector3 (0, 0, 0);
				}
			}
		}
		return beaconPosition;
	}

	void CreateBeacons () {
		int amount = 20;
		while (amount > 0) {
			amount--;
			while (true) {
				string beaconId = CreateThreeLetterId ();
				if (!beaconsDictionary.ContainsKey (beaconId)) {
					Vector3 beaconPosition = CreateBeaconPosition (beaconsList);
					GameObject bcn = Instantiate (beacon);
					beaconsDictionary.Add (beaconId, bcn);
					beaconsList.Add (bcn);
					bcn.GetComponent<Text> ().text = beaconId;
					bcn.transform.SetParent (dIPanel.transform);
					bcn.GetComponent<Script_Beacon> ().SetBeaconPosition (beaconPosition);
					bcn.GetComponent<Script_Beacon> ().UpdateBeaconPosition ();
					bcn.GetComponent<Script_Beacon> ().SetId (beaconId);
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
		airplanesList.Remove (airplanesDictionary[i]);
		airplanesDictionary.Remove (i);
		schedule.GetComponent<Script_Schedule> ().RemoveIdFromActiveIds (i);
	}

	void CreateTestFlight () {
		GameObject ap = Instantiate (airplane);
		ap.transform.position = new Vector3 (0, 0, 0);
		ap.GetComponent<Script_AirplaneMain> ().Construct (1111, 2000, 240, 0, false, activeDisplayName, this.gameObject, gameObjectChatText.GetComponent<Script_ChatText> (), GetComponent<Script_ColorsInterface> ().PickARandomColor ());
		ap.transform.eulerAngles = new Vector3 (0, 0, 0);
		airplanesDictionary.Add (1111, ap);
		airplanesList.Add (ap);
		schedule.GetComponent<Script_Schedule> ().AddToActiveIds (1111);
	}

	void CheckFlight () {
		flight = schedule.GetComponent<Script_Schedule> ().dequeueFlightIfReady ();
		if (flight) {
			GameObject ap = Instantiate (airplane);
			ap.transform.position = new Vector3 (flight.GetEntrypointPosition ().x, 0, flight.GetEntrypointPosition ().y);
			ap.GetComponent<Script_AirplaneMain> ().Construct (flight.GetId (), flight.GetAltitude (), flight.GetSpeed (), flight.GetHeading (), flight.GetTakeoff (), activeDisplayName, this.gameObject, gameObjectChatText.GetComponent<Script_ChatText> (), GetComponent<Script_ColorsInterface> ().PickARandomColor ());
			airplanesDictionary.Add (flight.GetId (), ap);
			airplanesList.Add (ap);
		}
	}

	public bool GetAirplaneTextsOffset () {
		return airplaneTextsOffset;
	}

	public GameObject GetDIPanel () {
		return dIPanel;
	}

	public bool CheckIfDIPanelIsActive () {
		return dIPanel.activeSelf;
	}

	public List<List<GameObject>> GetTargets () {
		return targets;
	}
}
