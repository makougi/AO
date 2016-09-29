using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Script_Schedule : MonoBehaviour {

	public GameObject scheduleText;

	List<int> activeIds;
	int scheduleSize;
	DateTime previousEntryTime;
	Queue<Script_ScheduledFlight> scheduledFlights;

	// Use this for initialization
	void Start () {
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
			flight.SetUpValues (activeIds, previousEntryTime);
			activeIds.Add (flight.getId ());
			previousEntryTime = flight.getEntryTime ();
			scheduledFlights.Enqueue (flight);
			UpdateScheduleText ();
		}
	}

	void UpdateScheduleText () {
		scheduleText.GetComponent<Script_ScheduleText> ().UpdateInformationForScheduleText (scheduledFlights);
	}

	public List<int> getActiveIds () {
		return activeIds;
	}

	public Script_ScheduledFlight dequeueFlightIfReady () {
		if (DateTime.Compare (DateTime.Now, scheduledFlights.Peek ().getEntryTime ()) > 0) {
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
