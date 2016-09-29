using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class Script_ScheduleText : MonoBehaviour {

	int counter;
	Queue<Script_ScheduledFlight> scheduledFlights;

	// Use this for initialization
	void Start () {
		counter = 750;

	}
	
	// Update is called once per frame
	void Update () {
		counter++;
		if (counter > 1000) {
			UpdateScheduleText ();
			counter = 0;
		}
	}

	public void UpdateInformationForScheduleText (Queue<Script_ScheduledFlight> flights) {
		scheduledFlights = flights;
	}

	void UpdateScheduleText () {
		Queue<Script_ScheduledFlight> scheduledFlightsClone = new Queue<Script_ScheduledFlight> (scheduledFlights);
		StringBuilder sb = new StringBuilder ();
		while (scheduledFlightsClone.Count > 0) {
			sb.AppendLine (scheduledFlightsClone.Dequeue ().ToString ());
		}
		GetComponent<Text> ().text = sb.ToString ();

	}
}
