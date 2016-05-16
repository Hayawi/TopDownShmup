using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenuScript : MonoBehaviour {

    public GameObject gamemodePanel;
    public GameObject mainPanel;
    public Button infiltrationButton;
    public RawImage splashscreen;
    public AudioSource mainMenuSound;

    bool playedSplashScreen = false;

    public MovieTexture splashscreenTexture;

	// Use this for initialization
	void Start () {
        splashscreen.GetComponent<RawImage>().material.mainTexture = splashscreenTexture;
        splashscreenTexture.Play();
        if (GameObject.Find("NetworkManager"))
        {
            SceneManager.UnloadScene("Deathmatch");
            NetworkManager manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            manager.StopHost();
            Destroy(manager.gameObject);
        }
        infiltrationButton.interactable = false;
        gamemodePanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (!playedSplashScreen)
        {
            if (!splashscreenTexture.isPlaying)
            {
                Destroy(splashscreen);
                if (!mainMenuSound.isPlaying)
                    mainMenuSound.Play();
            }
        }
        else
        {
            if (!mainMenuSound.isPlaying)
                mainMenuSound.Play();
        }
    }

    public void goToGamemode()
    {
        mainPanel.SetActive(false);
        gamemodePanel.SetActive(true);
    }

    public void goToMainMenu()
    {
        mainPanel.SetActive(true);
        gamemodePanel.SetActive(false);
    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
