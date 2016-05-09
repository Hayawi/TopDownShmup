using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShootingController : NetworkBehaviour {

	public GameObject characterBody;

	public GameObject shotLocation;

	public GameObject bullet;

	public float bulletSpeed = 10f;

	public float rateOfFire = 25f;

	private float fireTimer = 0f;

	private Animator playerAnimator;

	// Use this for initialization
	void Start () {
		playerAnimator = characterBody.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!playerAnimator.GetBool("Dead"))
        {
            if (Input.GetButton("Shoot") && fireTimer >= rateOfFire)
            {
                CmdspawnNewBullet();
                fireTimer = 0;
            }
            if (Input.GetButton("Shoot"))
                playerAnimator.SetBool("Shooting", true);
            else
                playerAnimator.SetBool("Shooting", false);
            fireTimer += 1;
        }
	}

    [Command]
	void CmdspawnNewBullet() {
		GameObject bulletClone = (GameObject)Instantiate (bullet, shotLocation.GetComponent<Transform> ().position, (characterBody.GetComponent<Transform> ().rotation));
		float angle = bulletClone.transform.rotation.eulerAngles.z + 90f;
		bulletClone.GetComponent<Transform>().rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		bulletClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * bulletSpeed, ForceMode2D.Impulse);
        bulletClone.GetComponent<BulletBehaviour>().spawnedBy = GetComponent<NetworkIdentity>().netId;
        NetworkServer.Spawn(bulletClone);
    }
}
