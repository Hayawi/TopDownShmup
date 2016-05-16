using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour {

    [SyncVar]
    public int playerHP = 100;

    int deathRespawnTime = 300;

    GameObject headsUpDisplay;
    GameObject respawnTimer;
    GameObject healthPercent;

    Animator playerAnimator; 

    public GameObject characterBody;
    public GameObject inGameUI;
    public GameObject[] spawnPoint = new GameObject[8];
    public GameObject respawnTimerText;
    public GameObject healthPercentText;

    void OnDestroy()
    {
        Destroy(headsUpDisplay);
        Destroy(respawnTimer);
    }

    // Use this for initialization
    void Start () {
        playerAnimator = characterBody.GetComponent<Animator>();
        if (isLocalPlayer)
        {
            headsUpDisplay = (GameObject)Instantiate(inGameUI, new Vector3(0, 0, 0), inGameUI.transform.rotation);
            respawnTimer = (GameObject)Instantiate(respawnTimerText, new Vector3(0, 0, 0), respawnTimerText.transform.rotation);
            respawnTimer.transform.SetParent(headsUpDisplay.transform, false);
            respawnTimer.GetComponent<Text>().text = "";
            healthPercent = (GameObject)Instantiate(healthPercentText, new Vector3(200, 50, 0), healthPercentText.GetComponent<Transform>().rotation);
            healthPercent.transform.SetParent(headsUpDisplay.transform, false);
        }
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
            healthPercent.GetComponent<Text>().text = playerHP.ToString() + "%";
        if (playerHP > 0)
        {
            playerAnimator.SetBool("Dead", false);
            deathRespawnTime = 300;
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = false;
            playerAnimator.SetBool("Dead", true);
        }

        if (playerHP <= 0)
        {
            deathRespawnTime -= 1;
            if (isLocalPlayer)
                respawnTimer.GetComponent<Text>().text = (deathRespawnTime / 60).ToString();
        }
        if (deathRespawnTime <= 0)
        {
            playerAnimator.SetBool("Dead", false);
            GetComponent<BoxCollider2D>().enabled = true;
            playerHP = 100;
            if (isLocalPlayer)
                respawnTimer.GetComponent<Text>().text = "";
            transform.position = spawnPoint[Random.Range(0, 7)].transform.position;
        }
        if (playerHP <= 0)
        {
            playerHP = 0;
        }
        else
        {
            if (isLocalPlayer)
                respawnTimer.GetComponent<Text>().text = "";
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Bullet" && playerHP > 0)
        {
            serverHitsPlayer(Random.Range(20,30), coll);
            Destroy(coll.gameObject);
        }
        if (coll.gameObject.tag == "Explosion" && playerHP > 0)
        {
            serverHitsPlayer(Random.Range(30, 70), coll);
        }
    }

    void serverHitsPlayer(int amountOfDamage, Collision2D coll) {
        playerHP -= amountOfDamage;
        if (playerHP <= 0)
        {
            if (coll.gameObject.tag == "Bullet")
            {
                if (coll.gameObject.GetComponent<BulletBehaviour>().spawnedBy != gameObject.GetComponent<NetworkIdentity>().netId)
                    ClientScene.FindLocalObject(coll.gameObject.GetComponent<BulletBehaviour>().spawnedBy).GetComponent<PlayerSetup>().kills += 1;
            }
            gameObject.GetComponent<PlayerSetup>().deaths += 1;
        }
    }
}
