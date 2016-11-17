using UnityEngine;
using System.Collections;

public class Script_AirplaneSpeed : MonoBehaviour {

	GameObject chatText;

	private bool speedCommandCompleted;
	private bool newCommand;
	private bool doneOnce;
	private int speedAssigned;
	private int unactivatedSpeedAssigned;
	private int speedMin;
	private int speedMax;
	private int fuelChatMark;
	private float speed;
	private float speedChangeRate;
	private float DelayedCommandTime;
	private float fuel;

	// Use this for initialization
	void Start () {
		fuelChatMark = 10000;
		fuel = UnityEngine.Random.Range (10000, 100000);
		speedMax = 600;
		speedChangeRate = 4.9f;
		speedCommandCompleted = true;
	}

	// Update is called once per frame
	void Update () {
		if (fuel < fuelChatMark) {
			chatText.GetComponent<Script_ChatText> ().StartNewLine (GetComponent<Script_Airplane> ().GetIDColor ());
			chatText.GetComponent<Script_ChatText> ().EnableBold ();
			chatText.GetComponent<Script_ChatText> ().AddText ("fuel " + fuelChatMark + ",  " + GetComponent<Script_Airplane> ().GetId () + ".");
			chatText.GetComponent<Script_ChatText> ().DisableBold ();
			chatText.GetComponent<Script_ChatText> ().EndLine ();
			if (fuelChatMark > 2500) {
				fuelChatMark -= 2500;
			} else if (fuelChatMark > 500) {
				fuelChatMark -= 500;
			} else {
				fuelChatMark -= 100;
			}

		}
		if (fuel > 0) {
			fuel -= speed / 75 * Time.deltaTime;
		} else {
			if (!doneOnce) {
				doneOnce = true;
				GetComponent<Script_Airplane> ().ActivateOutOfFuelMode ();
				speedChangeRate = 1;
				speedAssigned = 250;
				speedCommandCompleted = false;
			}
			if (GetComponent<Script_AirplaneAltitude> ().GetAltitude () < 200) {
				speedMin = 0;
				speedAssigned = 0;
				speedChangeRate = 9;
				speedCommandCompleted = false;
			}
		}


		if (newCommand) {
			if (Time.time > DelayedCommandTime) {
				if (unactivatedSpeedAssigned == -1) {
					speedAssigned = (int)speed;
				} else {
					speedAssigned = unactivatedSpeedAssigned;
				}
				speedCommandCompleted = false;
				newCommand = false;
			}
		}

		if (speedCommandCompleted == false) {
			CheckAndCorrectCommand ();
			UpdateSpeed ();

			if (speed == speedAssigned) {
				speedCommandCompleted = true;
			}
		}
	}

	void CheckAndCorrectCommand () {
		if (speedAssigned < speedMin) {
			speedAssigned = speedMin;
		} else if (speedAssigned > speedMax) {
			speedAssigned = speedMax;
		}
	}

	void UpdateSpeed () {
		if (speed < speedAssigned) {
			speed += 1 * Time.deltaTime * speedChangeRate;
			if (speed > speedAssigned) {
				speed = speedAssigned;
			}
		} else if (speed > speedAssigned) {
			speed -= 1 * Time.deltaTime * speedChangeRate;
			if (speed < speedAssigned) {
				speed = speedAssigned;
			}
		}
		if (speed < speedMin) {
			speed = speedMin;
		} else if (speed > speedMax) {
			speed = speedMax;
		}
	}

	public void SetSpeed (float spd) {
		speed = spd;
	}

	public void CommandSpeed (int spd) {
		unactivatedSpeedAssigned = spd;
		DelayedCommandTime = Time.time + UnityEngine.Random.Range (1.5f, 7f);
		newCommand = true;
	}

	public float GetSpeed () {
		return speed;
	}

	public int GetSpeedAssigned () {
		return speedAssigned;
	}

	public void SetSpeedMin (int s) {
		speedMin = s;
	}

	public void ActivateBrakingMode () {
		speedChangeRate = 11;
	}

	public void CommandSpeedWithoutDelay (int spd) {
		speedAssigned = spd;
		speedCommandCompleted = false;
	}

	public void setChatText (GameObject chatTextGameObject) {
		chatText = chatTextGameObject;
	}

	public float getFuel () {
		return fuel;
	}

	public bool CheckCommand (int spd) {
		if (spd < speedMin) {
			return false;
		}
		if (spd > speedMax) {
			return false;
		}
		return true;
	}

	public void Abort (bool takeoff) {
		if (takeoff) {
			speedMin = 0;
			CommandSpeed (0);
		} else {
			speedMin = 140;
			speedMax = 600;
			speedChangeRate = 3.9f;
			speedCommandCompleted = true;
			CommandSpeed (-1);
			newCommand = true;
		}
	}
}
