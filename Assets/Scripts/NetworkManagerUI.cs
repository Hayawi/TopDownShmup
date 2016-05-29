#if ENABLE_UNET

namespace UnityEngine.Networking
{
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;
    using System.Collections;
    using System;
    using UnityEngine.Networking.Types;
    using UnityEngine.Networking.Match;
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

        List<MatchDesc> matchList = new List<MatchDesc>();

        bool matchCreated;

        NetworkMatch networkMatch;

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
            //networkMatch = gameObject.AddComponent<NetworkMatch>();
            PlayerPrefs.SetString("CloudNetworkingId", "1093751");
            manager.StartMatchMaker();
            manager.matchMaker.SetProgramAppID((AppID)1093751);
        }

        public void createRoom()
        {
            if (manager.matchMaker == null)
            {
                manager.StartMatchMaker();
                manager.matchMaker.SetProgramAppID((AppID)1093751);
            }
            if (playerName.text != "")
            {
                CreateMatchRequest create = new CreateMatchRequest();
                create.name = playerName.text;
                create.size = 4;
                create.advertise = true;
                create.password = "";

                manager.matchMaker.CreateMatch(create, OnMatchCreate);
            }
            else
            {
                noPlayerName.enabled = true;
            }
        }

        public void listRooms()
        {
            if (manager.matchMaker == null)
            {
                manager.StartMatchMaker();
                manager.matchMaker.SetProgramAppID((AppID)1093751);
            }
            if (playerName.text != "")
            {
                manager.matchMaker.ListMatches(0, 20, "", OnMatchList);
            }
        }

        void OnGUI()
        {
            // You would normally not join a match you created yourself but this is possible here for demonstration purposes.
            if (GUILayout.Button("Create Room"))
            {
                CreateMatchRequest create = new CreateMatchRequest();
                create.name = "NewRoom";
                create.size = 4;
                create.advertise = true;
                create.password = "";

                manager.matchMaker.CreateMatch(create, OnMatchCreate);
            }

            if (GUILayout.Button("List rooms"))
            {
                manager.matchMaker.ListMatches(0, 20, "", OnMatchList);
            }

            if (matchList.Count > 0)
            {
                print("HEllo");
                GUILayout.Label("Current rooms");
            }
            foreach (var match in matchList)
            {
                if (GUILayout.Button(match.name))
                {
                    manager.matchMaker.JoinMatch(match.networkId, "", OnMatchJoined);
                }
            }
        }

        public void OnMatchCreate(CreateMatchResponse matchResponse)
        {
            if (matchResponse.success)
            {
                Debug.Log("Create match succeeded");
                matchCreated = true;
                Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
                NetworkServer.Listen(new MatchInfo(matchResponse), 9000);
                startHost();
            }
            else
            {
                Debug.LogError("Create match failed");
            }
        }

        public void OnMatchList(ListMatchResponse matchListResponse)
        {
            if (matchListResponse.success && matchListResponse.matches != null)
            {
                manager.matchMaker.JoinMatch(matchListResponse.matches[0].networkId, "", OnMatchJoined);
            }
        }

        public void OnMatchJoined(JoinMatchResponse matchJoin)
        {
            if (matchJoin.success)
            {
                Debug.Log("Join match succeeded");
                if (matchCreated)
                {
                    Debug.LogWarning("Match already set up, aborting...");
                    return;
                }
                Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
                NetworkClient myClient = new NetworkClient();
                myClient.RegisterHandler(MsgType.Connect, OnConnected);
                myClient.Connect(new MatchInfo(matchJoin));
                startClientMatchmaking();
            }
            else
            {
                Debug.LogError("Join match failed");
            }
        }

        public void OnConnected(NetworkMessage msg)
        {
            Debug.Log("Connected!");
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

            if (Input.GetButtonDown("Pause") && ((NetworkClient.active && manager.IsClientConnected()) || NetworkServer.active /*manager.matchMaker != null*/) && !ending)
            {
                pausePanel.SetActive(!pausePanel.activeSelf);
            }

            if (Input.GetButton("Leaderboards") && ((NetworkClient.active && manager.IsClientConnected()) || NetworkServer.active /*manager.matchMaker != null*/) && !ending)
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

            if (!NetworkClient.active && !NetworkServer.active/*manager.matchMaker == null*/)
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
            
            if ((NetworkClient.active && manager.IsClientConnected()) || NetworkServer.active /*manager.matchMaker != null*/)
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

        public void startClientMatchmaking()
        {
            noPlayerName.enabled = false;
            if (playerName.text != "")
            {
                chosenPlayerName = playerName.text;
                //playerNames.Add(chosenPlayerName);
                manager.StartClient();
            }
            if (playerName.text == "")
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
    }
};
#endif //ENABLE_UNET
