using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class BulletBehaviour : NetworkBehaviour {

	public float deathTime;

    public float timeTillLive;

	private float bulletTimer;

    [SyncVar]
    public NetworkInstanceId spawnedBy;

	// Use this for initialization
	void Start () {
    }

    public override void OnStartClient()
    {

        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), ClientScene.FindLocalObject(spawnedBy).GetComponent<BoxCollider2D>(), true);
    }

    // Update is called once per frame
    void FixedUpdate () {
		if (bulletTimer >= deathTime || GetComponent<Rigidbody2D> ().velocity.magnitude <= 0.5f) {
			Destroy (gameObject);
		}
        if (bulletTimer >= timeTillLive)
        {
            try {
                if (isClient)
                    Physics2D.IgnoreCollision(ClientScene.FindLocalObject(spawnedBy).GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>(), false);
            } catch (Exception e)
            {
                Destroy(gameObject);
            }
        }
        bulletTimer += 1;
	}
}
