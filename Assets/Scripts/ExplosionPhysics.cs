using UnityEngine;
using System.Collections;

public class ExplosionPhysics : MonoBehaviour {

    public int health = 1;

    public BoxCollider2D explosion;

    public Rigidbody2D crater;

	// Use this for initialization
	void Start () {
	
	}
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Bullet")
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
            Instantiate(crater, gameObject.GetComponent<Transform>().position, crater.GetComponent<Transform>().rotation);
            Destroy(gameObject);
        }
    }
}
