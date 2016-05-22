using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FlashlightController : NetworkBehaviour {

    public GameObject flashlight;

    [SyncVar]
    bool flashLightOn = false;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButtonDown("Flashlight") && isLocalPlayer)
        {
            CmdOnFlash();
        }
        if (flashlight.transform.parent.GetComponent<Animator>().GetBool("Dead"))
        {
            flashlight.SetActive(false);
        }
        flashlight.SetActive(flashLightOn);
    }

    [Command]
    void CmdOnFlash()
    {
        flashLightOn = !flashLightOn;
    }
}
