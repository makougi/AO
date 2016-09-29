using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Script_AirplaneDots : MonoBehaviour {

	public GameObject airplaneDot;
	public GameObject airplaneMainDot;
	int counter;
	int positionsListIndex;
	int positionsListSize;
	int airplaneDotsListIndex;
	int airplaneDotsListSize;
	int offsetIndex;
	List<GameObject> airplaneDots;
	List<Vector3> positions;

	// Use this for initialization
	void Start () {
		airplaneMainDot = Instantiate (airplaneMainDot);
		airplaneMainDot.transform.position = transform.position;
		GetComponent<Script_Airplane> ().getAirplaneText ().GetComponent<Script_AirplaneText> ().setAirplaneMainDot (airplaneMainDot);
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
			ad.transform.position = transform.position;
			airplaneDots.Add (ad);
		}
		while (counter < Time.time) {
			counter += 3;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > counter) {
			airplaneMainDot.transform.position = transform.position;
			positions [positionsListIndex] = transform.position;
			airplaneDots [airplaneDotsListIndex].transform.position = positions [offsetIndex];
			offsetIndex = step (offsetIndex, positionsListSize);
			positionsListIndex = step (positionsListIndex, positionsListSize);
			airplaneDotsListIndex = step (airplaneDotsListIndex, airplaneDotsListSize);
			counter += 3;
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
