using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnvironmentSetup : NetworkBehaviour {

    public GameObject gameMap;

    GameObject gameMapClone;

	// Use this for initialization
	void Start () {
        if (!GameObject.Find("GameMap(Clone)"))
        {
            gameMapClone = (GameObject)Instantiate(gameMap, gameMap.GetComponent<Transform>().position, gameMap.GetComponent<Transform>().rotation);
            NetworkServer.Spawn(gameMapClone);
        }
    }

    void OnDestroy()
    {
        Destroy(gameMapClone);
    }
}
