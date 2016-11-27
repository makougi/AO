using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Script_ChatParser : MonoBehaviour {

	public GameObject gameObjectChatText;

	private Script_ChatText chat;
	private Script_AirplaneMain sApMain;
	private Script_AirplaneChat sApChat;
	private Script_AirplaneAltitude sApAltitude;
	private Script_AirplaneSpeed sApSpeed;
	private Script_AirplaneHeading sApHeading;
	private Dictionary<int, GameObject> airplanesDictionary;
	private List<GameObject> airplanesList;
	private Dictionary<string, GameObject> beaconsDictionary;
	private Dictionary<string, GameObject> approachesDictionary;
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

	public void SetBeaconsDictionary (Dictionary<string, GameObject> bD) {
		beaconsDictionary = bD;
	}

	public void SetApproachesDictionary (Dictionary<string, GameObject> approachesDic) {
		approachesDictionary = approachesDic;
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
					sApMain = ap.GetComponent<Script_AirplaneMain> ();
					sApChat = ap.GetComponent<Script_AirplaneChat> ();
					sApAltitude = ap.GetComponent<Script_AirplaneAltitude> ();
					sApSpeed = ap.GetComponent<Script_AirplaneSpeed> ();
					sApHeading = ap.GetComponent<Script_AirplaneHeading> ();
				} else {
					airplaneId = 0;
					sApChat = null;
					sApAltitude = null;
					sApSpeed = null;
					sApHeading = null;
				}
				if (words.Length == 1) {
					if (airplaneId != 0) {
						sApChat.AddToChatList ("Tower");
					}
				} else {
					chat.AddComma ();
				}
				for (int i = 1; i < words.Length; i++) {
					if (words[i].Length > 0) {
						if (words[i][0] == 'a') { // if first character is a
							ParseAndAssignAltitude (sApChat, sApAltitude, words[i].Substring (1));
						} else if (words[i][0] == 's') { // if first character is s
							ParseAndAssignSpeed (sApChat, sApSpeed, words[i].Substring (1));
						} else if (words[i][0] == 'd') { // if first character is d
							ParseAndAssignHeading (sApChat, sApHeading, words[i].Substring (1));
						} else if (words[i][0] == 'f') {
							ParseAndAssignHeadingToBeacon (sApChat, sApHeading, words[i].Substring (1));
						} else if (words[i][0] == '+') {
							ParseAndAssignHeadingToBeaconAndHoldingPattern (sApChat, sApHeading, words[i].Substring (1));
						} else if (words[i] == "-fuel") {
							RequestFuelInfo (sApChat, sApSpeed);
						} else if (words[i] == "-abort") {
							Abort (sApMain);
							sApChat.OverrideChatList ("aborting");
							discardCommands = true;
						} else if (words[i] == "-takeoff") {
							GrantTakeoffClearance (sApMain, sApChat, sApSpeed);
						} else if (words[i].Length >= 5) {
							if (words[i].Substring (0, 5) == "-land") {
								GrantLandingClearance (sApMain, sApChat, words[i].Substring (5));
							} else {
								ProcessInvalidCommand (sApChat);
							}
						} else {
							ProcessInvalidCommand (sApChat);
						}
					}
				}
			}
			chat.AddDot ();
			chat.EndLine ();
			discardCommands = false;
		}
	}

	private void ProcessInvalidCommand (Script_AirplaneChat apChatScript) {
		chat.AddText ("...");
		apChatScript.OverrideChatList ("say again");
		discardCommands = true;
	}

	private void RequestStandbyCheckin () {
		chat.AddText ("Flights ready for takeoff please check in");
		foreach (GameObject go in airplanesList) {
			if (go.GetComponent<Script_AirplaneMain> ().GetStandby ()) {
				go.GetComponent<Script_AirplaneChat> ().OverrideChatIDString ();
				go.GetComponent<Script_AirplaneChat> ().AddToChatList (go.GetComponent<Script_AirplaneMain> ().GetId () + " ready for takeoff");
			}
		}
	}

	void ParseAndAssignAltitude (Script_AirplaneChat airplaneChatScript, Script_AirplaneAltitude altitudeScript, string word) {
		chat.AddText ("altitude to " + word);
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (word, out dflt)) {
				int alt = int.Parse (word);
				if (altitudeScript) {
					if (altitudeScript.CheckCommand (alt * 100)) {
						altitudeScript.CommandAltitude (alt * 100); // assign altitude				
						airplaneChatScript.AddToChatList ("altitude to " + alt.ToString ());
					} else {
						airplaneChatScript.OverrideChatList ("unable, invalid altitude");
						discardCommands = true;
					}
				}
			} else {
				airplaneChatScript.OverrideChatList ("say again");
				discardCommands = true;
			}
		}

	}

	void ParseAndAssignSpeed (Script_AirplaneChat airplaneChatScript, Script_AirplaneSpeed speedScript, string word) {
		chat.AddText ("speed to " + word);
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (word, out dflt)) {
				int spd = int.Parse (word);
				if (speedScript) {
					if (speedScript.CheckCommand (spd)) {
						speedScript.CommandSpeed (spd); // assign speed										
						airplaneChatScript.AddToChatList ("speed to " + spd.ToString ());
					} else {
						airplaneChatScript.OverrideChatList ("unable, invalid speed");
						discardCommands = true;
					}
				}
			} else {
				airplaneChatScript.OverrideChatList ("say again");
				discardCommands = true;
			}
		}
	}

	void ParseAndAssignHeading (Script_AirplaneChat airplaneChatScript, Script_AirplaneHeading headingScript, string word) {
		if (word.Length > 0) {
			if (word[0] == 'l') {
				chat.AddText ("turn left heading");
				if (!discardCommands && airplaneChatScript) {
					airplaneChatScript.AddToChatList ("left to");
				}
				ParseAndAssignHeadingNormalOrLeftOrRight (sApChat, headingScript, word.Substring (1), 1);
			} else if (word[0] == 'r') {
				chat.AddText ("turn right heading ");
				if (!discardCommands && airplaneChatScript) {
					airplaneChatScript.AddToChatList ("right to");
				}
				ParseAndAssignHeadingNormalOrLeftOrRight (sApChat, headingScript, word.Substring (1), 2);
			} else {
				chat.AddText ("heading to");
				if (!discardCommands && airplaneChatScript) {
					airplaneChatScript.AddToChatList ("heading to");
				}
				ParseAndAssignHeadingNormalOrLeftOrRight (sApChat, headingScript, word, 0);
			}
		} else {
			chat.AddText ("...");
			airplaneChatScript.OverrideChatList ("say again");
			discardCommands = true;
		}
	}

	void ParseAndAssignHeadingNormalOrLeftOrRight (Script_AirplaneChat airplaneChatScript, Script_AirplaneHeading headingScript, string word, int normalOrLeftOrRight) {
		chat.AddText (word);
		if (!discardCommands) {
			int dflt = 0;
			if (int.TryParse (word, out dflt)) {
				int hdg = int.Parse (word);
				if (headingScript) {
					if (headingScript.CheckCommand (hdg)) {
						headingScript.CommandHeading (hdg, normalOrLeftOrRight); // assign heading
						airplaneChatScript.AddToChatList (hdg.ToString ());
					} else {
						airplaneChatScript.AddToChatList ("unable, invalid heading");
						discardCommands = true;
					}
				}
			} else {
				airplaneChatScript.OverrideChatList ("say again");
				discardCommands = true;
			}
		}
	}

	void ParseAndAssignHeadingToBeacon (Script_AirplaneChat airplaneChatScript, Script_AirplaneHeading headingScript, string word) {
		string beaconId = word.ToUpperInvariant ();
		chat.AddText ("heading to " + beaconId);
		if (!discardCommands) {
			if (headingScript) {
				if (beaconsDictionary.ContainsKey (beaconId)) {
					headingScript.CommandBeaconHeading (beaconsDictionary[beaconId].GetComponent<Script_Beacon> ().GetWorldPosition ());
					airplaneChatScript.AddToChatList ("heading to " + ConvertLettersToICAOPronounciation (beaconId));
				} else {
					airplaneChatScript.AddToChatList ("unable, invalid beacon id");
					discardCommands = true;
				}
			}
		}
	}

	void ParseAndAssignHeadingToBeaconAndHoldingPattern (Script_AirplaneChat airplaneChatScript, Script_AirplaneHeading headingScript, string word) {
		string beaconId = word.ToUpperInvariant ();
		chat.AddText ("holding pattern at " + beaconId);
		if (!discardCommands) {
			if (headingScript) {
				if (beaconsDictionary.ContainsKey (beaconId)) {
					headingScript.CommandHoldingPattern (beaconsDictionary[beaconId].GetComponent<Script_Beacon> ().GetWorldPosition ());
					airplaneChatScript.AddToChatList ("holding at " + ConvertLettersToICAOPronounciation (beaconId));
				} else {
					airplaneChatScript.AddToChatList ("unable, invalid beacon id");
					discardCommands = true;
				}
			}
		}
	}

	void GrantLandingClearance (Script_AirplaneMain airplaneMainScript, Script_AirplaneChat airplaneChatScript, string wrd) {
		string word = wrd.ToUpperInvariant ();
		chat.AddText ("cleared to land " + word);
		if (approachesDictionary.ContainsKey (word)) {
			airplaneMainScript.GrantLandingClearance (word);
			if (airplaneMainScript.CheckIfReadyToLand () == false) {
				airplaneChatScript.AddToChatList ("cleared to land " + word);
			}
		} else if (wrd == "") {
			airplaneChatScript.AddToChatList ("requesting approach id");
			discardCommands = true;
		} else {
			airplaneChatScript.AddToChatList ("unable, invalid approach id");
			discardCommands = true;
		}
	}

	void RequestFuelInfo (Script_AirplaneChat airplaneChatScript, Script_AirplaneSpeed speedScript) {
		chat.AddText ("report fuel level");
		if (!discardCommands) {
			airplaneChatScript.AddToChatList ("fuel level " + ((int)(speedScript.getFuel () / 100)) * 100);
		}
	}

	private void GrantTakeoffClearance (Script_AirplaneMain airplaneMainScript, Script_AirplaneChat airplaneChatScript, Script_AirplaneSpeed speedScript) {
		chat.AddText ("cleared to takeoff");
		if (airplaneMainScript.GetTakeoff ()) {
			airplaneMainScript.GrantTakeoffClearance ();
			if (speedScript.GetSpeedAssigned () < 140) {
				speedScript.CommandSpeed (240);
			}
			airplaneChatScript.AddToChatList ("cleared to takeoff");
		} else {
			airplaneChatScript.AddToChatList ("already airborne");
			discardCommands = true;
		}
	}

	void Abort (Script_AirplaneMain airplaneMainScript) {
		chat.AddText ("abort");
		airplaneMainScript.Abort ();
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
