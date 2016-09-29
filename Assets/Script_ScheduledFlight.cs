using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Script_ScheduledFlight : ScriptableObject {

	DateTime entryTime;
	int id;
	int altitude;
	int speed;
	string entrypoint;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetUpValues (List<int> activeIds, DateTime previousEntryTime) {
		entryTime = SelectArbitraryEntrytime (previousEntryTime);
		id = CreateId (activeIds);
		altitude = SelectArbitraryAltitude ();
		speed = SelectArbitrarySpeed ();
		entrypoint = SelectArbitraryEntrypoint ();
	}

	DateTime SelectArbitraryEntrytime (DateTime previousEntryTime) {
		int maxEntryTimeSeparationInSeconds = 240;
		return previousEntryTime.AddSeconds (UnityEngine.Random.Range (0, maxEntryTimeSeparationInSeconds));
	}

	int CreateId (List<int> activeIds) {
		while (true) {
			int id = UnityEngine.Random.Range (1, 10000);
			if (!activeIds.Contains (id)) {
				return id;
			}			
		}
	}

	int SelectArbitraryAltitude () {
		return UnityEngine.Random.Range (10000, 30000);
	}

	int SelectArbitrarySpeed () {
		return UnityEngine.Random.Range (250, 500);
	}

	string SelectArbitraryEntrypoint () {
		List<string> entrypoints = new List<string> ();
		entrypoints.Add ("A");
		entrypoints.Add ("B");
		entrypoints.Add ("C");
		entrypoints.Add ("D");
		entrypoints.Add ("E");
		return entrypoints [UnityEngine.Random.Range (0, 4)];
	}

	override public string ToString () {
		return "ETA " + HourOrMinuteToTwoDigitString (entryTime.Hour) + ":" + HourOrMinuteToTwoDigitString (entryTime.Minute) + " - ID " + idToFourDigitString (id) + " - FL" + AltitudeToRoundedFlightlevelString (altitude) + " - " + entrypoint;
	}

	string HourOrMinuteToTwoDigitString (int integ) {
		if (integ > 9) {
			return integ.ToString ();
		}
		return "0" + integ;
	}

	string idToFourDigitString (int idInt) {
		if (id > 999) {
			return "" + idInt;
		}
		if (id > 99) {
			return "0" + idInt;
		}
		if (id > 9) {
			return "00" + idInt;
		}
		return "000" + idInt;
	}

	string AltitudeToRoundedFlightlevelString (int alt) {
		int altid = 10 * (int)Math.Round (alt * 0.001f);
		return altid.ToString ();
	}

	public DateTime getEntryTime () {
		return entryTime;
	}

	public int getId () {
		return id;
	}

	public int getAltitude () {
		return altitude;
	}

	public int getSpeed () {
		return speed;
	}

	public string getEntryPoint () {
		return entrypoint;
	}


}
