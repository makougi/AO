using UnityEngine;
using System.Collections;

public class Script_AirplaneText : MonoBehaviour {

	GameObject controller;
	GameObject airplaneMainDot;

	int counter;
	Vector3 position;
	int airplaneId;
	int airplaneFlightlevel;
	int airplaneFlightlevelAssigned;
	int airplaneHeading;
	int airplaneSpeed;
	Vector3 offset;
	Vector3 offsetDirection;
	bool offsetActive;

	// Use this for initialization
	void Start () {
		RandomizeOffset (controller.GetComponent<Script_Controller> ().getAirplaneTextsOffset ());
		while (counter < Time.time) {
			counter += 3;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > counter) {
			UpdateAirplaneText ();
			counter += 3;
		}
	}

	public void UpdatePosition (Vector3 airplanePosition) {
		position = airplanePosition;
		position += offset;
	}

	public void UpdateAirplaneFlightlevel (int flightlevel) {
		airplaneFlightlevel = flightlevel;
	}

	public void UpdateAirplaneFlightlevelAssigned (int flightlevelAssigned) {
		airplaneFlightlevelAssigned = flightlevelAssigned;
	}

	public void UpdateAirplaneHeading (int heading) {
		airplaneHeading = heading;
	}

	public void UpdateAirplaneSpeed (int speed) {
		airplaneSpeed = speed;
	}

	void UpdateAirplaneText () {
		transform.position = position;
		GetComponent<TextMesh> ().text = ""
		+ IdToFourDigitString (airplaneId) + "\n"
		+ airplaneFlightlevel + "  " + airplaneFlightlevelAssigned + "\n"
		+ airplaneSpeed + "  " + HeadingToThreeDigitString (airplaneHeading);
		GetComponent<LineRenderer> ().SetPosition (0, airplaneMainDot.transform.position + offsetDirection);
		GetComponent<LineRenderer> ().SetPosition (1, transform.position - (offsetDirection * 3));
	}

	string IdToFourDigitString (int integ) {
		if (integ > 999) {
			return integ.ToString ();
		}
		if (integ > 99) {
			return "0" + integ;
		}
		if (integ > 9) {
			return "00" + integ;
		}
		return "000" + integ;
	}

	string HeadingToThreeDigitString (int integ) {
		if (integ > 999) {
			return "999";
		}
		if (integ > 99) {
			return integ.ToString ();
		}
		if (integ > 9) {
			return "0" + integ;
		}
		if (integ >= 0) {
			return "00" + integ;
		}
		return "000";
	}

	public void SetAirplaneId (int id) {
		airplaneId = id;
	}

	public void RandomizeOffset (bool active) {
		if (active) {
			int offsetAngle = UnityEngine.Random.Range (0, 360);
			float offsetDistance = UnityEngine.Random.Range (5f, 15f);
			offsetDirection = Quaternion.AngleAxis (offsetAngle, Vector3.up) * Vector3.forward;
			offset = offsetDirection * offsetDistance;
			position = airplaneMainDot.transform.position;
			position += offset;
			transform.position = position;
			GetComponent<LineRenderer> ().enabled = true;
			GetComponent<LineRenderer> ().SetPosition (0, airplaneMainDot.transform.position + offsetDirection);
			GetComponent<LineRenderer> ().SetPosition (1, transform.position - offsetDirection * 3);
		} else {
			offset = new Vector3 (0, 0, -3);
			position = airplaneMainDot.transform.position;
			position += offset;
			transform.position = position;
			GetComponent<LineRenderer> ().enabled = false;
		}
	}

	public void setAirplaneMainDot (GameObject apMd) {
		airplaneMainDot = apMd;
	}

	public void setController (GameObject contr) {
		controller = contr;
	}
}
