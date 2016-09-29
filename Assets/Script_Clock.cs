using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Script_Clock : MonoBehaviour {

	int landed;

	// Use this for initialization
	void Start () {
		landed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Text> ().text = "" + DateTime.Now.ToString ("HH:mm:ss") + "\nLanded: " + landed;
	}

	public void SetLanded (int i) {
		landed = i;
	}
}
