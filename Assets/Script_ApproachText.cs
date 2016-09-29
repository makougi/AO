using UnityEngine;
using System.Collections;

public class Script_ApproachText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetUpApproachText (Vector3 approachPosition, Vector3 approachDirection, string approachId) {
		GetComponent<TextMesh> ().text = approachId;

	}
}
