using UnityEngine;
using System.Collections;

public class Script_Approach : MonoBehaviour {

	public GameObject approachText;

	private Vector3 approachPosition;
	private Vector3 approachDirection;
	private string id;

	// Use this for initialization
	void Start () {
		approachPosition = new Vector3 (20 - UnityEngine.Random.Range (0, 40), 0, 10 - UnityEngine.Random.Range (0, 20));
		approachDirection = new Vector3 (90, 10 * UnityEngine.Random.Range (0, 36), 0);
//		approachPosition = new Vector3 (0, 0, 0);
//		approachDirection = new Vector3 (90, 0, 0);
		this.transform.position = approachPosition;
		this.transform.eulerAngles = approachDirection;
		id = CreateId ();

		approachText = Instantiate (approachText);
		approachText.transform.position = transform.TransformPoint (2, -16.2f, 0);
		approachText.GetComponent<TextMesh> ().text = id;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	string CreateId () {
		if (approachDirection.y < 100) {
			return "0" + approachDirection.y / 10;
		}
		return "" + approachDirection.y / 10;
	}

	void OnTriggerEnter (Collider collider) {
		if (collider.gameObject.GetComponent<Script_Airplane> ()) {
			collider.gameObject.GetComponent<Script_Airplane> ().setIsInsideApproachArea (true, transform);
		}
	}

	void OnTriggerExit (Collider collider) {
		if (collider.gameObject.GetComponent<Script_Airplane> ()) {
			collider.gameObject.GetComponent<Script_Airplane> ().setIsInsideApproachArea (false, transform);
		}
	}
}
