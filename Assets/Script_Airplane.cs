using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Script_Airplane : MonoBehaviour {

	public GameObject airplaneText;

	private GameObject controller;
	private GameObject chatText;
	private Transform otherTransform;
	private Script_AirplaneAltitude scriptAirplaneAltitude;
	private Script_AirplaneSpeed scriptAirplaneSpeed;
	private Script_AirplaneHeading scriptAirplaneHeading;
	private Script_AirplaneDots scriptAirplaneDots;
	private List<string> chatCommentsList;

	private bool overrideChatIdString;
	private bool newChatComments;
	private bool isInsideApproachArea;
	private bool landing;
	private bool readyForDestroy;
	private bool once;
	private bool clearedToLand;
	private bool abort;
	private int id;
	private int landingHeadingCorrectionFactor;
	private int runwayDirection;
	private float speedMapScaleFactor;
	private float delayedChatCommentTime;
	private float delayedCommandTime;
	private string iDColor;



	// Use this for initialization
	void Start () {
		iDColor = controller.GetComponent<Script_Controller> ().PickARandomColor ();
		chatCommentsList = new List<string> ();
		landingHeadingCorrectionFactor = 10;
		landing = false;
		isInsideApproachArea = false;
		abort = false;
		scriptAirplaneAltitude = GetComponent<Script_AirplaneAltitude> ();
		scriptAirplaneSpeed = GetComponent<Script_AirplaneSpeed> ();
		scriptAirplaneHeading = GetComponent<Script_AirplaneHeading> ();
		scriptAirplaneDots = GetComponent<Script_AirplaneDots> ();
//		speedMapScaleFactor = 0.0006f; previous
		speedMapScaleFactor = 0.000514444444f; // 1 kts = 0.000514444444 km / s

		airplaneText = Instantiate (airplaneText);
		if (id != 0) {
			airplaneText.GetComponent<Script_AirplaneText> ().SetAirplaneId (id);
			airplaneText.GetComponent<Script_AirplaneText> ().setController (controller);
		}
		overrideChatIdString = true;
		int nowHour = int.Parse (DateTime.Now.ToString ("HH"));
		if (nowHour >= 18 || nowHour < 3) {
			AddToChatList ("Tower, " + id + ", good evening");
		} else if (nowHour >= 3 && nowHour < 12) {
			AddToChatList ("Tower, " + id + ", good morning");
		} else {
			AddToChatList ("Tower, " + id + ", good afternoon");
		}

	}

	// Update is called once per frame
	void Update () {
		if (readyForDestroy) {
			if (Time.time > delayedCommandTime) {
				DestroyThisEntity ();
			}
		}
		if (newChatComments) {
			if (Time.time > delayedChatCommentTime) {
				SendChatCommentsToChat (chatCommentsList);
				newChatComments = false;
				chatCommentsList.Clear ();
			}			
		}
		if (abort) {
			if (Time.time > delayedCommandTime) {
				clearedToLand = false;
				landing = false;
				scriptAirplaneAltitude.Abort ();
				scriptAirplaneSpeed.Abort ();
				scriptAirplaneHeading.Abort ();
				abort = false;
			}
		}

		UpdatePosition ();
		UpdateAirplaneText ();
		if (landing) {
			land ();
//		} else if (CheckIfReadyToLand () && clearedToLand && delayedCommandTime == -1) {
//			delayedCommandTime = Time.time + UnityEngine.Random.Range (1.5f, 4f);

		} else if (clearedToLand && Time.time > delayedCommandTime && CheckIfReadyToLand ()) {
			scriptAirplaneAltitude.ActivateLandingMode ();
			scriptAirplaneSpeed.ActivateLandingMode ();
			landing = true;
			chatText.GetComponent<Script_ChatText> ().StartNewLine (iDColor);
			chatText.GetComponent<Script_ChatText> ().EnableBold ();
			chatText.GetComponent<Script_ChatText> ().AddText ("landing, " + id + ".");
			chatText.GetComponent<Script_ChatText> ().DisableBold ();
			chatText.GetComponent<Script_ChatText> ().EndLine ();			
		}
	}

	public void Abort () {
		abort = true;
		delayedCommandTime = Time.time + UnityEngine.Random.Range (1.5f, 3f);
	}

	public void GrantLandingClearance () {
		delayedCommandTime = Time.time + UnityEngine.Random.Range (1.5f, 4f);
		clearedToLand = true;
	}

	void SendChatCommentsToChat (List<string> chatComments) {
		chatText.GetComponent<Script_ChatText> ().StartNewLine (iDColor);
		chatText.GetComponent<Script_ChatText> ().EnableBold ();
		foreach (string s in chatComments) {
			chatText.GetComponent<Script_ChatText> ().AddText (s);
		}
		if (!overrideChatIdString) {
			chatText.GetComponent<Script_ChatText> ().AddComma ();
			chatText.GetComponent<Script_ChatText> ().AddText (id.ToString ());
		}
		overrideChatIdString = false;
		chatText.GetComponent<Script_ChatText> ().AddDot ();
		chatText.GetComponent<Script_ChatText> ().DisableBold ();
		chatText.GetComponent<Script_ChatText> ().EndLine ();
	}

	void OnTriggerEnter (Collider collider) {
		if (collider.gameObject.GetComponent<Script_Airplane> ()) {
			if (collider.gameObject.GetComponent<Script_AirplaneAltitude> ().GetAltitude () < GetComponent<Script_AirplaneAltitude> ().GetAltitude () + 900 && collider.gameObject.GetComponent<Script_AirplaneAltitude> ().GetAltitude () > GetComponent<Script_AirplaneAltitude> ().GetAltitude () - 900) {
				controller.GetComponent<Script_Controller> ().ProcessAirplanesTooClose (id);
			}
		}
	}

	public void setControllerAndChatTextGameObjects (GameObject contr, GameObject chatTxt) {
		controller = contr;
		chatText = chatTxt;
	}

	void land () {
		scriptAirplaneHeading.CommandHeadingWithoutDelay (runwayDirection - Mathf.RoundToInt (otherTransform.InverseTransformPoint (transform.position).x * landingHeadingCorrectionFactor));
		int runwayLength = 1;
		int glideSlopeLength = 9;
		if (scriptAirplaneSpeed.GetSpeed () <= 0 && scriptAirplaneAltitude.GetAltitude () == 0 && otherTransform.InverseTransformPoint (transform.position).y > -runwayLength) {
			if (!once) {
				chatText.GetComponent<Script_ChatText> ().StartNewLine (iDColor);
				chatText.GetComponent<Script_ChatText> ().EnableBold ();
				int nowHour = int.Parse (DateTime.Now.ToString ("HH"));
				if (nowHour >= 19) {
					chatText.GetComponent<Script_ChatText> ().AddText ("landing completed, " + id + ", good evening.");
				} else if (nowHour < 5) {
					chatText.GetComponent<Script_ChatText> ().AddText ("landing completed, " + id + ", good night.");
				} else if (nowHour >= 5 && nowHour < 9) {
					chatText.GetComponent<Script_ChatText> ().AddText ("landing completed, " + id + ", good morning.");
				} else {
					chatText.GetComponent<Script_ChatText> ().AddText ("landing completed, " + id + ", good day.");
				}
				chatText.GetComponent<Script_ChatText> ().DisableBold ();
				chatText.GetComponent<Script_ChatText> ().EndLine ();
				controller.GetComponent<Script_Controller> ().addLanded ();

				delayedCommandTime = Time.time + 3;
				once = true;
				readyForDestroy = true;
			}
		} else if (scriptAirplaneAltitude.GetAltitude () == 0 && otherTransform.InverseTransformPoint (transform.position).y > -runwayLength) {
			scriptAirplaneSpeed.ActivateBrakingMode ();
			scriptAirplaneSpeed.CommandSpeedWithoutDelay (0);
		} else if (otherTransform.InverseTransformPoint (transform.position).y > -(runwayLength + glideSlopeLength)) {
			float distanceFromRunway = -(otherTransform.InverseTransformPoint (transform.position).y + runwayLength);
			float glideSlopeRatio = 2000 / glideSlopeLength;
			if (scriptAirplaneAltitude.GetAltitude () > distanceFromRunway * glideSlopeRatio) {
				scriptAirplaneAltitude.CommandAltitudeWithoutDelay (distanceFromRunway * glideSlopeRatio);

			}
			scriptAirplaneSpeed.CommandSpeedWithoutDelay (160);
		} else {
			scriptAirplaneSpeed.CommandSpeedWithoutDelay (160);
		}
	}

	void DestroyThisEntity () {
		controller.GetComponent<Script_Controller> ().RemoveAirplane (id);
		Destroy (airplaneText.gameObject);
		scriptAirplaneDots.DestroyDots ();
		scriptAirplaneHeading.DestroyHeadingAssignedGameObject ();
		Destroy (this.gameObject);
	}

	public void setIsInsideApproachArea (bool b, Transform approachTransform) {
		isInsideApproachArea = b;
		runwayDirection = (int)approachTransform.eulerAngles.y;
		otherTransform = approachTransform;

	}

	public void SetUpValues (int airplaneId, int airplaneAltitude, int airplaneSpeed, int airplaneHeading) {
		id = airplaneId;
		GetComponent<Script_AirplaneAltitude> ().SetAltitude (airplaneAltitude);
		GetComponent<Script_AirplaneAltitude> ().CommandAltitude (airplaneAltitude);
		GetComponent<Script_AirplaneSpeed> ().SetSpeed (airplaneSpeed);
		GetComponent<Script_AirplaneSpeed> ().CommandSpeed (airplaneSpeed);
		GetComponent<Script_AirplaneHeading> ().SetHeading (airplaneHeading);
		GetComponent<Script_AirplaneHeading> ().SecondaryStart ();
		GetComponent<Script_AirplaneHeading> ().CommandHeading (airplaneHeading, 0);
		if (airplaneText.activeInHierarchy) {
			airplaneText.GetComponent<Script_AirplaneText> ().SetAirplaneId (id);
			airplaneText.GetComponent<Script_AirplaneText> ().setController (controller);
		}
	}

	public bool CheckIfReadyToLand () {
		if (isInsideApproachArea
		    && scriptAirplaneAltitude.GetAltitude () <= 2000
		    && scriptAirplaneSpeed.GetSpeed () <= 240
		    && (transform.eulerAngles.y > runwayDirection - 11 || transform.eulerAngles.y > -11 + 360)
		    && (transform.eulerAngles.y < runwayDirection + 11 || transform.eulerAngles.y < +11 - 360)) {
			return true;
		}
		return false;
	}

	void UpdatePosition () {
		transform.position += transform.forward * Time.deltaTime * scriptAirplaneSpeed.GetSpeed () * speedMapScaleFactor;
	}

	int AltitudeToFlightlevel (float altitude) {
		return (int)Math.Round (altitude * 0.01f);
	}

	void UpdateAirplaneText () {
		airplaneText.GetComponent<Script_AirplaneText> ().UpdatePosition (transform.position);
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneFlightlevel (AltitudeToFlightlevel (scriptAirplaneAltitude.GetAltitude ()));
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneFlightlevelAssigned (AltitudeToFlightlevel (scriptAirplaneAltitude.GetAltitudeAssigned ()));
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneHeading (scriptAirplaneHeading.GetHeading ());
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneSpeed ((int)scriptAirplaneSpeed.GetSpeed ());	
	}

	public void ActivateOutOfFuelMode () {
		controller.GetComponent<Script_Controller> ().ProcessOutOfFuel (id);
		scriptAirplaneAltitude.ActivateGlideMode ();
	}

	public int GetId () {
		return id;
	}

	public void AddToChatList (string s) {
		if (chatCommentsList.Count == 0) {
			s = char.ToUpper (s [0]) + s.Substring (1);
		} 
		chatCommentsList.Add (s);			
		delayedChatCommentTime = Time.time + UnityEngine.Random.Range (1.5f, 8f);
		newChatComments = true;
	}

	public void OverrideChatList (string s) {
		chatCommentsList.Clear ();
		s = char.ToUpper (s [0]) + s.Substring (1);
		chatCommentsList.Add (s);			
		delayedChatCommentTime = Time.time + UnityEngine.Random.Range (1.5f, 8f);
		newChatComments = true;
	}

	public string GetIDColor () {
		return iDColor;
	}

	public GameObject getAirplaneText () {
		return airplaneText;
	}

	public GameObject getController () {
		return controller;
	}
}