﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Script_AirplaneText : MonoBehaviour {

	public GameObject lineImage;

	private GameObject controller;
	private Vector3 airplaneMainDotUIPosition;
	private Vector3 offset;
	private Vector3 offsetDirection;
	private Vector3 offsetDefault;

	private int airplaneId;
	private int airplaneFlightlevel;
	private int airplaneFlightlevelAssigned;
	private int airplaneHeading;
	private int airplaneSpeed;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	public void Construct (int iDInt, GameObject dIPanelGO, bool airplaneTextsOffsetBool, bool standbyBool) {
		airplaneId = iDInt;
		offsetDefault = new Vector3 (0, -30, 0);
		offset = offsetDefault;
		lineImage = Instantiate (lineImage);
		transform.SetParent (dIPanelGO.transform);
		lineImage.transform.SetParent (dIPanelGO.transform);
		RandomizeOffset (airplaneTextsOffsetBool);
		this.gameObject.SetActive (!standbyBool);
	}

	public void UpdateUIPosition (Vector3 mainDotPosition) {
		airplaneMainDotUIPosition = mainDotPosition;
		UpdatePosition ();
	}

	public void UpdateAirplaneText (int flightlevel, int flightlevelAssigned, int heading, int speed) {
		airplaneFlightlevel = flightlevel;
		airplaneFlightlevelAssigned = flightlevelAssigned;
		airplaneHeading = heading;
		airplaneSpeed = speed;
		UpdateText ();
	}

	void UpdatePosition () {
		transform.position = airplaneMainDotUIPosition + offset;
		drawLine (airplaneMainDotUIPosition + offsetDirection * 15, transform.position - offsetDirection * 30, 2);
	}

	void UpdateText () {
		GetComponent<Text> ().text = ""
		+ IdToFourDigitString (airplaneId) + "\n"
		+ airplaneFlightlevel + "  " + airplaneFlightlevelAssigned + "\n"
		+ airplaneSpeed + "  " + HeadingToThreeDigitString (airplaneHeading);
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

	public void RandomizeOffset (bool active) {
		if (active) {
			int offsetAngle = UnityEngine.Random.Range (0, 360);
			float offsetDistance = UnityEngine.Random.Range (50f, 150f);
			offsetDirection = Quaternion.AngleAxis (offsetAngle, Vector3.forward) * Vector3.up;
			offset = offsetDirection * offsetDistance;
			transform.position = airplaneMainDotUIPosition + offset;
			UpdatePosition ();
			lineImage.SetActive (true);
		} else {
			lineImage.SetActive (false);
			offset = offsetDefault;
			transform.position = airplaneMainDotUIPosition + offset;
			UpdatePosition ();
		}
	}

	private void drawLine (Vector3 pointA, Vector3 pointB, float lineWidth) { // http://answers.unity3d.com/questions/865927/draw-a-2d-line-in-the-new-ui.html
		Vector3 differenceVector = pointB - pointA;

		lineImage.GetComponent<RectTransform> ().sizeDelta = new Vector2 (differenceVector.magnitude, lineWidth);
		lineImage.GetComponent<RectTransform> ().pivot = new Vector2 (0, 0.5f);
		lineImage.GetComponent<RectTransform> ().position = pointA;
		float angle = Mathf.Atan2 (differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
		lineImage.GetComponent<RectTransform> ().rotation = Quaternion.Euler (0, 0, angle);
	}

	public void DestoryLineImage () {
		Destroy (lineImage.gameObject);
	}
}
