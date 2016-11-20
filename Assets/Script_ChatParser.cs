using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Script_ChatParser : MonoBehaviour {

	public GameObject gameObjectChatText;

	private Script_ChatText chat;
	private Script_Airplane sAp;
	private Script_AirplaneAltitude sApAltitude;
	private Script_AirplaneSpeed sApSpeed;
	private Script_AirplaneHeading sApHeading;
	private Dictionary<int, GameObject> airplanesDictionary;
	private List<GameObject> airplanesList;
	private Dictionary<string, Vector3> beaconsDictionary;
	private Dictionary<char, string> ICAOPronounciations;
	private bool discardCommands;
	private int airplaneId;

	// Use this for initialization
	void Start () {
		chat = gameObjectChatText.GetComponent<Script_ChatText> ();
		SetupICAOPronounciations ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void SetAirplanesDictionary (Dictionary<int, GameObject> aPD) {
		airplanesDictionary = aPD;
	}

	public void SetAirplanesList (List<GameObject> aPL) {
		airplanesList = aPL;
	}

	public void SetBeaconsDictionary (Dictionary<string, Vector3> bD) {
		beaconsDictionary = bD;
	}

	public void Parse (string command) {
		if (command != "") {
			chat.StartNewLine ("<color=black>");
			string[] words = command.Split (' ');
			int deflt = 0;
			if (words[0] == "ground") {
				RequestStandbyCheckin ();
			} else if (int.TryParse (words[0], out deflt)) {
				int id = int.Parse (words[0]);
				chat.AddText (id.ToString ());
				if (airplanesDictionary.ContainsKey (id)) { // if there is an airplane with this id word[0]
					airplaneId = id;
					GameObject ap = airplanesDictionary[id];
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
					if (words[i].Length > 0) {
						if (words[i][0] == 'a') { // if first character is a
							ParseAndAssignAltitude (sAp, sApAltitude, words, i);
						} else if (words[i][0] == 's') { // if first character is s
							ParseAndAssignSpeed (sAp, sApSpeed, words, i);
						} else if (words[i][0] == 'd') { // if first character is d
							ParseAndAssignHeading (sAp, sApHeading, words, i);
						} else if (words[i][0] == 'f') {
							ParseAndAssignHeadingToBeacon (sAp, sApHeading, words, i);
						} else if (words[i][0] == '+') {
							ParseAndAssignHeadingToBeaconAndHoldingPattern (sAp, sApHeading, words, i);
						} else if (words[i] == "-land") {
							GrantLandingClearance (sAp);
						} else if (words[i] == "-fuel") {
							RequestFuelInfo (sAp, sApSpeed);
						} else if (words[i] == "-abort") {
							Abort (sAp);
							sAp.OverrideChatList ("aborting");
							discardCommands = true;
						} else if (words[i] == "-takeoff") {
							GrantTakeoffClearance (sAp, sApSpeed);
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

	private void RequestStandbyCheckin () {
		chat.AddText ("Flights ready for takeoff please check in");
		foreach (GameObject go in airplanesList) {
			sAp = go.GetComponent<Script_Airplane> ();
			if (sAp.GetStandby ()) {
				sAp.OverrideChatIDString ();
				sAp.AddToChatList (sAp.GetId () + " ready for takeoff");
			}
		}
	}

	void ParseAndAssignAltitude (Script_Airplane airplaneScript, Script_AirplaneAltitude altitudeScript, string[] words, int i) {
		chat.AddText ("altitude to " + words[i].Substring (1));
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (words[i].Substring (1), out dflt)) {
				int alt = int.Parse (words[i].Substring (1));
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
		chat.AddText ("speed to " + words[i].Substring (1));
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (words[i].Substring (1), out dflt)) {
				int spd = int.Parse (words[i].Substring (1));
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
		if (words[i].Length > 1) {
			if (words[i][1] == 'l') {
				chat.AddText ("turn left heading");
				if (!discardCommands && airplaneScript) {
					airplaneScript.AddToChatList ("left to");
				}
				ParseAndAssignHeadingNormalOrLeftOrRight (sAp, headingScript, words, i, 1, 2);
			} else if (words[i][1] == 'r') {
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
		chat.AddText (words[i].Substring (substringStart));
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (words[i].Substring (substringStart), out dflt)) {
				int hdg = int.Parse (words[i].Substring (substringStart));
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
		string beaconId = words[i].Substring (1).ToUpperInvariant ();
		chat.AddText ("heading to " + beaconId);
		if (!discardCommands) {
			if (headingScript) {
				if (beaconsDictionary.ContainsKey (beaconId)) {
					headingScript.CommandBeaconHeading (beaconsDictionary[beaconId]);
					airplaneScript.AddToChatList ("heading to " + ConvertLettersToICAOPronounciation (beaconId));
				} else {
					airplaneScript.AddToChatList ("unable, invalid beacon id");
					discardCommands = true;
				}
			}
		}
	}

	void ParseAndAssignHeadingToBeaconAndHoldingPattern (Script_Airplane airplaneScript, Script_AirplaneHeading headingScript, string[] words, int i) {
		string beaconId = words[i].Substring (1).ToUpperInvariant ();
		chat.AddText ("holding pattern at " + beaconId);
		if (!discardCommands) {
			if (headingScript) {
				if (beaconsDictionary.ContainsKey (beaconId)) {
					headingScript.CommandHoldingPattern (beaconsDictionary[beaconId]);
					airplaneScript.AddToChatList ("holding at " + ConvertLettersToICAOPronounciation (beaconId));
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

	private void GrantTakeoffClearance (Script_Airplane airplaneScript, Script_AirplaneSpeed speedScript) {
		chat.AddText ("cleared to takeoff");
		if (airplaneScript.GetTakeoff ()) {
			airplaneScript.GrantTakeoffClearance ();
			if (speedScript.GetSpeedAssigned () < 140) {
				speedScript.CommandSpeed (240);
			}
			airplaneScript.AddToChatList ("cleared to takeoff");
		} else {
			airplaneScript.AddToChatList ("already airborne");
			discardCommands = true;
		}
	}

	void Abort (Script_Airplane airplaneScript) {
		chat.AddText ("abort");
		airplaneScript.Abort ();
	}

	private void SetupICAOPronounciations () {
		ICAOPronounciations = new Dictionary<char, string> ();
		//ICAOPronounciations.Add ('a', "Alpha");
		//ICAOPronounciations.Add ('b', "Bravo");
		//ICAOPronounciations.Add ('c', "Charlie");
		//ICAOPronounciations.Add ('d', "Delta");
		//ICAOPronounciations.Add ('e', "Echo");
		//ICAOPronounciations.Add ('f', "Foxtrot");
		//ICAOPronounciations.Add ('g', "Golf");
		//ICAOPronounciations.Add ('h', "Hotel");
		//ICAOPronounciations.Add ('i', "India");
		//ICAOPronounciations.Add ('j', "Juliet");
		//ICAOPronounciations.Add ('k', "Kilo");
		//ICAOPronounciations.Add ('l', "Lima");
		//ICAOPronounciations.Add ('m', "Mike");
		//ICAOPronounciations.Add ('n', "November");
		//ICAOPronounciations.Add ('o', "Oscar");
		//ICAOPronounciations.Add ('p', "Papa");
		//ICAOPronounciations.Add ('q', "Quebec");
		//ICAOPronounciations.Add ('r', "Romeo");
		//ICAOPronounciations.Add ('s', "Sierra");
		//ICAOPronounciations.Add ('t', "Tango");
		//ICAOPronounciations.Add ('u', "Uniform");
		//ICAOPronounciations.Add ('v', "Victor");
		//ICAOPronounciations.Add ('w', "Whiskey");
		//ICAOPronounciations.Add ('x', "Xray");
		//ICAOPronounciations.Add ('y', "Yankee");
		//ICAOPronounciations.Add ('z', "Zulu");
		ICAOPronounciations.Add ('a', "alpha");
		ICAOPronounciations.Add ('b', "bravo");
		ICAOPronounciations.Add ('c', "charlie");
		ICAOPronounciations.Add ('d', "delta");
		ICAOPronounciations.Add ('e', "echo");
		ICAOPronounciations.Add ('f', "foxtrot");
		ICAOPronounciations.Add ('g', "golf");
		ICAOPronounciations.Add ('h', "hotel");
		ICAOPronounciations.Add ('i', "india");
		ICAOPronounciations.Add ('j', "juliet");
		ICAOPronounciations.Add ('k', "kilo");
		ICAOPronounciations.Add ('l', "lima");
		ICAOPronounciations.Add ('m', "mike");
		ICAOPronounciations.Add ('n', "november");
		ICAOPronounciations.Add ('o', "oscar");
		ICAOPronounciations.Add ('p', "papa");
		ICAOPronounciations.Add ('q', "quebec");
		ICAOPronounciations.Add ('r', "romeo");
		ICAOPronounciations.Add ('s', "sierra");
		ICAOPronounciations.Add ('t', "tango");
		ICAOPronounciations.Add ('u', "uniform");
		ICAOPronounciations.Add ('v', "victor");
		ICAOPronounciations.Add ('w', "whiskey");
		ICAOPronounciations.Add ('x', "xray");
		ICAOPronounciations.Add ('y', "yankee");
		ICAOPronounciations.Add ('z', "zulu");
	}

	private string ConvertLettersToICAOPronounciation (string word) {
		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < word.Length; i++) {
			if (i > 0) {
				sb.Append (" ");
			}
			if (ICAOPronounciations.ContainsKey (char.ToLower (word[i]))) {
				sb.Append (ICAOPronounciations[char.ToLower (word[i])]);
			} else {
				sb.Append (char.ToUpper (word[i]));
			}
		}
		return sb.ToString ();
	}
}
