using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float characterSpeed;
	public Camera characterCamera;

	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetButton("Horizontal")) {
			GetComponent<Transform> ().Translate(new Vector3(characterSpeed * Input.GetAxis("Horizontal"), 0, 0));
		}
		if (Input.GetButton ("Vertical")) {
			GetComponent<Transform> ().Translate(new Vector3(0, characterSpeed * Input.GetAxis("Vertical"), 0));
		}
		FaceMouse();
	}

	 void FaceMouse () {
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		Vector3 lookPos = characterCamera.ScreenToWorldPoint(mousePos);
		lookPos = lookPos - transform.position;
		float angle = (Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg) - 90f;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		characterCamera.GetComponent<Transform> ().rotation = Quaternion.AngleAxis (0, Vector3.forward);
	}
}