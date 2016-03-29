using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

	public float deathTime;

	private float bulletTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (bulletTimer >= deathTime || GetComponent<Rigidbody2D> ().velocity.magnitude <= 0.5f) {
			Destroy (gameObject);
		}
		bulletTimer += 1;
	}
}
