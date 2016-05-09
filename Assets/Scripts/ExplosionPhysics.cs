using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ExplosionPhysics : NetworkBehaviour {

    public int health = 1;

    public BoxCollider2D explosion;

    public GameObject crater;

	// Use this for initialization
	void Start () {
	
	}
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Bullet" || coll.gameObject.tag == "notBullet")
        {
            health -= 1;
            Destroy(coll.gameObject);
        }
    }

	// Update is called once per frame
	void Update () {
        if (health <= 0)
        {
            Instantiate(explosion, gameObject.GetComponent<Transform>().position, explosion.GetComponent<Transform>().rotation);
            CmdSpawnCrater();
            Destroy(gameObject);
        }
    }

    [Command]
    void CmdSpawnCrater()
    {
        GameObject craterClone = (GameObject)Instantiate(crater, gameObject.GetComponent<Transform>().position, crater.GetComponent<Transform>().rotation);
        NetworkServer.Spawn(craterClone);
    }
}
