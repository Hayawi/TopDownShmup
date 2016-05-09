using UnityEngine;
using System.Collections;

public class ExplosionDeath : MonoBehaviour {

    Animator animator;

	// Use this for initialization
	void Start () {
        animator = gameObject.GetComponent<Animator>();
	}

    // Update is called once per frame
    void FixedUpdate () {
	    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            Destroy(gameObject);
        }
	}
}
