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

    public GameObject muzzleFlash;

    [SyncVar]
    bool onStatus = false;

    [SyncVar]
    bool shootingSoundTrigger = false;

    Image[] ammo;

    public GameObject shootingSound;

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

            if(Input.GetButton("Shoot") && fireTimer >= rateOfFire && !playerAnimator.GetBool("Dead") && !GameObject.Find("NetworkManager").GetComponent<NetworkManagerUI>().pausePanel.activeSelf && ammoCount > 0)
            {
                    CmdspawnNewBullet(playerName);
                    CmdMuzzleFlash(true);
                    CmdShootingSound(true);
                    shootingSoundTrigger = true;
                    onStatus = true;
                    fireTimer = 0;
                    ammoCount--;
                    updatingHUD();
                    playerAnimator.SetBool("Shooting", true);
            }

            if (!Input.GetButton("Shoot") || ammoCount == 0)
                playerAnimator.SetBool("Shooting", false);

            if (Input.GetButton("Reload"))
                ammoCount = 0;

            if (fireTimer >= rateOfFire / 4)
                CmdMuzzleFlash(false);

            if (ammoCount == 0 && fireTimer >= reloadSpeed)
            {
                fireTimer = 0;
                ammoCount = clipSize;
                reloadAll();
            }

            if (playerAnimator.GetBool("Dead"))
            {
                ammoCount = clipSize;
                reloadAll();
            }

            fireTimer += 1;
        }

        if (shootingSoundTrigger && !isServer)
        {
            shootingSound.GetComponent<AudioSource>().PlayOneShot(shootingSound.GetComponent<AudioSource>().clip);
            if (isLocalPlayer)
            {
                shootingSoundTrigger = false;
                CmdShootingSound(false);
            }
        }
        if (!isServer)
            muzzleFlash.SetActive(onStatus);
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

    [Command]
    void CmdMuzzleFlash(bool on)
    {
        onStatus = on;
        muzzleFlash.SetActive(on);
    }

    [Command]
    void CmdShootingSound(bool on)
    {
        shootingSoundTrigger = on;
        if (on)
            shootingSound.GetComponent<AudioSource>().PlayOneShot(shootingSound.GetComponent<AudioSource>().clip);
    }
}
