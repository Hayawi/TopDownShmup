using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SyncVar]
    public string playerName;

    [SyncVar]
    public int kills;

    [SyncVar]
    public int deaths;

    [SyncVar]
    public int gameTime = 600;

    Camera sceneCamera;

    void Start()
    {
        if (isLocalPlayer && isServer)
        {
            gameTime = GameObject.Find("NetworkManager").GetComponent<NetworkManagerUI>().originalGameTime;
            gameObject.name = "PlayerCharacterStandingServer";
        }
        if (!isLocalPlayer && isServer )
        {
            gameTime = GameObject.Find("PlayerCharacterStandingServer").GetComponent<PlayerSetup>().gameTime;
        }

        if (isLocalPlayer)
            CmdSettingName(GameObject.Find("NetworkManager").GetComponent<NetworkManagerUI>().chosenPlayerName);
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

        }
    }

    void FixedUpdate()
    {
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerUI>().updateGameTime(gameTime);
        if (isServer)
            gameTime--;
    }

    [Command]
    void CmdSettingName(string name)
    {
        playerName = name;
    }

    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

}
