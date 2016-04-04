using UnityEngine;
using System.Collections;

public class ShootingController : MonoBehaviour {

	public GameObject characterBody;

	public GameObject shotLocation;

	public Rigidbody2D bullet;

	public float bulletSpeed = 10f;

	public float rateOfFire = 25f;

	private float fireTimer = 0f;

	private Animator playerAnimator;

	// Use this for initialization
	void Start () {
		playerAnimator = characterBody.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton ("Shoot") && fireTimer >= rateOfFire) {
			spawnNewBullet();
			fireTimer = 0;
		}
		if (Input.GetButton ("Shoot"))
			playerAnimator.SetBool ("Shooting", true);
		else
			playerAnimator.SetBool ("Shooting", false);
		fireTimer += 1;
	}

	void spawnNewBullet() {
		Rigidbody2D bulletClone = (Rigidbody2D)Instantiate (bullet, shotLocation.GetComponent<Transform> ().position, (characterBody.GetComponent<Transform> ().rotation));
		float angle = bulletClone.transform.rotation.eulerAngles.z + 90f;
		bulletClone.GetComponent<Transform>().rotation = Quaternion.AngleAxis (angle, Vector3.forward);

		bulletClone.velocity =  new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * bulletSpeed;
	}
}
