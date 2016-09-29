using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Script_Parser : MonoBehaviour {

	Dictionary<int, GameObject> airplanesDictionary;
	public GameObject gameObjectChatText;
	Script_ChatText chat;
	Dictionary<string, Vector3> beaconsDictionary;
	Script_Airplane sAp;
	Script_AirplaneAltitude sApAltitude;
	Script_AirplaneSpeed sApSpeed;
	Script_AirplaneHeading sApHeading;
	int airplaneId;
	bool discardCommands;


	// Use this for initialization
	void Start () {
		chat = gameObjectChatText.GetComponent<Script_ChatText> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetAirplanesDictionary (Dictionary<int, GameObject> aPD) {
		airplanesDictionary = aPD;
	}

	public void SetBeaconsDictionary (Dictionary<string, Vector3> bD) {
		beaconsDictionary = bD;
	}

	public void Parse (string command) {
		if (command != "") {
			chat.StartNewLine ("<color=black>");
			string[] words = command.Split (' ');
			int deflt = 0;
			if (int.TryParse (words [0], out deflt)) {
				int id = int.Parse (words [0]);
				chat.AddText (id.ToString ());
				if (airplanesDictionary.ContainsKey (id)) { // if there is an airplane with this id word[0]
					airplaneId = id;
					GameObject ap = airplanesDictionary [id];
					sAp = ap.GetComponent<Script_Airplane> ();
					sApAltitude = ap.GetComponent<Script_AirplaneAltitude> ();
					sApSpeed = ap.GetComponent<Script_AirplaneSpeed> ();
					sApHeading = ap.GetComponent<Script_AirplaneHeading> ();
				} else {
					airplaneId = 0;
					sAp = null;
					sApAltitude = null;
					sApSpeed = null;
					sApHeading = null;
				}
				if (words.Length == 1) {
					if (airplaneId != 0) {
						sAp.AddToChatList ("Tower");
					}
				} else {
					chat.AddComma ();
				}
				for (int i = 1; i < words.Length; i++) {
					if (words [i].Length > 0) {
						if (words [i] [0] == 'a') { // if first character is a
							ParseAndAssignAltitude (sAp, sApAltitude, words, i);
						} else if (words [i] [0] == 's') { // if first character is s
							ParseAndAssignSpeed (sAp, sApSpeed, words, i);
						} else if (words [i] [0] == 'd') { // if first character is d
							ParseAndAssignHeading (sAp, sApHeading, words, i);
						} else if (words [i] [0] == 'f') {
							ParseAndAssignHeadingToBeacon (sAp, sApHeading, words, i);
						} else if (words [i] [0] == '+') {
							ParseAndAssignHeadingToBeaconAndHoldingPattern (sAp, sApHeading, words, i);
						} else if (words [i] == "-land") {
							GrantLandingClearance (sAp);
						} else if (words [i] == "-fuel") {
							RequestFuelInfo (sAp, sApSpeed);
						} else if (words [i] == "-abort") {
							Abort (sAp);
							sAp.OverrideChatList ("aborting");
							discardCommands = true;
						} else {
							chat.AddText ("...");
							sAp.OverrideChatList ("say again");
							discardCommands = true;
						}
					}
				}
			}
			chat.AddDot ();
			chat.EndLine ();
			discardCommands = false;
		}
	}

	void ParseAndAssignAltitude (Script_Airplane airplaneScript, Script_AirplaneAltitude altitudeScript, string[] words, int i) {
		chat.AddText ("altitude to " + words [i].Substring (1));			
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (words [i].Substring (1), out dflt)) {
				int alt = int.Parse (words [i].Substring (1));
				if (altitudeScript) {
					if (altitudeScript.CheckCommand (alt * 100)) {
						altitudeScript.CommandAltitude (alt * 100); // assign altitude				
						airplaneScript.AddToChatList ("altitude to " + alt.ToString ());
					} else {
						airplaneScript.OverrideChatList ("unable, invalid altitude");
						discardCommands = true;
					}
				}
			} else {
				airplaneScript.OverrideChatList ("say again");
				discardCommands = true;
			}			
		}

	}

	void ParseAndAssignSpeed (Script_Airplane airplaneScript, Script_AirplaneSpeed speedScript, string[] words, int i) {
		chat.AddText ("speed to " + words [i].Substring (1));
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (words [i].Substring (1), out dflt)) {
				int spd = int.Parse (words [i].Substring (1));
				if (speedScript) {
					if (speedScript.CheckCommand (spd)) {
						speedScript.CommandSpeed (spd); // assign speed										
						airplaneScript.AddToChatList ("speed to " + spd.ToString ());
					} else {
						airplaneScript.OverrideChatList ("unable, invalid speed");
						discardCommands = true;
					}
				}
			} else {
				airplaneScript.OverrideChatList ("say again");
				discardCommands = true;
			}
		}
	}

	void ParseAndAssignHeading (Script_Airplane airplaneScript, Script_AirplaneHeading headingScript, string[] words, int i) {
		if (words [i].Length > 1) {
			if (words [i] [1] == 'l') {
				chat.AddText ("turn left heading");
				if (!discardCommands && airplaneScript) {
					airplaneScript.AddToChatList ("left to");					
				}
				ParseAndAssignHeadingNormalOrLeftOrRight (sAp, headingScript, words, i, 1, 2);
			} else if (words [i] [1] == 'r') {
				chat.AddText ("turn right heading ");
				if (!discardCommands && airplaneScript) {
					airplaneScript.AddToChatList ("right to");
				}
				ParseAndAssignHeadingNormalOrLeftOrRight (sAp, headingScript, words, i, 2, 2);
			} else {
				chat.AddText ("heading to");
				if (!discardCommands && airplaneScript) {
					airplaneScript.AddToChatList ("heading to");
				}
				ParseAndAssignHeadingNormalOrLeftOrRight (sAp, headingScript, words, i, 0, 1);			
			}	
		} else {
			chat.AddText ("...");
			airplaneScript.OverrideChatList ("say again");
			discardCommands = true;
		}
	}

	void ParseAndAssignHeadingNormalOrLeftOrRight (Script_Airplane airplaneScript, Script_AirplaneHeading headingScript, string[] words, int i, int normalOrLeftOrRight, int substringStart) {
		chat.AddText (words [i].Substring (substringStart));		
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (words [i].Substring (substringStart), out dflt)) {
				int hdg = int.Parse (words [i].Substring (substringStart));
				if (headingScript) {
					if (headingScript.CheckCommand (hdg)) {
						headingScript.CommandHeading (hdg, normalOrLeftOrRight); // assign heading
						airplaneScript.AddToChatList (hdg.ToString ());						
					} else {
						airplaneScript.AddToChatList ("unable, invalid heading");
						discardCommands = true;
					}
				}				
			} else {
				airplaneScript.OverrideChatList ("say again");
				discardCommands = true;
			}
		}
	}

	void ParseAndAssignHeadingToBeacon (Script_Airplane airplaneScript, Script_AirplaneHeading headingScript, string[] words, int i) {
		string beaconId = words [i].Substring (1).ToUpperInvariant ();
		chat.AddText ("heading to " + beaconId);
		if (!discardCommands) {
			if (headingScript) {
				if (beaconsDictionary.ContainsKey (beaconId)) {
					headingScript.CommandBeaconHeading (beaconsDictionary [beaconId]);
					airplaneScript.AddToChatList ("heading to " + beaconId);
				} else {
					airplaneScript.AddToChatList ("unable, invalid beacon id");
					discardCommands = true;
				}			
			}
		}
	}

	void ParseAndAssignHeadingToBeaconAndHoldingPattern (Script_Airplane airplaneScript, Script_AirplaneHeading headingScript, string[] words, int i) {
		string beaconId = words [i].Substring (1).ToUpperInvariant ();
		chat.AddText ("holding pattern at " + beaconId);
		if (!discardCommands) {
			if (headingScript) {
				if (beaconsDictionary.ContainsKey (beaconId)) {
					headingScript.CommandHoldingPattern (beaconsDictionary [beaconId]);
					airplaneScript.AddToChatList ("holding at " + beaconId);
				} else {
					airplaneScript.AddToChatList ("unable, invalid beacon id");
					discardCommands = true;
				}
			}
		}
	}

	void GrantLandingClearance (Script_Airplane airplaneScript) {
		chat.AddText ("cleared to land");
		airplaneScript.GrantLandingClearance ();
		if (airplaneScript.CheckIfReadyToLand () == false) {
			airplaneScript.AddToChatList ("cleared to land");			
		}
	}

	void RequestFuelInfo (Script_Airplane airplaneScript, Script_AirplaneSpeed speedScript) {
		chat.AddText ("report fuel level");
		if (!discardCommands) {
			airplaneScript.AddToChatList ("fuel level " + ((int)(speedScript.getFuel () / 100)) * 100);			
		}
	}

	void Abort (Script_Airplane airplaneScript) {
		chat.AddText ("abort");
		airplaneScript.Abort ();
	}
}
