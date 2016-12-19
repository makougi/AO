using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
		airplaneMainScript.CommandHeadingToPosition (waypointsList[waypointsIndex].GetVector3WpPosition (), false);
	}

	public string GetCurrentWaypointName () {
		return waypointsList[waypointsIndex].GetWpName ();
	}
}
