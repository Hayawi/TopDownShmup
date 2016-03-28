using UnityEngine;
using System.Collections;

public class ShootingController : MonoBehaviour {

	public GameObject characterBody;

	public Rigidbody2D bullet;

	public float bulletSpeed = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Shoot")) {
			spawnNewBullet();
		}
	}

	void spawnNewBullet() {
		Rigidbody2D bulletClone = (Rigidbody2D)Instantiate (bullet, characterBody.GetComponent<Transform> ().position, (characterBody.GetComponent<Transform> ().rotation));
		float angle = bulletClone.transform.rotation.eulerAngles.z ;
		bulletClone.GetComponent<Transform>().rotation = Quaternion.AngleAxis (angle, Vector3.forward);

		bulletClone.velocity =  new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * bulletSpeed;
		print (bulletClone.transform.rotation.eulerAngles);
	}
}
