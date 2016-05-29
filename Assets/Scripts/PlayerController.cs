using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    [SyncVar]
	public float characterSpeed;

    public Camera characterCamera;

    public GameObject characterBody;

	private Animator playerAnimator;

    public GameObject walkingSound;

	void Start() {
		playerAnimator = characterBody.GetComponent<Animator> ();
	}

    // Update is called once per frame
    void FixedUpdate() {
        if (!playerAnimator.GetBool("Dead") && isLocalPlayer)
        {
            if (Input.GetButton("Horizontal"))
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(characterSpeed * Input.GetAxis("Horizontal"), 0));
                //GetComponent<Transform>().Translate(new Vector3(characterSpeed * Input.GetAxis("Horizontal"), 0, 0));
            }
            if (Input.GetButton("Vertical"))
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, characterSpeed * Input.GetAxis("Vertical")));
                //GetComponent<Transform>().Translate(new Vector3(0, characterSpeed * Input.GetAxis("Vertical"), 0));
            }
            FaceMouse();
        }

        if (!playerAnimator.GetBool("Dead") && GetComponent<Rigidbody2D>().velocity.magnitude > 5)
        {
            playerAnimator.SetBool("Walking", true);
            if (!walkingSound.GetComponent<AudioSource>().isPlaying)
            {
                walkingSound.GetComponent<AudioSource>().Play();
                walkingSound.GetComponent<AudioSource>().loop = true;
            }
        }
        else
        {
            playerAnimator.SetBool("Walking", false);
            walkingSound.GetComponent<AudioSource>().loop = false;
        }
    }

	 void FaceMouse () {
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		Vector3 lookPos = characterCamera.ScreenToWorldPoint(mousePos);
		lookPos = lookPos - transform.position;
		float angle = (Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg) - 90f;
		characterBody.GetComponent<Transform>().rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		characterCamera.GetComponent<Transform> ().rotation = Quaternion.AngleAxis (0, Vector3.forward);
	}
}