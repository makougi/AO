using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class Script_AirplaneWaypoints : MonoBehaviour {

	private Script_AirplaneMain airplaneMainScript;
	private List<Script_Waypoint> waypointsList;
	private bool active;
	private int waypointsIndex;

	// Use this for initialization
	void Start () {
		waypointsIndex = 0;
		airplaneMainScript = GetComponent<Script_AirplaneMain> ();
		waypointsList = new List<Script_Waypoint> ();
		Script_Waypoint waypoint;
		DateTime wptime = DateTime.Now;
		waypoint = ScriptableObject.CreateInstance<Script_Waypoint> ();
		wptime = wptime.AddMinutes (5);
		waypoint.Construct ("Alpha", "waypoint", new Vector3 (100, 100), 10000, wptime);
		waypointsList.Add (waypoint);
		waypoint = ScriptableObject.CreateInstance<Script_Waypoint> ();
		wptime = wptime.AddMinutes (5);
		waypoint.Construct ("Bravo", "waypoint", new Vector3 (0, 200), 10000, wptime);
		waypointsList.Add (waypoint);
	}

	// Update is called once per frame
	void Update () {
		if (Mathf.Abs (waypointsList[waypointsIndex].GetVector3WpPosition ().x - transform.position.x) < 10 && Mathf.Abs (waypointsList[waypointsIndex].GetVector3WpPosition ().z - transform.position.z) < 10) {
			waypointsIndex++;
			if (airplaneMainScript.GetMode () == "waypoint") {
				Activate (true);
			}
		}
	}

	public void Activate (bool activate) {
		airplaneMainScript.CommandHeadingToPosition (waypointsList[waypointsIndex].GetVector3WpPosition (), waypointsList[waypointsIndex].GetWpName (), false);
	}

	public string GetCurrentWaypointName () {
		return waypointsList[waypointsIndex].GetWpName ();
	}

	public string ReturnWaypointStatusString () {
		DateTime wpTime = waypointsList[waypointsIndex].GetWpTime ();
		int hours = wpTime.Hour - DateTime.Now.Hour;
		int minutes = wpTime.Minute - DateTime.Now.Minute;
		int seconds = wpTime.Second - DateTime.Now.Second;
		StringBuilder sb = new StringBuilder ();
		AppendHoursMinutesAndSeconds (sb, hours, minutes, seconds);
		return sb.ToString () + ", role " + waypointsList[waypointsIndex].GetWpType ();
	}

	private void AppendHoursMinutesAndSeconds (StringBuilder sb, int hours, int minutes, int seconds) {
		if (hours != 0 || minutes != 0 || seconds >= 10) {
			sb.Append ("ETA ");
			if (hours > 1) {
				sb.Append (hours + " hours");
			} else if (hours == 1) {
				sb.Append ("1 hour");
				AppendMinutesRoundedToTen (sb, minutes);
			} else {
				AppendMinutesAndSeconds (sb, minutes, seconds);
			}
		}
	}

	private void AppendMinutesRoundedToTen (StringBuilder sb, int minutes) {
		if (minutes >= 10) {
			minutes = minutes / 10;
			sb.Append (" " + minutes + "0 minutes");
		}
	}

	private void AppendMinutesAndSeconds (StringBuilder sb, int minutes, int seconds) {
		if (minutes > 30) {
			int fives = minutes % 10;
			if (fives > 4) {
				fives = 5;
			} else {
				fives = 0;
			}
			sb.Append (minutes / 10 + "" + fives + " minutes");
		} else if (minutes > 1) {
			sb.Append (minutes + " minutes");
		} else if (minutes == 1) {
			sb.Append ("1 minute");
			AppendSecondsRoundedToTen (sb, seconds);
		} else {
			AppendSeconds (sb, seconds);
		}
	}

	private void AppendSecondsRoundedToTen (StringBuilder sb, int seconds) {
		if (seconds >= 10) {
			seconds = seconds / 10;
			sb.Append (" " + seconds + "0 seconds");
		}
	}

	private void AppendSeconds (StringBuilder sb, int seconds) {
		if (seconds >= 10) {
			seconds = seconds / 10;
			sb.Append (seconds + "0 seconds");
		}
	}
}
