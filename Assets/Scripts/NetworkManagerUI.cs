#if ENABLE_UNET

namespace UnityEngine.Networking
{
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using System.Collections;
    using System;
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class NetworkManagerUI : NetworkBehaviour
    {
        public NetworkManager manager;
        [SerializeField]
        public bool showGUI = true;
        [SerializeField]
        public int offsetX;
        [SerializeField]
        public int offsetY;

        public Canvas networkConfigCanvas;

        public InputField playerName;

        public InputField ipAddress;

        public Text noPlayerName;

        public Text noIP;

        public GameObject connectingPanel;

        public GameObject configPanel;

        public Text attemptingConnect;

        public GameObject panelToToggleActive;

        public GameObject pausePanel;

        public GameObject leaderboardPanel;

        public string chosenPlayerName;

        public Text playerHeading;

        public Text killHeading;

        public Text deathHeading;

        public Text whoWon;

        Text winner;

        ArrayList playerKills = new ArrayList(); //GUI text

        ArrayList playerDeaths = new ArrayList(); //GUI text

        ArrayList playersInGame = new ArrayList(); // GUI text componenets

        [SyncVar]
        public ArrayList playerNames = new ArrayList(); // Actual Names

        [SyncVar]
        ArrayList playerNetIDs = new ArrayList();

        [SyncVar]
        public int gameTime = 600;

        [SyncVar]
        public int originalGameTime = 600;

        public Text gameTimerText;

        NetworkViewID localPlayer;

        bool ending = false;

        ArrayList scores = new ArrayList();

        // Runtime variable
        bool showServer = false;

        void Awake()
        {
            originalGameTime *= 60;
            gameTime = originalGameTime;
            leaderboardPanel.SetActive(false);
            pausePanel.SetActive(false);
            noPlayerName.enabled = false;
            noIP.enabled = false;
            connectingPanel.SetActive(false);
            manager = gameObject.GetComponent<NetworkManager>();
        }

        public void updateGameTime(int playerGameTime)
        {
            gameTime = playerGameTime;
        }

        void Update()
        {
            if ((gameTime % 3600) / 60 > 9)
                gameTimerText.text = gameTime / 3600 + ":" + (gameTime % 3600) / 60;
            else
                gameTimerText.text = gameTime / 3600 + ":0" + (gameTime % 3600) / 60;

            if (gameTime <= 0)
            {
                scores.Clear();
                ending = true;
                if (winner != null)
                    Destroy(winner.gameObject);
                winner = (Text)Instantiate(whoWon, whoWon.transform.position, whoWon.transform.rotation);
                winner.enabled = true;
                winner.gameObject.SetActive(true);
                winner.transform.SetParent(whoWon.transform.parent);
                try {
                    for (int i = 0; i < playerKills.Count; i++)
                    {
                        ((Text)playerDeaths[i]).text = (ClientScene.FindLocalObject((NetworkInstanceId)playerNetIDs[i]).GetComponent<PlayerSetup>().deaths).ToString();
                        ((Text)playerKills[i]).text = (ClientScene.FindLocalObject((NetworkInstanceId)playerNetIDs[i]).GetComponent<PlayerSetup>().kills).ToString();
                        scores.Add(int.Parse(((Text)playerKills[i]).text) - int.Parse(((Text)playerDeaths[i]).text));
                    }
                } catch (Exception e)
                {
                    disconnect();
                }
                int indexOfWinner = 0;
                for (int i = 0; i < scores.Count; i++)
                {
                    if ((int)scores[i] > (int)scores[indexOfWinner])
                    {
                        indexOfWinner = i;
                    }
                }
                winner.text = (string)playerNames[indexOfWinner] + " has won with a score of " + ((Text)playerKills[indexOfWinner]).text + " and " + ((Text)playerDeaths[indexOfWinner]).text;
                if (gameTime < -180)
                    disconnect();
            }

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length != playerNames.Count)
            {
                playerNames.Clear();
                playerNetIDs.Clear();
                for (int i = 0; i < playersInGame.Count; i++)
                {
                    Destroy((Text)playersInGame[i]);
                    Destroy((Text)playerDeaths[i]);
                    Destroy((Text)playerKills[i]);
                }
                playerDeaths.Clear();
                playerKills.Clear();
                playersInGame.Clear();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].GetComponent<PlayerSetup>().playerName != "")
                    {
                        playerNames.Add(players[i].GetComponent<PlayerSetup>().playerName);
                        playerNetIDs.Add(players[i].GetComponent<NetworkIdentity>().netId);
                        print(playerNames[i]);
                        playersInGame.Add((Text)Instantiate(playerHeading, new Vector3(playerHeading.transform.position.x, playerHeading.transform.position.y + (i + 1) * -35, 0), playerHeading.transform.rotation));
                        ((Text)playersInGame[i]).transform.SetParent(leaderboardPanel.transform);
                        ((Text)playersInGame[i]).text = (string)playerNames[i];
                        playerKills.Add((Text)Instantiate(killHeading, new Vector3(killHeading.transform.position.x, killHeading.transform.position.y + (i + 1) * -35, 0), killHeading.transform.rotation));
                        ((Text)playerKills[i]).transform.SetParent(leaderboardPanel.transform);
                        ((Text)playerKills[i]).text = (ClientScene.FindLocalObject((NetworkInstanceId)playerNetIDs[i]).GetComponent<PlayerSetup>().kills).ToString();
                        playerDeaths.Add((Text)Instantiate(deathHeading, new Vector3(deathHeading.transform.position.x, deathHeading.transform.position.y + (i + 1) * -35, 0), deathHeading.transform.rotation));
                        ((Text)playerDeaths[i]).transform.SetParent(leaderboardPanel.transform);
                        ((Text)playerDeaths[i]).text = (ClientScene.FindLocalObject((NetworkInstanceId)playerNetIDs[i]).GetComponent<PlayerSetup>().deaths).ToString();
                    }
                }
            }

            if (!showGUI)
                return;

            if (Input.GetButtonDown("Pause") && ((NetworkClient.active && manager.IsClientConnected()) || NetworkServer.active) && !ending)
            {
                pausePanel.SetActive(!pausePanel.activeSelf);
            }

            if (Input.GetButton("Leaderboards") && ((NetworkClient.active && manager.IsClientConnected()) || NetworkServer.active) && !ending)
            {
                for (int i = 0; i < playerKills.Count; i++)
                {
                    ((Text)playerDeaths[i]).text = (ClientScene.FindLocalObject((NetworkInstanceId)playerNetIDs[i]).GetComponent<PlayerSetup>().deaths).ToString();
                    ((Text)playerKills[i]).text = (ClientScene.FindLocalObject((NetworkInstanceId)playerNetIDs[i]).GetComponent<PlayerSetup>().kills).ToString();
                }
                leaderboardPanel.SetActive(true);
            }
            else
            {
                leaderboardPanel.SetActive(false);
            }

            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                if (winner != null)
                    Destroy(winner.gameObject);
                ending = false;
                gameTime = originalGameTime;
                if (playerNames.Count > 0)
                {
                    playerNames.Clear();
                    for (int i = 0; i < playersInGame.Count; i++)
                    {
                        Destroy((Text)playersInGame[i]);
                        Destroy((Text)playerDeaths[i]);
                        Destroy((Text)playerKills[i]);
                    }
                    playerDeaths.Clear();
                    playerKills.Clear();
                    playersInGame.Clear();
                }
                leaderboardPanel.SetActive(false);
                pausePanel.SetActive(false);
                panelToToggleActive.SetActive(true);
                configPanel.SetActive(true);
                connectingPanel.SetActive(false);
            }
            
            if ((NetworkClient.active && manager.IsClientConnected()) || NetworkServer.active)
            {
                panelToToggleActive.SetActive(false);
            }
            else
            {
                panelToToggleActive.SetActive(true);
            }

            if (!manager.IsClientConnected() && NetworkClient.active)
            {
                connectingPanel.SetActive(true);
                configPanel.SetActive(false);
                attemptingConnect.GetComponent<Text>().text = "Attempting to Connect to " + manager.networkAddress;
            }
            /*if (Input.GetKeyDown(KeyCode.S))
            {
                manager.StartServer();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                manager.StartHost();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                manager.StartClient();
            }
        }
        if (NetworkServer.active && NetworkClient.active)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                manager.StopHost();
            }
        }*/
        }

        public void closePauseScreen()
        {
            pausePanel.SetActive(false);
        }

        public void disconnect()
        {
            manager.StopHost();
            pausePanel.SetActive(false);
        }

        public void exitToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void exitGame()
        {
            Application.Quit();
        }

        public void cancelConnect()
        {
            manager.StopClient();
            configPanel.SetActive(true);
            connectingPanel.SetActive(false);
        }

        public void startHost()
        {
            noPlayerName.enabled = false;
            noIP.enabled = false;
            if (playerName.text != "")
            {
                chosenPlayerName = playerName.text;
                //playerNames.Add(chosenPlayerName);
                manager.StartHost();
            }
            else
            {
                noPlayerName.enabled = true;
            }
        }

        public void startClient()
        {
            noPlayerName.enabled = false;
            noIP.enabled = false;
            if (ipAddress.text != "" && playerName.text != "")
            {
                chosenPlayerName = playerName.text;
                //playerNames.Add(chosenPlayerName);
                manager.networkAddress = ipAddress.text;
                manager.StartClient();
            }
            if (ipAddress.text == "")
            {
                noIP.enabled = true;
            }
            if (playerName.text == "")
            {
                noPlayerName.enabled = true;
            }
        }

        public void backToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
            Destroy(manager.gameObject);
        }

        /*void OnGUI()
        {
            if (!showGUI)
                return;

            int xpos = 10 + offsetX;
            int ypos = 40 + offsetY;
            int spacing = 24;

            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "LAN Host(H)"))
                {
                    manager.StartHost();
                }
                ypos += spacing;

                if (GUI.Button(new Rect(xpos, ypos, 105, 20), "LAN Client(C)"))
                {
                    manager.StartClient();
                }
                manager.networkAddress = GUI.TextField(new Rect(xpos + 100, ypos, 95, 20), manager.networkAddress);
                ypos += spacing;

                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "LAN Server Only(S)"))
                {
                    manager.StartServer();
                }
                ypos += spacing;
            }
            else
            {
                if (NetworkServer.active)
                {
                    GUI.Label(new Rect(xpos, ypos, 300, 20), "Server: port=" + manager.networkPort);
                    ypos += spacing;
                }
                if (NetworkClient.active)
                {
                    GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                    ypos += spacing;
                }
            }

            if (NetworkClient.active && !ClientScene.ready)
            {
                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Client Ready"))
                {
                    ClientScene.Ready(manager.client.connection);

                    if (ClientScene.localPlayers.Count == 0)
                    {
                        ClientScene.AddPlayer(0);
                    }
                }
                ypos += spacing;
            }

            if (NetworkServer.active || NetworkClient.active)
            {
                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Stop (X)"))
                {
                    manager.StopHost();
                }
                ypos += spacing;
            }

            if (!NetworkServer.active && !NetworkClient.active)
            {
                ypos += 10;

                if (manager.matchMaker == null)
                {
                    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Enable Match Maker (M)"))
                    {
                        manager.StartMatchMaker();
                    }
                    ypos += spacing;
                }
                else
                {
                    if (manager.matchInfo == null)
                    {
                        if (manager.matches == null)
                        {
                            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Create Internet Match"))
                            {
                                manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", manager.OnMatchCreate);
                            }
                            ypos += spacing;

                            GUI.Label(new Rect(xpos, ypos, 100, 20), "Room Name:");
                            manager.matchName = GUI.TextField(new Rect(xpos + 100, ypos, 100, 20), manager.matchName);
                            ypos += spacing;

                            ypos += 10;

                            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Find Internet Match"))
                            {
                                manager.matchMaker.ListMatches(0, 20, "", manager.OnMatchList);
                            }
                            ypos += spacing;
                        }
                        else
                        {
                            foreach (var match in manager.matches)
                            {
                                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Join Match:" + match.name))
                                {
                                    manager.matchName = match.name;
                                    manager.matchSize = (uint)match.currentSize;
                                    manager.matchMaker.JoinMatch(match.networkId, "", manager.OnMatchJoined);
                                }
                                ypos += spacing;
                            }
                        }
                    }

                    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Change MM server"))
                    {
                        showServer = !showServer;
                    }
                    if (showServer)
                    {
                        ypos += spacing;
                        if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Local"))
                        {
                            manager.SetMatchHost("localhost", 1337, false);
                            showServer = false;
                        }
                        ypos += spacing;
                        if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Internet"))
                        {
                            manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
                            showServer = false;
                        }
                        ypos += spacing;
                        if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Staging"))
                        {
                            manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
                            showServer = false;
                        }
                    }

                    ypos += spacing;

                    GUI.Label(new Rect(xpos, ypos, 300, 20), "MM Uri: " + manager.matchMaker.baseUri);
                    ypos += spacing;

                    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Disable Match Maker"))
                    {
                        manager.StopMatchMaker();
                    }
                    ypos += spacing;
                }
            }
        }*/

    }
};
#endif //ENABLE_UNET
