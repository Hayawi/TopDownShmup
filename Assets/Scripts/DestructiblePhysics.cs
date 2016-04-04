using UnityEngine;
using System.Collections;

public class DestructiblePhysics : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Explosion")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

    }
}
