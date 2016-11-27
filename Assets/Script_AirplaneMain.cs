using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Script_AirplaneMain : MonoBehaviour {

	public GameObject airplaneText;
	public GameObject airplaneSprite;

	private GameObject controller;
	private Script_AirplaneAltitude airplaneAltitudeScript;
	private Script_AirplaneSpeed airplaneSpeedScript;
	private Script_AirplaneHeading airplaneHeadingScript;
	private Script_AirplaneDots airplaneDotsScript;
	private Script_AirplaneLanding airplaneLandingScript;
	private Script_AirplaneChat airplaneChatScript;

	private bool readyForDestroy;
	private bool abort;
	private int id;
	private int timeCounter;
	private float speedMapScaleFactor;
	private float delayedCommandTime;
	private bool takeoff;
	private int takeoffHeading;
	private bool standby;
	private bool clearedToTakeoff;
	private string displayName;

	// Use this for initialization
	void Start () {
		airplaneSprite = Instantiate (airplaneSprite);
		airplaneAltitudeScript = GetComponent<Script_AirplaneAltitude> ();
		airplaneSpeedScript = GetComponent<Script_AirplaneSpeed> ();
		airplaneHeadingScript = GetComponent<Script_AirplaneHeading> ();
		airplaneDotsScript = GetComponent<Script_AirplaneDots> ();
		airplaneLandingScript = GetComponent<Script_AirplaneLanding> ();
		airplaneChatScript = GetComponent<Script_AirplaneChat> ();
		speedMapScaleFactor = 0.000514444444f; // 1 kts = 0.000514444444 km / s

		airplaneText = Instantiate (airplaneText);
		if (standby) {
			airplaneText.SetActive (false);
		} else {
			airplaneText.SetActive (true);
		}
		if (id != 0) {
			airplaneText.GetComponent<Script_AirplaneText> ().SetAirplaneId (id);
			airplaneText.GetComponent<Script_AirplaneText> ().setController (controller);
		}
		while (timeCounter < Time.time) {
			timeCounter += 3;
		}
	}

	// Update is called once per frame
	void Update () {
		if (takeoff) {
			if (clearedToTakeoff && Time.time > delayedCommandTime) {
				standby = false;
			}
			if (standby) {
				airplaneSpeedScript.SetSpeed (0);
				airplaneAltitudeScript.SetAltitude (0);
				airplaneHeadingScript.SetHeading (takeoffHeading);
			} else {
				GetComponent<Collider> ().enabled = true;
				if (displayName == "radar") {
					airplaneDotsScript.SetActive (true);
					airplaneText.SetActive (true);
				}
				if (airplaneSpeedScript.GetSpeed () < 145) {
					airplaneAltitudeScript.SetAltitude (0);
					airplaneHeadingScript.SetHeading (takeoffHeading);
				} else {
					airplaneAltitudeScript.SetAltitudeMin (1000);
					airplaneAltitudeScript.CommandAltitudeWithoutDelay (airplaneAltitudeScript.GetAltitudeAssigned ());
					airplaneSpeedScript.SetSpeedMin (140);
					takeoff = false;
				}
			}
		}
		if (readyForDestroy) {
			if (Time.time > delayedCommandTime) {
				DestroyThisEntity ();
			}
		}

		if (abort) {
			if (Time.time > delayedCommandTime) {
				airplaneLandingScript.Abort ();
				airplaneAltitudeScript.Abort (takeoff);
				airplaneSpeedScript.Abort (takeoff);
				airplaneHeadingScript.Abort ();
				abort = false;
			}
		}

		UpdatePosition ();
		if (Time.time > timeCounter) {
			UpdateAirplaneUIElements ();
			timeCounter += 3;
		}
		airplaneSprite.transform.position = transform.position;
		airplaneSprite.transform.eulerAngles = new Vector3 (90, 0, transform.eulerAngles.y * -1);
	}

	void OnTriggerEnter (Collider collider) {
		Script_AirplaneMain otherAirplaneMainScript = collider.gameObject.GetComponent<Script_AirplaneMain> ();
		if (otherAirplaneMainScript) {
			if (otherAirplaneMainScript.GetAltitude () < GetAltitude () + 900 && otherAirplaneMainScript.GetAltitude () > GetAltitude () - 900) {
				controller.GetComponent<Script_Controller> ().ProcessAirplanesTooClose (id);
			}
		}
	}

	public void Construct (int airplaneId, int airplaneAltitude, int airplaneSpeed, int airplaneHeading, bool airplaneTakeoff, string dispName, GameObject controllerGameObject, Script_ChatText chatTextScript, string idColr) {
		SetupVariablesOfThisScripts (dispName, airplaneId, airplaneTakeoff, airplaneHeading, controllerGameObject);
		ConstructOtherScriptsOfThisGameObject (airplaneAltitude, airplaneSpeed, airplaneHeading, idColr, chatTextScript);
		ConstructAirplaneTextGameObject (id, controllerGameObject);
		transform.eulerAngles = new Vector3 (0, airplaneHeading, 0);
		if (airplaneTakeoff) {
			RunIfConstructTakeoffTrue (airplaneHeading);
		} else {
			RunIfConstructTakeoffFalse (dispName);
		}
	}

	public void ChangeDisplayName (string dn) {
		displayName = dn;
		if (dn == "radar") {
			airplaneSprite.SetActive (false);
			if (standby) {
				airplaneDotsScript.SetActive (false);
				airplaneText.SetActive (false);
			} else {
				airplaneDotsScript.SetActive (true);
				airplaneText.SetActive (true);
			}

		} else if (dn == "satellite") {
			airplaneSprite.SetActive (true);
			airplaneDotsScript.SetActive (false);
			airplaneText.SetActive (false);
		}
	}

	public void UpdateAirplaneUIElementUIPositions () {
		if (airplaneDotsScript) {
			airplaneDotsScript.UpdateUIPosition ();
		}
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateUIPosition (GetComponent<Script_AirplaneDots> ().getAirplaneMainDotPosition ());
	}

	public void Abort () {
		abort = true;
		delayedCommandTime = Time.time + UnityEngine.Random.Range (1.5f, 3f);
	}

	public void GrantLandingClearance (string clearedApprchId) {
		airplaneLandingScript.GrantLandingClearance (clearedApprchId);
	}

	public void ControlApproachAreaEnterAndExit (bool enter, Transform approachTransform, string id) {
		airplaneLandingScript.ControlApproachAreaEnterAndExit (enter, approachTransform, id);
	}

	public bool CheckIfReadyToLand () {
		return airplaneLandingScript.CheckIfReadyToLand ();
	}

	public void ActivateOutOfFuelMode () {
		controller.GetComponent<Script_Controller> ().ProcessOutOfFuel (id);
		airplaneAltitudeScript.ActivateGlideMode ();
	}

	public void GrantTakeoffClearance () {
		delayedCommandTime = Time.time + UnityEngine.Random.Range (8, 16);
		clearedToTakeoff = true;
	}

	public void ExecuteLandingMessage () {
		airplaneChatScript.ExecuteLandingMessage ();
	}

	public void CommandHeadingWithoutDelay (int hdg) {
		airplaneHeadingScript.CommandHeadingWithoutDelay (hdg);
	}

	public void ExecuteLandingCompletedMessage () {
		airplaneChatScript.ExecuteLandingCompletedMessage ();
	}

	public void AddLanded () {
		controller.GetComponent<Script_Controller> ().addLanded ();
	}

	public void ActivateReadyForDestroy () {
		readyForDestroy = true;
		delayedCommandTime = Time.time + 3;
	}

	public void ActivateBrakingMode () {
		airplaneSpeedScript.ActivateBrakingMode ();
	}

	public void CommandAltitudeWithoutDelay (float alt) {
		airplaneAltitudeScript.CommandAltitudeWithoutDelay (alt);
	}

	public void CommandSpeedWithoutDelay (int spd) {
		airplaneSpeedScript.CommandSpeedWithoutDelay (spd);
	}

	public string GetId () {
		return id.ToString ();
	}

	public GameObject GetAirplaneText () {
		return airplaneText;
	}

	public GameObject GetController () {
		return controller;
	}

	public bool GetTakeoff () {
		return takeoff;
	}

	public bool GetStandby () {
		return standby;
	}

	public float GetAltitude () {
		return airplaneAltitudeScript.GetAltitude ();
	}

	public float GetSpeed () {
		return airplaneSpeedScript.GetSpeed ();
	}

	public void SetAltitudeMin (int alt) {
		airplaneAltitudeScript.SetAltitudeMin (alt);
	}

	public void SetSpeedMin (int spd) {
		airplaneSpeedScript.SetSpeedMin (spd);
	}

	private void UpdatePosition () {
		transform.position += transform.forward * Time.deltaTime * airplaneSpeedScript.GetSpeed () * speedMapScaleFactor;
	}

	private void UpdateAirplaneUIElements () {
		UpdateAirplaneText ();
		airplaneDotsScript.UpdateWorldPosition ();
		UpdateAirplaneUIElementUIPositions ();
	}

	private void UpdateAirplaneText () {
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneText (AltitudeToFlightlevel (airplaneAltitudeScript.GetAltitude ()), AltitudeToFlightlevel (airplaneAltitudeScript.GetAltitudeAssigned ()), airplaneHeadingScript.GetHeading (), (int)airplaneSpeedScript.GetSpeed ());
	}

	private void DestroyThisEntity () {
		controller.GetComponent<Script_Controller> ().RemoveAirplane (id);
		airplaneText.GetComponent<Script_AirplaneText> ().DestoryLineImage ();
		Destroy (airplaneText.gameObject);
		airplaneDotsScript.DestroyDots ();
		airplaneHeadingScript.DestroyHeadingAssignedGameObject ();
		Destroy (airplaneSprite.gameObject);
		Destroy (this.gameObject);
	}

	private int AltitudeToFlightlevel (float altitude) {
		return (int)Math.Round (altitude * 0.01f);
	}

	private void ConstructAirplaneTextGameObject (int id, GameObject controllerGameObject) {
		if (airplaneText.activeInHierarchy) {
			airplaneText.GetComponent<Script_AirplaneText> ().SetAirplaneId (id);
			airplaneText.GetComponent<Script_AirplaneText> ().setController (controllerGameObject);
		}
	}

	private void ConstructOtherScriptsOfThisGameObject (float airplaneAltitude, int airplaneSpeed, int airplaneHeading, string idColr, Script_ChatText chatTextScript) {
		GetComponent<Script_AirplaneAltitude> ().Construct (airplaneAltitude);
		GetComponent<Script_AirplaneSpeed> ().Construct (airplaneSpeed);
		GetComponent<Script_AirplaneHeading> ().Construct (airplaneHeading);
		GetComponent<Script_AirplaneChat> ().Construct (idColr, chatTextScript);
	}

	private void SetupVariablesOfThisScripts (string dispName, int airplaneId, bool airplaneTakeoff, int airplaneHeading, GameObject controllerGameObject) {
		displayName = dispName;
		id = airplaneId;
		standby = airplaneTakeoff;
		takeoff = airplaneTakeoff;
		takeoffHeading = airplaneHeading;
		controller = controllerGameObject;
	}

	private void RunIfConstructTakeoffTrue (int airplaneHeading) {
		GetComponent<Script_AirplaneDots> ().SetActive (false);
		GetComponent<Collider> ().enabled = false;
		GetComponent<Script_AirplaneAltitude> ().SetAltitudeMin (0);
		GetComponent<Script_AirplaneSpeed> ().SetSpeedMin (0);
	}

	private void RunIfConstructTakeoffFalse (string dispName) {
		if (dispName == "radar") {
			GetComponent<Script_AirplaneDots> ().SetActive (true);
		}
		GetComponent<Collider> ().enabled = true;
		GetComponent<Script_AirplaneAltitude> ().SetAltitudeMin (1000);
		GetComponent<Script_AirplaneSpeed> ().SetSpeedMin (140);
	}
}