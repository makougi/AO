using UnityEngine;
using System.Collections;

public class Script_ColorsInterface : MonoBehaviour {

	Script_Colors colors;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	public string PickARandomColor () {
		if (!colors) {
			colors = ScriptableObject.CreateInstance<Script_Colors> ();
		}
		return colors.PickARandomColor ();
	}
}
