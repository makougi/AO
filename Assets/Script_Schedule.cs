using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Script_Schedule : MonoBehaviour {

	public GameObject scheduleText;

	private List<GameObject> approaches;
	private DateTime previousEntryTime;
	private List<int> activeIds;
	private Queue<Script_ScheduledFlight> scheduledFlights;
	private int scheduleSize;
	private List<Script_FlightEntrypoint> entrypoints;

	// Use this for initialization
	void Start () {
		CreateEntrypoints ();
		if (activeIds == null) {
			activeIds = new List<int> ();
		}
		scheduleSize = 20;
		previousEntryTime = DateTime.Now;
		scheduledFlights = new Queue<Script_ScheduledFlight> ();
	}

	// Update is called once per frame
	void Update () {
		while (scheduledFlights.Count < scheduleSize) {
			Script_ScheduledFlight flight = ScriptableObject.CreateInstance<Script_ScheduledFlight> ();
			flight.SetUpValues (activeIds, previousEntryTime, entrypoints);
			activeIds.Add (flight.GetId ());
			previousEntryTime = flight.GetEntryTime ();
			scheduledFlights.Enqueue (flight);
			UpdateScheduleText ();
		}
	}

	private void CreateEntrypoints () {
		entrypoints = new List<Script_FlightEntrypoint> ();
		Script_FlightEntrypoint entrypoint = ScriptableObject.CreateInstance<Script_FlightEntrypoint> ();
		foreach (GameObject go in approaches) {
			entrypoint = ScriptableObject.CreateInstance<Script_FlightEntrypoint> ();
			entrypoint.SetId (go.GetComponent<Script_Approach> ().GetId ());
			entrypoint.SetDirection (go.GetComponent<Script_Approach> ().GetDirection ());
			entrypoint.SetPosition (go.GetComponent<Script_Approach> ().GetPositionWorldVector2 ());
			entrypoint.SetTakeoff (true);
			entrypoints.Add (entrypoint);
		}
		entrypoint = ScriptableObject.CreateInstance<Script_FlightEntrypoint> ();
		entrypoint.SetId ("A");
		entrypoint.SetDirection (0);
		entrypoint.SetPosition (new Vector2 (40, -40));
		entrypoint.SetTakeoff (false);
		entrypoints.Add (entrypoint);
		entrypoint = ScriptableObject.CreateInstance<Script_FlightEntrypoint> ();
		entrypoint.SetId ("B");
		entrypoint.SetDirection (90);
		entrypoint.SetPosition (new Vector2 (-40, -40));
		entrypoint.SetTakeoff (false);
		entrypoints.Add (entrypoint);
		entrypoint = ScriptableObject.CreateInstance<Script_FlightEntrypoint> ();
		entrypoint.SetId ("C");
		entrypoint.SetDirection (180);
		entrypoint.SetPosition (new Vector2 (-40, 40));
		entrypoint.SetTakeoff (false);
		entrypoints.Add (entrypoint);
		entrypoint = ScriptableObject.CreateInstance<Script_FlightEntrypoint> ();
		entrypoint.SetId ("D");
		entrypoint.SetDirection (270);
		entrypoint.SetPosition (new Vector2 (40, 40));
		entrypoint.SetTakeoff (false);
		entrypoints.Add (entrypoint);
	}

	public void SetApproaches (List<GameObject> apprchs) {
		approaches = apprchs;
	}

	void UpdateScheduleText () {
		scheduleText.GetComponent<Script_ScheduleText> ().UpdateInformationForScheduleText (scheduledFlights);
	}

	public List<int> getActiveIds () {
		return activeIds;
	}

	public Script_ScheduledFlight dequeueFlightIfReady () {
		if (DateTime.Compare (DateTime.Now, scheduledFlights.Peek ().GetEntryTime ()) > 0) {
			return scheduledFlights.Dequeue ();
		}
		return null;
	}

	public void AddToActiveIds (int id) {
		if (activeIds == null) {
			activeIds = new List<int> ();
		}
		activeIds.Add (id);
	}

	public void RemoveIdFromActiveIds (int id) {
		activeIds.Remove (id);
	}
}
