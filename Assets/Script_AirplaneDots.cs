using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Script_AirplaneDots : MonoBehaviour {

	public GameObject airplaneDot;
	public GameObject airplaneMainDot;

	private List<GameObject> airplaneDots;
	private List<Vector3> positions;
	private int counter;
	private int positionsListIndex;
	private int positionsListSize;
	private int airplaneDotsListIndex;
	private int airplaneDotsListSize;
	private int offsetIndex;

	// Use this for initialization
	void Start () {
		airplaneMainDot = Instantiate (airplaneMainDot);
		airplaneMainDot.transform.SetParent (GetComponent<Script_Airplane> ().getController ().GetComponent<Script_Controller> ().GetDIPanel ().transform);
		GetComponent<Script_Airplane> ().getAirplaneText ().GetComponent<Script_AirplaneText> ().setAirplaneMainDot (airplaneMainDot);
		airplaneMainDot.transform.position = new Vector3 (Camera.main.WorldToScreenPoint (transform.position).x, Camera.main.WorldToScreenPoint (transform.position).y, 0);
		airplaneDotsListSize = 5;
		airplaneDotsListIndex = 0;
		offsetIndex = 2;
		positionsListSize = airplaneDotsListSize + offsetIndex;
		positionsListIndex = offsetIndex;
		offsetIndex = 0;
		positions = new List<Vector3> (positionsListSize);
		while (positions.Count < positionsListSize) {
			positions.Add (transform.position);
		}
		airplaneDots = new List<GameObject> (positionsListSize - 2);
		while (airplaneDots.Count < airplaneDotsListSize) {
			GameObject ad = Instantiate (airplaneDot);
			ad.transform.SetParent (GetComponent<Script_Airplane> ().getController ().GetComponent<Script_Controller> ().GetDIPanel ().transform);
			ad.transform.position = new Vector3 (Camera.main.WorldToScreenPoint (positions [offsetIndex]).x, Camera.main.WorldToScreenPoint (positions [offsetIndex]).y, 0);
			airplaneDots.Add (ad);
		}
		while (counter < Time.time) {
			counter += 3;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Time.time > counter) {
			positions [positionsListIndex] = transform.position;
			airplaneDots [airplaneDotsListIndex].transform.position = positions [offsetIndex];
			offsetIndex = step (offsetIndex, positionsListSize);
			positionsListIndex = step (positionsListIndex, positionsListSize);
			airplaneDotsListIndex = step (airplaneDotsListIndex, airplaneDotsListSize);
			counter += 3;
		}
	}

	public void UpdatePosition () {
		airplaneMainDot.transform.position = new Vector3 (Camera.main.WorldToScreenPoint (transform.position).x, Camera.main.WorldToScreenPoint (transform.position).y, 0);
		foreach (GameObject go in airplaneDots) {
			
		}
	}

	public void DestroyDots () {
		foreach (GameObject dot in airplaneDots) {
			Destroy (dot);
		}
		airplaneDots.Clear ();
		Destroy (airplaneMainDot);
	}

	int step (int i, int limit) {
		i++;
		if (i == limit) {
			i = 0;
		}
		return i;
	}

	public GameObject getAirplaneMainDot () {
		return airplaneMainDot;
	}
}
