using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Script_Approach : MonoBehaviour {

	//	public GameObject approachImage;
	public GameObject approachText;

	private Vector3 approachPosition;
	private Vector3 approachDirection;
	private string id;

	// Use this for initialization
	void Start () {

	}

	public void RunSecondaryStart (GameObject controller) {
		approachPosition = new Vector3 (20 - UnityEngine.Random.Range (0, 40), 0, 10 - UnityEngine.Random.Range (0, 20));
		approachDirection = new Vector3 (90, 10 * UnityEngine.Random.Range (0, 36), 0);
		this.transform.position = approachPosition;
		this.transform.eulerAngles = approachDirection;
		id = CreateId ();

		//		approachImage = Instantiate (approachImage);
		//		approachImage.transform.SetParent (controller.GetComponent<Script_Controller> ().GetDIPanel ().transform);
		//		approachImage.transform.position = new Vector3 (Camera.main.WorldToScreenPoint (transform.position).x, Camera.main.WorldToScreenPoint (transform.position).y, 0);
		//		approachImage.transform.eulerAngles = new Vector3 (0, 0, approachDirection.y - 180);

		approachText = Instantiate (approachText);
		approachText.transform.SetParent (controller.GetComponent<Script_Controller> ().GetDIPanel ().transform);
		approachText.GetComponent<Text> ().text = id;
		UpdateUIPosition ();
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

	public void UpdateUIPosition () {
		approachText.transform.position = new Vector3 (Camera.main.WorldToScreenPoint (transform.TransformPoint (2, -16.2f, 0)).x, Camera.main.WorldToScreenPoint (transform.TransformPoint (2, -16.2f, 0)).y);
	}
}
