using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Script_Airplane : MonoBehaviour {

	public GameObject airplaneText;
	public GameObject airplaneSprite;

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
	private int timeCounter;
	private float speedMapScaleFactor;
	private float delayedChatCommentTime;
	private float delayedCommandTime;
	private string iDColor;
	private bool takeoff;
	private int takeoffHeading;
	private bool standby;
	private bool clearedToTakeoff;
	private string displayName;
	private string approachId;
	private string clearedApproachId;

	// Use this for initialization
	void Start () {
		airplaneSprite = Instantiate (airplaneSprite);
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
		if (standby) {
			airplaneText.SetActive (false);
		} else {
			airplaneText.SetActive (true);
		}
		if (id != 0) {
			airplaneText.GetComponent<Script_AirplaneText> ().SetAirplaneId (id);
			airplaneText.GetComponent<Script_AirplaneText> ().setController (controller);
		}
		overrideChatIdString = true;
		string request = "";
		if (standby) {
			request = ", requesting takeoff";
		}
		int nowHour = int.Parse (DateTime.Now.ToString ("HH"));
		if (nowHour >= 18 || nowHour < 3) {
			AddToChatList ("Tower, " + id + ", good evening" + request);
		} else if (nowHour >= 3 && nowHour < 12) {
			AddToChatList ("Tower, " + id + ", good morning" + request);
		} else {
			AddToChatList ("Tower, " + id + ", good afternoon" + request);
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
				scriptAirplaneSpeed.SetSpeed (0);
				scriptAirplaneAltitude.SetAltitude (0);
				scriptAirplaneHeading.SetHeading (takeoffHeading);
			} else {
				GetComponent<Collider> ().enabled = true;
				if (displayName == "radar") {
					scriptAirplaneDots.SetActive (true);
					airplaneText.SetActive (true);
				}
				if (scriptAirplaneSpeed.GetSpeed () < 145) {
					scriptAirplaneAltitude.SetAltitude (0);
					scriptAirplaneHeading.SetHeading (takeoffHeading);
				} else {
					scriptAirplaneAltitude.SetAltitudeMin (1000);
					scriptAirplaneAltitude.CommandAltitudeWithoutDelay (scriptAirplaneAltitude.GetAltitudeAssigned ());
					scriptAirplaneSpeed.SetSpeedMin (140);
					takeoff = false;
				}
			}
		}
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
				scriptAirplaneAltitude.Abort (takeoff);
				scriptAirplaneSpeed.Abort (takeoff);
				scriptAirplaneHeading.Abort ();
				abort = false;
			}
		}

		UpdatePosition ();
		if (landing) {
			land ();
			//		} else if (CheckIfReadyToLand () && clearedToLand && delayedCommandTime == -1) {
			//			delayedCommandTime = Time.time + UnityEngine.Random.Range (1.5f, 4f);

		} else if (clearedToLand && Time.time > delayedCommandTime && CheckIfReadyToLand ()) {
			scriptAirplaneAltitude.SetAltitudeMin (0);
			scriptAirplaneSpeed.SetSpeedMin (0);
			landing = true;
			chatText.GetComponent<Script_ChatText> ().StartNewLine (iDColor);
			chatText.GetComponent<Script_ChatText> ().EnableBold ();
			chatText.GetComponent<Script_ChatText> ().AddText ("landing, " + id + ".");
			chatText.GetComponent<Script_ChatText> ().DisableBold ();
			chatText.GetComponent<Script_ChatText> ().EndLine ();
		}
		if (Time.time > timeCounter) {
			UpdateAirplaneUIElements ();
			timeCounter += 3;
		}
		airplaneSprite.transform.position = transform.position;
		airplaneSprite.transform.eulerAngles = new Vector3 (90, 0, transform.eulerAngles.y * -1);
	}

	public void UpdateAirplaneUIElementUIPositions () {
		if (scriptAirplaneDots) {
			scriptAirplaneDots.UpdateUIPosition ();
		}
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateUIPosition (GetComponent<Script_AirplaneDots> ().getAirplaneMainDotPosition ());
	}

	private void UpdateAirplaneUIElements () {
		UpdateAirplaneText ();
		scriptAirplaneDots.UpdateWorldPosition ();
		UpdateAirplaneUIElementUIPositions ();
	}

	public void Abort () {
		abort = true;
		delayedCommandTime = Time.time + UnityEngine.Random.Range (1.5f, 3f);
	}

	public void GrantLandingClearance (string clearedApprchId) {
		clearedApproachId = clearedApprchId;
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
		airplaneText.GetComponent<Script_AirplaneText> ().DestoryLineImage ();
		Destroy (airplaneText.gameObject);
		scriptAirplaneDots.DestroyDots ();
		scriptAirplaneHeading.DestroyHeadingAssignedGameObject ();
		Destroy (airplaneSprite.gameObject);
		Destroy (this.gameObject);
	}

	public void setIsInsideApproachArea (bool b, Transform approachTransform, string id) {
		isInsideApproachArea = b;
		approachId = id;
		runwayDirection = (int)approachTransform.eulerAngles.y;
		otherTransform = approachTransform;

	}

	public void SetUpValues (int airplaneId, int airplaneAltitude, int airplaneSpeed, int airplaneHeading, bool airplaneTakeoff, string dispName) {
		displayName = dispName;
		id = airplaneId;
		standby = airplaneTakeoff;
		takeoff = airplaneTakeoff;
		if (airplaneTakeoff) {
			GetComponent<Script_AirplaneDots> ().SetActive (false);
			GetComponent<Collider> ().enabled = false;
			GetComponent<Script_AirplaneAltitude> ().SetAltitudeMin (0);
			GetComponent<Script_AirplaneSpeed> ().SetSpeedMin (0);
			takeoffHeading = airplaneHeading;
		} else {
			if (dispName == "radar") {
				GetComponent<Script_AirplaneDots> ().SetActive (true);
			}
			GetComponent<Collider> ().enabled = true;
			GetComponent<Script_AirplaneAltitude> ().SetAltitudeMin (1000);
			GetComponent<Script_AirplaneSpeed> ().SetSpeedMin (140);
		}
		GetComponent<Script_AirplaneAltitude> ().SetAltitude (airplaneAltitude);
		GetComponent<Script_AirplaneAltitude> ().CommandAltitude (airplaneAltitude);
		GetComponent<Script_AirplaneSpeed> ().SetSpeed (airplaneSpeed);
		GetComponent<Script_AirplaneSpeed> ().CommandSpeed (airplaneSpeed);
		GetComponent<Script_AirplaneHeading> ().SetHeading (airplaneHeading);
		GetComponent<Script_AirplaneHeading> ().SecondaryStart ();
		GetComponent<Script_AirplaneHeading> ().CommandHeading (airplaneHeading, 0);
		transform.eulerAngles = new Vector3 (0, airplaneHeading, 0);
		if (airplaneText.activeInHierarchy) {
			airplaneText.GetComponent<Script_AirplaneText> ().SetAirplaneId (id);
			airplaneText.GetComponent<Script_AirplaneText> ().setController (controller);
		}
	}

	public bool CheckIfReadyToLand () {
		if (isInsideApproachArea
			&& clearedApproachId == approachId
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
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneFlightlevel (AltitudeToFlightlevel (scriptAirplaneAltitude.GetAltitude ()));
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneFlightlevelAssigned (AltitudeToFlightlevel (scriptAirplaneAltitude.GetAltitudeAssigned ()));
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneHeading (scriptAirplaneHeading.GetHeading ());
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneSpeed ((int)scriptAirplaneSpeed.GetSpeed ());
		airplaneText.GetComponent<Script_AirplaneText> ().UpdateAirplaneText ();
	}

	public void ActivateOutOfFuelMode () {
		controller.GetComponent<Script_Controller> ().ProcessOutOfFuel (id);
		scriptAirplaneAltitude.ActivateGlideMode ();
	}

	public string GetId () {
		return id.ToString ();
	}

	public void AddToChatList (string s) {
		if (chatCommentsList.Count == 0) {
			s = char.ToUpper (s[0]) + s.Substring (1);
		}
		chatCommentsList.Add (s);
		delayedChatCommentTime = Time.time + UnityEngine.Random.Range (1.5f, 8f);
		newChatComments = true;
	}

	public void OverrideChatList (string s) {
		chatCommentsList.Clear ();
		s = char.ToUpper (s[0]) + s.Substring (1);
		chatCommentsList.Add (s);
		delayedChatCommentTime = Time.time + UnityEngine.Random.Range (1.5f, 8f);
		newChatComments = true;
	}

	public string GetIDColor () {
		return iDColor;
	}

	public GameObject GetAirplaneText () {
		return airplaneText;
	}

	public GameObject GetController () {
		return controller;
	}

	public Vector3 GetWorldPosition () {
		return transform.position;
	}
	public bool GetTakeoff () {
		return takeoff;
	}

	public void GrantTakeoffClearance () {
		delayedCommandTime = Time.time + UnityEngine.Random.Range (8, 16);
		clearedToTakeoff = true;
	}

	public bool GetStandby () {
		return standby;
	}

	public void ChangeDisplayName (string dn) {
		displayName = dn;
		if (dn == "radar") {
			airplaneSprite.SetActive (false);
			if (standby) {
				scriptAirplaneDots.SetActive (false);
				airplaneText.SetActive (false);
			} else {
				scriptAirplaneDots.SetActive (true);
				airplaneText.SetActive (true);
			}

		} else if (dn == "satellite") {
			airplaneSprite.SetActive (true);
			scriptAirplaneDots.SetActive (false);
			airplaneText.SetActive (false);
		}
	}

	public void OverrideChatIDString () {
		overrideChatIdString = true;
	}
}