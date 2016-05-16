using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShootingController : NetworkBehaviour {

	public GameObject characterBody;

	public GameObject shotLocation;

	public GameObject bullet;

	public float bulletSpeed = 10f;

	public float rateOfFire = 25f;

    public int reloadSpeed = 50;

    public int ammoCount;

    public int clipSize = 20;

	private float fireTimer = 0f;

	private Animator playerAnimator;

    public Image bulletImage;

    public string playerName;

    Image[] ammo;

	// Use this for initialization
	void Start () {
        playerName = GameObject.Find("NetworkManager").GetComponent<NetworkManagerUI>().chosenPlayerName;
        ammoCount = clipSize;
		playerAnimator = characterBody.GetComponent<Animator> ();
        if (isLocalPlayer)
        {
            ammo = new Image[clipSize];
            for (int i = 0; i < clipSize; i++)
            {
                ammo[i] = (Image)Instantiate(bulletImage, new Vector3(bulletImage.gameObject.transform.localPosition.x, bulletImage.gameObject.transform.localPosition.y + i * 15, 0), bulletImage.gameObject.transform.rotation);
            }
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isLocalPlayer)
        {
            if (ammo[0].gameObject.transform.parent == null && GameObject.Find("Canvas(Clone)").transform != null)
            {
                for (int i = 0; i < clipSize; i++)
                    ammo[i].gameObject.transform.SetParent(GameObject.Find("Canvas(Clone)").transform);
            }
        }

        if (!playerAnimator.GetBool("Dead") && !GameObject.Find("NetworkManager").GetComponent<NetworkManagerUI>().pausePanel.activeSelf)
        {
            if (Input.GetButton("Shoot") && fireTimer >= rateOfFire)
            {
                if (ammoCount > 0)
                {
                    CmdspawnNewBullet(playerName);
                    fireTimer = 0;
                    ammoCount--;
                    if (isLocalPlayer)
                        updatingHUD();
                }
            }

            if (ammoCount == 0 && fireTimer >= reloadSpeed)
            {
                fireTimer = 0;
                ammoCount = clipSize;
                if (isLocalPlayer)
                    reloadAll();
            }
            if (Input.GetButton("Shoot"))
                playerAnimator.SetBool("Shooting", true);
            else
                playerAnimator.SetBool("Shooting", false);

            if (Input.GetButton("Reload"))
                ammoCount = 0;
            fireTimer += 1;
        }
        if (playerAnimator.GetBool("Dead"))
        {
            ammoCount = clipSize;
            reloadAll();
        }
    }

    void updatingHUD()
    {
        if (ammoCount < clipSize)
        {
            if (ammo[ammoCount].gameObject.activeSelf)
            {
                ammo[ammoCount].gameObject.SetActive(false);
            }
        }
        if (ammoCount > 0)
        {
            if (!ammo[ammoCount - 1].gameObject.activeSelf)
                ammo[ammoCount].gameObject.SetActive(true);
        }
    }

    void reloadAll()
    {
        for (int i = 0; i < clipSize; i++)
        {
            ammo[i].gameObject.SetActive(true);
        }
    }

    [Command]
	void CmdspawnNewBullet(string playerNameToPass) {
		GameObject bulletClone = (GameObject)Instantiate (bullet, shotLocation.GetComponent<Transform> ().position, (characterBody.GetComponent<Transform> ().rotation));
		float angle = bulletClone.transform.rotation.eulerAngles.z + 90f;
		bulletClone.GetComponent<Transform>().rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		bulletClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * bulletSpeed, ForceMode2D.Impulse);
        bulletClone.GetComponent<BulletBehaviour>().spawnedBy = GetComponent<NetworkIdentity>().netId;
        bulletClone.GetComponent<BulletBehaviour>().playerName = playerNameToPass;
        NetworkServer.Spawn(bulletClone);
    }
}
