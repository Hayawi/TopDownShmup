using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float characterSpeed;
	public Camera characterCamera;
    public GameObject characterBody;
	private Animator playerAnimator;

    private float characterRotation;
    private Rigidbody2D rigBody;
    private Transform charTrans;
    private Transform camTrans;

    void Start() {
        playerAnimator = characterBody.GetComponent<Animator>();
        rigBody = GetComponent<Rigidbody2D>();
        charTrans = characterBody.GetComponent<Transform>();
        camTrans = characterCamera.GetComponent<Transform>();
        }

	// Update is called once per frame
	void FixedUpdate () {
		rigBody.velocity = new Vector2(characterSpeed * Input.GetAxis("Horizontal"), characterSpeed * Input.GetAxis("Vertical"));
			playerAnimator.SetBool ("Walking", true);
		if (!Input.GetButton ("Vertical") && !Input.GetButton ("Horizontal")) {
			playerAnimator.SetBool ("Walking", false);
		}
		FaceMouse();
	}

	 void FaceMouse () {
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		Vector3 lookPos = characterCamera.ScreenToWorldPoint(mousePos);
		lookPos = lookPos - transform.position;
		float angle = (Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg) - 90f;
		charTrans.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
        characterRotation = angle;
		camTrans.rotation = Quaternion.AngleAxis (0, Vector3.forward);
	}
}