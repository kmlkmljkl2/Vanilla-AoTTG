using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class FengGameManagerMKII : Photon.MonoBehaviour
{
    public static FengGameManagerMKII Instance { get; private set; }
    public static InRoomChat InroomChat { get; set; }

    public static FengCustomInputs InputManager { get; set; }

    public FengCustomInputs inputManager;

    public static readonly string applicationId = "";

    public int difficulty;

    private GameObject ui;

    public bool needChooseSide;

    public bool justSuicide;

    private ArrayList chatContent;

    private string myLastHero;

    private string myLastRespawnTag = "playerRespawn";

    public float myRespawnTime;

    private int titanScore;

    private int humanScore;

    public int PVPtitanScore;

    public int PVPhumanScore;

    private int PVPtitanScoreMax = 200;

    private int PVPhumanScoreMax = 200;

    private bool isWinning;

    private bool isLosing;

    private bool isPlayer1Winning;

    private bool isPlayer2Winning;

    private int teamWinner;

    private int[] teamScores;

    private float gameEndCD;

    private float gameEndTotalCDtime = 9f;

    public int wave = 1;

    private int highestwave = 1;

    public static string Level = string.Empty;

    public int time = 600;

    private float timeElapse;

    public float roundTime;

    private float timeTotalServer;

    private float maxSpeed;

    private float currentSpeed;

    public static bool LAN;

    private bool startRacing;

    private bool endRacing;

    public GameObject checkpoint;

    private ArrayList racingResult;

    private ArrayList kicklist;

    private bool gameTimesUp;

    private IN_GAME_MAIN_CAMERA mainCamera;

    public bool gameStart;

    private List<HERO> heroes;

    private List<TITAN_EREN> ErenTitans;

    private List<TITAN> titans;

    private List<FEMALE_TITAN> Female_Titans;

    private List<COLOSSAL_TITAN> Colossal_Titans;

    private List<Bullet> hooks;

    private string localRacingResult;

    private int single_kills;

    private int single_maxDamage;

    private int single_totalDamage;

    private ArrayList killInfoGO = new ArrayList();

    private void Start()
    {
        Instance = this;
        gameObject.name = "MultiplayerManager";
        HeroCostume.init();
        CharacterMaterials.init();
        DontDestroyOnLoad(base.gameObject);
        heroes = new List<HERO>();
        ErenTitans = new List<TITAN_EREN>();
        titans = new List<TITAN>();
        Female_Titans = new List<FEMALE_TITAN>();
        Colossal_Titans = new List<COLOSSAL_TITAN>();
        hooks = new List<Bullet>();
        Application.targetFrameRate = 144;
        QualitySettings.vSyncCount = 0;
        BackroundTexture.SetPixel(0, 0, Color.black);
        BackroundTexture.Apply();
    }
    private Rect ScaledRect(Rect _rect)
    {
        float FilScreenWidth = _rect.width / 800;
        float rectWidth = FilScreenWidth * Screen.width;
        float FilScreenHeight = _rect.height / 600;
        float rectHeight = FilScreenHeight * Screen.height;
        float rectX = (_rect.x / 800) * Screen.width;
        float rectY = (_rect.y / 600) * Screen.height;

        return new Rect(rectX, rectY, rectWidth, rectHeight);
    }
    public static string UserNameField
    {
        get
        {
            if (PlayerPrefs.HasKey("username"))
            {
                LoginFengKAI.player.name = PlayerPrefs.GetString("username");
                return PlayerPrefs.GetString("username");
            }

            return "";
        }
        set
        {
            LoginFengKAI.player.name = value;
            PlayerPrefs.SetString("username", value);
        }
    }
    public static string GuildNameField
    {
        get
        {
            if (PlayerPrefs.HasKey("guild"))
            {
                LoginFengKAI.player.guildname = PlayerPrefs.GetString("guild");
                return PlayerPrefs.GetString("guild");
            }

            return "";
        }
        set
        {
            LoginFengKAI.player.guildname= value;
            PlayerPrefs.SetString("guild", value);
        }
    }
    private int left = 0;
    private int top = 5;
    private int width = 120;
    private int heigt = 20;

    Texture2D BackroundTexture = new Texture2D(1,1, TextureFormat.ARGB32, false);
    private int FontSize;
    public void OnGUI()
    {
        if(Application.loadedLevelName == "menu")
        {
            GUI.DrawTexture(ScaledRect(new Rect(0, 0,180, 60)), BackroundTexture);
            var Style = new GUIStyle() { fontSize = Screen.height / 40};
            Style.normal.textColor = Color.white;

            GUI.Label(ScaledRect(new Rect(5, 5, 45, 20)), "Name: ", Style);
            GUI.Label(ScaledRect(new Rect(5, 30, 45, 20)), "Guild: ", Style);



            Style = new GUIStyle(GUI.skin.textField) { fontSize = Screen.height / 40 };


            UserNameField = GUI.TextField(ScaledRect(new Rect(55, 5, 120, 20)), UserNameField, 100, Style);
            GuildNameField = GUI.TextField(ScaledRect(new Rect(55, 30, 120, 20)), GuildNameField, 100, Style);

        }
        //GUI.Box(ScaledRect(new Rect(left, top, width, heigt)), "");
        //left = int.Parse(GUI.TextField(ScaledRect(new Rect(100, 100, 100, 20)), left.ToString()));
        //top = int.Parse(GUI.TextField(ScaledRect(new Rect(100, 130, 100, 20)), top.ToString()));

        //width = int.Parse(GUI.TextField(ScaledRect(new Rect(100, 160, 100, 20)), width.ToString()));

        //heigt = int.Parse(GUI.TextField(ScaledRect(new Rect(100, 190, 100, 20)), heigt.ToString()));



    }

    public void AddHero(HERO hero)
    {
        heroes.Add(hero);
    }

    public void RemoveHero(HERO hero)
    {
        heroes.Remove(hero);
    }

    public void AddHook(Bullet h)
    {
        hooks.Add(h);
    }

    public void RemoveHook(Bullet h)
    {
        hooks.Remove(h);
    }

    public void AddET(TITAN_EREN hero)
    {
        ErenTitans.Add(hero);
    }

    public void RemoveET(TITAN_EREN hero)
    {
        ErenTitans.Remove(hero);
    }

    public void AddTitan(TITAN titan)
    {
        titans.Add(titan);
    }

    public void RemoveTitan(TITAN titan)
    {
        titans.Remove(titan);
    }

    public void AddFT(FEMALE_TITAN titan)
    {
        Female_Titans.Add(titan);
    }

    public void RemoveFT(FEMALE_TITAN titan)
    {
        Female_Titans.Remove(titan);
    }

    public void AddCT(COLOSSAL_TITAN titan)
    {
        Colossal_Titans.Add(titan);
    }

    public void RemoveCT(COLOSSAL_TITAN titan)
    {
        Colossal_Titans.Remove(titan);
    }

    public void AddCamera(IN_GAME_MAIN_CAMERA c)
    {
        mainCamera = c;
    }

    private void LateUpdate()
    {
        if (!gameStart)
        {
            return;
        }
        core();
    }

    private void Update()
    {
        mainCamera?.snapShotUpdate();

        //mainCamera?.Update();
        if (IN_GAME_MAIN_CAMERA.GameType != 0 && (bool)GameObject.Find("LabelNetworkStatus"))
        {
            GameObject.Find("LabelNetworkStatus").GetComponent<UILabel>().text = PhotonNetwork.ConnectionStateDetailed.ToString();
            if (PhotonNetwork.Connected)
            {
                UILabel component = GameObject.Find("LabelNetworkStatus").GetComponent<UILabel>();
                component.text = component.text + " ping:" + PhotonNetwork.GetPing();
            }
        }
        return;
        if (!gameStart)
        {
            return;
        }
        int i;
        for (i = 0; i < hooks.Count; i++)
        {
            hooks[i].Update();
        }
        for (i = 0; i < heroes.Count; i++)
        {
            heroes[i].Update();
        }
        for (i = 0; i < titans.Count; i++)
        {
            titans[i].Update();
        }

        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || PhotonNetwork.IsMasterClient)
        {
            for (i = 0; i < ErenTitans.Count; i++)
            {
                ErenTitans[i].Update();
            }
            for (i = 0; i < Female_Titans.Count; i++)
            {
                Female_Titans[i].Update();
            }
            for (i = 0; i < Colossal_Titans.Count; i++)
            {
                Colossal_Titans[i].Update();
            }
        }

      

    }

    private void core()
    {
        if (IN_GAME_MAIN_CAMERA.GameType != 0 && needChooseSide)
        {
            if (InputManager.isInputDown[InputCode.flare1])
            {
                if (NGUITools.GetActive(ui.GetComponent<UIReferArray>().panels[3]))
                {
                    Screen.lockCursor = true;
                    Screen.showCursor = true;
                    NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: true);
                    NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
                    NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
                    NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
                    IN_GAME_MAIN_CAMERA.Camera.GetComponent<SpectatorMovement>().disable = false;
                    IN_GAME_MAIN_CAMERA.Camera.GetComponent<MouseLook>().disable = false;
                }
                else
                {
                    Screen.lockCursor = false;
                    Screen.showCursor = true;
                    NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: false);
                    NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
                    NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
                    NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: true);
                    IN_GAME_MAIN_CAMERA.Camera.GetComponent<SpectatorMovement>().disable = true;
                    IN_GAME_MAIN_CAMERA.Camera.GetComponent<MouseLook>().disable = true;
                }
            }
            if (InputManager.isInputDown[15] && !NGUITools.GetActive(ui.GetComponent<UIReferArray>().panels[3]))
            {
                NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: false);
                NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: true);
                NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
                NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
                Screen.showCursor = true;
                Screen.lockCursor = false;
                IN_GAME_MAIN_CAMERA.Camera.GetComponent<SpectatorMovement>().disable = true;
                IN_GAME_MAIN_CAMERA.Camera.GetComponent<MouseLook>().disable = true;
                InputManager.showKeyMap();
                InputManager.justUPDATEME();
                InputManager.menuOn = true;
            }
        }
        if (IN_GAME_MAIN_CAMERA.GameType != 0 && IN_GAME_MAIN_CAMERA.GameType != GameType.Multiplayer)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
        {
            //string text = string.Empty;
            //PhotonPlayer[] playerList = PhotonNetwork.playerList;
            //PlayerList

            ShowHUDInfoTopLeft(PhotonPlayer.GetPlayerList());
            if (IN_GAME_MAIN_CAMERA.Camera != null && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.RACING && IN_GAME_MAIN_CAMERA.Camera.gameOver && !needChooseSide)
            {
                ShowHUDInfoCenter("Press [F7D358]" + InputManager.inputString[InputCode.flare1] + "[-] to spectate the next player. \nPress [F7D358]" + InputManager.inputString[InputCode.flare2] + "[-] to spectate the previous player.\nPress [F7D358]" + InputManager.inputString[InputCode.attack1] + "[-] to enter the spectator mode.\n\n\n\n");
                if (LevelInfo.getInfo(Level).respawnMode == RespawnMode.DEATHMATCH)
                {
                    myRespawnTime += Time.deltaTime;
                    int num = 10;
                    if ((int)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.isTitan] == 2)
                    {
                        num = 15;
                    }
                    ShowHUDInfoCenterADD("Respawn in " + (int)((float)num - myRespawnTime) + "s.");
                    if (myRespawnTime > (float)num)
                    {
                        myRespawnTime = 0f;
                        IN_GAME_MAIN_CAMERA.Camera.gameOver = false;
                        if ((int)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.isTitan] == 2)
                        {
                            SpawnNonAITitan(myLastHero);
                        }
                        else
                        {
                            SpawnPlayer(myLastHero, myLastRespawnTag);
                        }
                        IN_GAME_MAIN_CAMERA.Camera.gameOver = false;
                        ShowHUDInfoCenter(string.Empty);
                    }
                }
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
            {
                if (!isLosing)
                {
                    currentSpeed = IN_GAME_MAIN_CAMERA.Camera.main_object.rigidbody.velocity.magnitude;
                    maxSpeed = Mathf.Max(maxSpeed, currentSpeed);
                    ShowHUDInfoTopLeft("Current Speed : " + (int)currentSpeed + "\nMax Speed:" + maxSpeed);
                }
            }
            else
            {
                ShowHUDInfoTopLeft("Kills:" + single_kills + "\nMax Damage:" + single_maxDamage + "\nTotal Damage:" + single_totalDamage);
            }
        }
        if (isLosing && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.RACING)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                {
                    ShowHUDInfoCenter("Survive " + wave + " Waves!\n Press " + InputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                }
                else
                {
                    ShowHUDInfoCenter("Humanity Fail!\n Press " + InputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                }
            }
            else
            {
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                {
                    ShowHUDInfoCenter("Survive " + wave + " Waves!\nGame Restart in " + (int)gameEndCD + "s\n\n");
                }
                else
                {
                    ShowHUDInfoCenter("Humanity Fail!\nAgain!\nGame Restart in " + (int)gameEndCD + "s\n\n");
                }
                if (gameEndCD <= 0f)
                {
                    gameEndCD = 0f;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        restartGame();
                    }
                    ShowHUDInfoCenter(string.Empty);
                }
                else
                {
                    gameEndCD -= Time.deltaTime;
                }
            }
        }
        if (isWinning)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
                {
                    ShowHUDInfoCenter((float)(int)(timeTotalServer * 10f) * 0.1f - 5f + "s !\n Press " + InputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                {
                    ShowHUDInfoCenter("Survive All Waves!\n Press " + InputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                }
                else
                {
                    ShowHUDInfoCenter("Humanity Win!\n Press " + InputManager.inputString[InputCode.restart] + " to Restart.\n\n\n");
                }
            }
            else
            {
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
                {
                    ShowHUDInfoCenter(localRacingResult + "\n\nGame Restart in " + (int)gameEndCD + "s");
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                {
                    ShowHUDInfoCenter("Survive All Waves!\nGame Restart in " + (int)gameEndCD + "s\n\n");
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
                {
                    ShowHUDInfoCenter("Team " + teamWinner + " Win!\nGame Restart in " + (int)gameEndCD + "s\n\n");
                }
                else
                {
                    ShowHUDInfoCenter("Humanity Win!\nGame Restart in " + (int)gameEndCD + "s\n\n");
                }
                if (gameEndCD <= 0f)
                {
                    gameEndCD = 0f;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        restartGame();
                    }
                    ShowHUDInfoCenter(string.Empty);
                }
                else
                {
                    gameEndCD -= Time.deltaTime;
                }
            }
        }
        timeElapse += Time.deltaTime;
        roundTime += Time.deltaTime;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
            {
                if (!isWinning)
                {
                    timeTotalServer += Time.deltaTime;
                }
            }
            else if (!isLosing && !isWinning)
            {
                timeTotalServer += Time.deltaTime;
            }
        }
        else
        {
            timeTotalServer += Time.deltaTime;
        }
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                if (!isWinning)
                {
                    ShowHUDInfoTopCenter("Time : " + ((float)(int)(timeTotalServer * 10f) * 0.1f - 5f));
                }
                if (timeTotalServer < 5f)
                {
                    ShowHUDInfoCenter("RACE START IN " + (int)(5f - timeTotalServer));
                }
                else if (!startRacing)
                {
                    ShowHUDInfoCenter(string.Empty);
                    startRacing = true;
                    endRacing = false;
                    GameObject.Find("door").SetActive(value: false);
                }
            }
            else
            {
                ShowHUDInfoTopCenter("Time : " + ((!(roundTime < 20f)) ? ((float)(int)(roundTime * 10f) * 0.1f - 20f).ToString() : "WAITING"));
                if (roundTime < 20f)
                {
                    ShowHUDInfoCenter("RACE START IN " + (int)(20f - roundTime) + ((!(localRacingResult == string.Empty)) ? ("\nLast Round\n" + localRacingResult) : "\n\n"));
                }
                else if (!startRacing)
                {
                    ShowHUDInfoCenter(string.Empty);
                    startRacing = true;
                    endRacing = false;
                    GameObject.Find("door").SetActive(value: false);
                }
            }
            if (IN_GAME_MAIN_CAMERA.Camera.gameOver && !needChooseSide)
            {
                myRespawnTime += Time.deltaTime;
                if (myRespawnTime > 1.5f)
                {
                    myRespawnTime = 0f;
                    IN_GAME_MAIN_CAMERA.Camera.gameOver = false;
                    if (checkpoint != null)
                    {
                        SpawnPlayerAt(myLastHero, checkpoint);
                    }
                    else
                    {
                        SpawnPlayer(myLastHero, myLastRespawnTag);
                    }
                    IN_GAME_MAIN_CAMERA.Camera.gameOver = false;
                    ShowHUDInfoCenter(string.Empty);
                }
            }
        }
        if (timeElapse > 1f)
        {
            timeElapse -= 1f;
            string text3 = string.Empty;
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
            {
                text3 += "Time : ";
                text3 += (int)((float)time - timeTotalServer);
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
            {
                text3 = "Titan Left: ";
                text3 += GameObject.FindGameObjectsWithTag("titan").Length;
                text3 += "  Time : ";
                text3 = ((IN_GAME_MAIN_CAMERA.GameType != 0) ? (text3 + (int)((float)time - timeTotalServer)) : (text3 + (int)timeTotalServer));
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
            {
                text3 = "Titan Left: ";
                text3 += GameObject.FindGameObjectsWithTag("titan").Length;
                text3 += " Wave : ";
                text3 += wave;
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
            {
                text3 = "Time : ";
                text3 += (int)((float)time - timeTotalServer);
                text3 += "\nDefeat the Colossal Titan.\nPrevent abnormal titan from running to the north gate";
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                string text4 = "| ";
                for (int j = 0; j < PVPcheckPoint.chkPts.Count; j++)
                {
                    text4 = text4 + (PVPcheckPoint.chkPts[j] as PVPcheckPoint).getStateString() + " ";
                }
                text4 += "|";
                text3 = PVPtitanScoreMax - PVPtitanScore + "  " + text4 + "  " + (PVPhumanScoreMax - PVPhumanScore) + "\n";
                text3 += "Time : ";
                text3 += (int)((float)time - timeTotalServer);
            }
            ShowHUDInfoTopCenter(text3);
            text3 = string.Empty;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                {
                    text3 = "Time : ";
                    text3 += (int)timeTotalServer;
                }
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
            {
                text3 = "Humanity " + humanScore + " : Titan " + titanScore + " ";
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                text3 = "Humanity " + humanScore + " : Titan " + titanScore + " ";
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.CAGE_FIGHT)
            {
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                {
                    text3 = "Time : ";
                    text3 += (int)((float)time - timeTotalServer);
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
                {
                    for (int k = 0; k < teamScores.Length; k++)
                    {
                        string text2 = text3;
                        text3 = text2 + ((k == 0) ? string.Empty : " : ") + "Team" + (k + 1) + " " + teamScores[k] + string.Empty;
                    }
                    text3 += "\nTime : ";
                    text3 += (int)((float)time - timeTotalServer);
                }
            }
            ShowHUDInfoTopRight(text3);
            string text5 = ((IN_GAME_MAIN_CAMERA.difficulty < 0) ? "Trainning" : ((IN_GAME_MAIN_CAMERA.difficulty == 0) ? "Normal" : ((IN_GAME_MAIN_CAMERA.difficulty != 1) ? "Abnormal" : "Hard")));
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
            {
                ShowHUDInfoTopRightMAPNAME((int)roundTime + "s\n" + Level + " : " + text5);
            }
            else
            {
                ShowHUDInfoTopRightMAPNAME("\n" + Level + " : " + text5);
            }
            ShowHUDInfoTopRightMAPNAME("\nCamera(" + InputManager.inputString[InputCode.camera] + "):" + IN_GAME_MAIN_CAMERA.cameraMode);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && needChooseSide)
            {
                ShowHUDInfoTopCenterADD("\n\nPRESS 1 TO ENTER GAME");
            }
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer && killInfoGO.Count > 0 && killInfoGO[0] == null)
        {
            killInfoGO.RemoveAt(0);
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || !PhotonNetwork.IsMasterClient || !(timeTotalServer > (float)time))
        {
            return;
        }
        IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
        gameStart = false;
        Screen.lockCursor = false;
        Screen.showCursor = true;
        string text6 = string.Empty;
        string text7 = string.Empty;
        string text8 = string.Empty;
        string text9 = string.Empty;
        string text10 = string.Empty;
        PhotonPlayer[] playerList2 = PhotonNetwork.PlayerList;
        foreach (PhotonPlayer photonPlayer2 in playerList2)
        {
            if (photonPlayer2 != null)
            {
                text6 = string.Concat(text6, photonPlayer2.CustomProperties[PhotonPlayerProperty.name], "\n");
                text7 = string.Concat(text7, photonPlayer2.CustomProperties[PhotonPlayerProperty.kills], "\n");
                text8 = string.Concat(text8, photonPlayer2.CustomProperties[PhotonPlayerProperty.deaths], "\n");
                text9 = string.Concat(text9, photonPlayer2.CustomProperties[PhotonPlayerProperty.max_dmg], "\n");
                text10 = string.Concat(text10, photonPlayer2.CustomProperties[PhotonPlayerProperty.total_dmg], "\n");
            }
        }
        string text11;
        if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.PVP_AHSS)
        {
            text11 = ((IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.SURVIVE_MODE) ? ("Humanity " + humanScore + " : Titan " + titanScore) : ("Highest Wave : " + highestwave));
        }
        else
        {
            text11 = string.Empty;
            for (int m = 0; m < teamScores.Length; m++)
            {
                text11 += ((m == 0) ? ("Team" + (m + 1) + " " + teamScores[m] + " ") : " : ");
            }
        }
        base.photonView.RPC("showResult", PhotonTargets.AllBuffered, text6, text7, text8, text9, text10, text11);
    }

    public void SpawnPlayer(string id, string tag = "playerRespawn")
    {
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
        {
            SpawnPlayerAt(id, checkpoint);
            return;
        }
        myLastRespawnTag = tag;
        GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
        GameObject pos = array[Random.Range(0, array.Length)];
        SpawnPlayerAt(id, pos);
    }

    public void SpawnPlayerAt(string id, GameObject pos)
    {
        IN_GAME_MAIN_CAMERA component = IN_GAME_MAIN_CAMERA.Camera;
        myLastHero = id.ToUpper();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            if (IN_GAME_MAIN_CAMERA.singleCharacter == "TITAN_EREN")
            {
                component.setMainObject((GameObject)Object.Instantiate(Resources.Load("TITAN_EREN"), pos.transform.position, pos.transform.rotation));
            }
            else
            {
                component.setMainObject((GameObject)Object.Instantiate(Resources.Load("AOTTG_HERO 1"), pos.transform.position, pos.transform.rotation));
                if (IN_GAME_MAIN_CAMERA.singleCharacter == "SET 1" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 2" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 3")
                {
                    HeroCostume heroCostume = CostumeConeveter.LocalDataToHeroCostume(IN_GAME_MAIN_CAMERA.singleCharacter);
                    heroCostume.checkstat();
                    CostumeConeveter.HeroCostumeToLocalData(heroCostume, IN_GAME_MAIN_CAMERA.singleCharacter);
                    component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                    if (heroCostume != null)
                    {
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = heroCostume;
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = heroCostume.stat;
                    }
                    else
                    {
                        heroCostume = HeroCostume.costumeOption[3];
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = heroCostume;
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(heroCostume.name.ToUpper());
                    }
                    component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                    component.main_object.GetComponent<HERO>().setStat();
                    component.main_object.GetComponent<HERO>().setSkillHUDPosition();
                }
                else
                {
                    for (int i = 0; i < HeroCostume.costume.Length; i++)
                    {
                        if (HeroCostume.costume[i].name.ToUpper() == IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper())
                        {
                            int num = HeroCostume.costume[i].id + CheckBoxCostume.costumeSet - 1;
                            if (HeroCostume.costume[num].name != HeroCostume.costume[i].name)
                            {
                                num = HeroCostume.costume[i].id + 1;
                            }
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[num];
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(HeroCostume.costume[num].name.ToUpper());
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                            component.main_object.GetComponent<HERO>().setStat();
                            component.main_object.GetComponent<HERO>().setSkillHUDPosition();
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            component.setMainObject(PhotonNetwork.Instantiate("AOTTG_HERO 1", pos.transform.position, pos.transform.rotation, 0));
            id = id.ToUpper();
            switch (id)
            {
                case "SET 1":
                case "SET 2":
                case "SET 3":
                    {
                        HeroCostume heroCostume2 = CostumeConeveter.LocalDataToHeroCostume(id);
                        heroCostume2.checkstat();
                        CostumeConeveter.HeroCostumeToLocalData(heroCostume2, id);
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                        if (heroCostume2 != null)
                        {
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = heroCostume2;
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = heroCostume2.stat;
                        }
                        else
                        {
                            heroCostume2 = HeroCostume.costumeOption[3];
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = heroCostume2;
                            component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(heroCostume2.name.ToUpper());
                        }
                        component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                        component.main_object.GetComponent<HERO>().setStat();
                        component.main_object.GetComponent<HERO>().setSkillHUDPosition();
                        break;
                    }
                default:
                    {
                        for (int j = 0; j < HeroCostume.costume.Length; j++)
                        {
                            if (HeroCostume.costume[j].name.ToUpper() == id.ToUpper())
                            {
                                int num2 = HeroCostume.costume[j].id;
                                if (id.ToUpper() != "AHSS")
                                {
                                    num2 += CheckBoxCostume.costumeSet - 1;
                                }
                                if (HeroCostume.costume[num2].name != HeroCostume.costume[j].name)
                                {
                                    num2 = HeroCostume.costume[j].id + 1;
                                }
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[num2];
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(HeroCostume.costume[num2].name.ToUpper());
                                component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
                                component.main_object.GetComponent<HERO>().setStat();
                                component.main_object.GetComponent<HERO>().setSkillHUDPosition();
                                break;
                            }
                        }
                        break;
                    }
            }
            CostumeConeveter.HeroCostumeToPhotonData(component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume, PhotonNetwork.Player);
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                component.main_object.transform.position += new Vector3(Random.Range(-20, 20), 2f, Random.Range(-20, 20));
            }
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add("dead", false);
            ExitGames.Client.Photon.Hashtable customProperties = hashtable;
            PhotonNetwork.Player.SetCustomProperties(customProperties);
            hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add(PhotonPlayerProperty.isTitan, 1);
            customProperties = hashtable;
            PhotonNetwork.Player.SetCustomProperties(customProperties);
        }
        component.enabled = true;
        IN_GAME_MAIN_CAMERA.Camera.setHUDposition();
        IN_GAME_MAIN_CAMERA.Camera.GetComponent<SpectatorMovement>().disable = true;
        IN_GAME_MAIN_CAMERA.Camera.GetComponent<MouseLook>().disable = true;
        component.gameOver = false;
        if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = false;
        isLosing = false;
        ShowHUDInfoCenter(string.Empty);
    }

    public void NOTSpawnPlayer(string id)
    {
        myLastHero = id.ToUpper();
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("dead", true);
        ExitGames.Client.Photon.Hashtable customProperties = hashtable;
        PhotonNetwork.Player.SetCustomProperties(customProperties);
        hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add(PhotonPlayerProperty.isTitan, 1);
        customProperties = hashtable;
        PhotonNetwork.Player.SetCustomProperties(customProperties);
        if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = false;
        ShowHUDInfoCenter("the game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
        IN_GAME_MAIN_CAMERA.Camera.enabled = true;
        IN_GAME_MAIN_CAMERA.Camera.setMainObject(null);
        IN_GAME_MAIN_CAMERA.Camera.setSpectorMode(val: true);
        IN_GAME_MAIN_CAMERA.Camera.gameOver = true;
    }

    public void NOTSpawnNonAITitan(string id)
    {
        myLastHero = id.ToUpper();
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("dead", true);
        ExitGames.Client.Photon.Hashtable customProperties = hashtable;
        PhotonNetwork.Player.SetCustomProperties(customProperties);
        hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add(PhotonPlayerProperty.isTitan, 2);
        customProperties = hashtable;
        PhotonNetwork.Player.SetCustomProperties(customProperties);
        if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = true;
        ShowHUDInfoCenter("the game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
        IN_GAME_MAIN_CAMERA.Camera.enabled = true;
        IN_GAME_MAIN_CAMERA.Camera.setMainObject(null);
        IN_GAME_MAIN_CAMERA.Camera.setSpectorMode(val: true);
        IN_GAME_MAIN_CAMERA.Camera.gameOver = true;
    }

    public void SpawnNonAITitan(string id, string tag = "titanRespawn")
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
        GameObject gameObject = array[Random.Range(0, array.Length)];
        myLastHero = id.ToUpper();
        GameObject gameObject2 = ((IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.PVP_CAPTURE) ? PhotonNetwork.Instantiate("TITAN_VER3.1", gameObject.transform.position, gameObject.transform.rotation, 0) : PhotonNetwork.Instantiate("TITAN_VER3.1", checkpoint.transform.position + new Vector3(Random.Range(-20, 20), 2f, Random.Range(-20, 20)), checkpoint.transform.rotation, 0));
        IN_GAME_MAIN_CAMERA.Camera.setMainObjectASTITAN(gameObject2);
        gameObject2.GetComponent<TITAN>().nonAI = true;
        gameObject2.GetComponent<TITAN>().speed = 30f;
        gameObject2.GetComponent<TITAN_CONTROLLER>().enabled = true;
        if (id == "RANDOM" && Random.Range(0, 100) < 7)
        {
            gameObject2.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER, forceCrawler: true);
        }
        IN_GAME_MAIN_CAMERA.Camera.enabled = true;
        IN_GAME_MAIN_CAMERA.Camera.GetComponent<SpectatorMovement>().disable = true;
        IN_GAME_MAIN_CAMERA.Camera.GetComponent<MouseLook>().disable = true;
        IN_GAME_MAIN_CAMERA.Camera.gameOver = false;
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("dead", false);
        ExitGames.Client.Photon.Hashtable customProperties = hashtable;
        PhotonNetwork.Player.SetCustomProperties(customProperties);
        hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add(PhotonPlayerProperty.isTitan, 2);
        customProperties = hashtable;
        PhotonNetwork.Player.SetCustomProperties(customProperties);
        if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
        {
            Screen.lockCursor = true;
        }
        else
        {
            Screen.lockCursor = false;
        }
        Screen.showCursor = true;
        ShowHUDInfoCenter(string.Empty);
    }

    public void OnConnectedToPhoton()
    {
        UnityEngine.MonoBehaviour.print("OnConnectedToPhoton");
    }

    public void OnLeftRoom()
    {
        UnityEngine.MonoBehaviour.print("OnLeftRoom");
        if (Application.loadedLevel != 0)
        {
            Time.timeScale = 1f;
            if (PhotonNetwork.Connected)
            {
                PhotonNetwork.Disconnect();
            }
            IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
            gameStart = false;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            InputManager.menuOn = false;
            Object.Destroy(GameObject.Find("MultiplayerManager"));
            Application.LoadLevel("menu");
        }
    }

    public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        UnityEngine.MonoBehaviour.print("OnMasterClientSwitched");
        if (!gameTimesUp && PhotonNetwork.IsMasterClient)
        {
            restartGame(masterclientSwitched: true);
        }
    }

    public void OnPhotonCreateRoomFailed()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonCreateRoomFailed");
    }

    public void OnPhotonJoinRoomFailed()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonJoinRoomFailed");
    }

    public void OnCreatedRoom()
    {
        kicklist = new ArrayList();
        racingResult = new ArrayList();
        teamScores = new int[2];
        UnityEngine.MonoBehaviour.print("OnCreatedRoom");
    }

    public void OnJoinedLobby()
    {
        UnityEngine.MonoBehaviour.print("OnJoinedLobby");
        NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiStart, state: false);
        NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiROOM, state: true);
    }

    public void OnLeftLobby()
    {
        UnityEngine.MonoBehaviour.print("OnLeftLobby");
    }

    public void OnDisconnectedFromPhoton()
    {
        UnityEngine.MonoBehaviour.print("OnDisconnectedFromPhoton");
        Screen.lockCursor = false;
        Screen.showCursor = true;
    }

    public void OnConnectionFail(DisconnectCause cause)
    {
        UnityEngine.MonoBehaviour.print("OnConnectionFail : " + cause);
        Screen.lockCursor = false;
        Screen.showCursor = true;
        IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
        gameStart = false;
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: false);
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[4], state: true);
        GameObject.Find("LabelDisconnectInfo").GetComponent<UILabel>().text = "OnConnectionFail : " + cause;
    }

    public void OnFailedToConnectToPhoton()
    {
        UnityEngine.MonoBehaviour.print("OnFailedToConnectToPhoton");
    }

    public void OnReceivedRoomListUpdate()
    {
    }

    public void OnJoinedRoom()
    {
        print("OnJoinedRoom " + PhotonNetwork.Room.PhotonName + "    >>>>   " + PhotonNetwork.Room.MapName);

        gameTimesUp = false;

        Level = PhotonNetwork.Room.MapName;

        IN_GAME_MAIN_CAMERA.difficulty = PhotonNetwork.Room.Difficulty;
        time = PhotonNetwork.Room.Time;
        time *= 60;

        IN_GAME_MAIN_CAMERA.DayLight = PhotonNetwork.Room.DayTime;
       
        IN_GAME_MAIN_CAMERA.gamemode = LevelInfo.getInfo(Level).type;
        PhotonNetwork.LoadLevel(LevelInfo.getInfo(Level).mapName);

        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
        {
            { PhotonPlayerProperty.name, LoginFengKAI.player.name },
            { PhotonPlayerProperty.guildName, LoginFengKAI.player.guildname },
            { PhotonPlayerProperty.kills, 0 },
            { PhotonPlayerProperty.max_dmg, 0 },
            { PhotonPlayerProperty.total_dmg, 0 },
            { PhotonPlayerProperty.deaths, 0 },
            { PhotonPlayerProperty.dead, true },
            { PhotonPlayerProperty.isTitan, 0 }
        };


        PhotonNetwork.Player.SetCustomProperties(hashtable);

        humanScore = 0;
        titanScore = 0;
        PVPtitanScore = 0;
        PVPhumanScore = 0;
        wave = 1;
        highestwave = 1;
        localRacingResult = string.Empty;
        needChooseSide = true;
        chatContent = new ArrayList();
        killInfoGO = new ArrayList();
        InRoomChat.messages = new List<string>();
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RequireStatus", PhotonTargets.MasterClient);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 0 || Application.loadedLevelName == "characterCreation" || Application.loadedLevelName == "SnapShot")
        {
            return;
        }
        ChangeQuality.setCurrentQuality();
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        foreach (GameObject gameObject in array)
        {
            if (gameObject.GetPhotonView() == null || !gameObject.GetPhotonView().owner.IsMasterClient)
            {
                Destroy(gameObject);
            }
        }

        isWinning = false;
        gameStart = true;
        ShowHUDInfoCenter(string.Empty);
        GameObject gameObject2 = (GameObject)Object.Instantiate(Resources.Load("MainCamera_mono"), GameObject.Find("cameraDefaultPosition").transform.position, GameObject.Find("cameraDefaultPosition").transform.rotation);
        Object.Destroy(GameObject.Find("cameraDefaultPosition"));
        gameObject2.name = "MainCamera";
        Screen.lockCursor = true;
        Screen.showCursor = true;
        ui = (GameObject)Object.Instantiate(Resources.Load("UI_IN_GAME"));
        ui.name = "UI_IN_GAME";
        ui.SetActive(value: true);
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: true);
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
        NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
        IN_GAME_MAIN_CAMERA.Camera.setHUDposition();
        IN_GAME_MAIN_CAMERA.Camera.setDayLight(IN_GAME_MAIN_CAMERA.DayLight);
        LevelInfo info = LevelInfo.getInfo(Level);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            single_kills = 0;
            single_maxDamage = 0;
            single_totalDamage = 0;
            IN_GAME_MAIN_CAMERA.Camera.enabled = true;
            IN_GAME_MAIN_CAMERA.Camera.GetComponent<SpectatorMovement>().disable = true;
            IN_GAME_MAIN_CAMERA.Camera.GetComponent<MouseLook>().disable = true;
            IN_GAME_MAIN_CAMERA.gamemode = LevelInfo.getInfo(Level).type;
            SpawnPlayer(IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper());
            if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
            {
                Screen.lockCursor = true;
            }
            else
            {
                Screen.lockCursor = false;
            }
            Screen.showCursor = false;
            int rate = 90;
            if (difficulty == 1)
            {
                rate = 70;
            }
            randomSpawnTitan("titanRespawn", rate, info.enemyNumber);
            return;
        }
        PVPcheckPoint.chkPts = new ArrayList();
        IN_GAME_MAIN_CAMERA.Camera.enabled = false;
        IN_GAME_MAIN_CAMERA.Camera.GetComponent<CameraShake>().enabled = false;
        IN_GAME_MAIN_CAMERA.GameType = GameType.Multiplayer;
        if (info.type == GAMEMODE.TROST)
        {
            GameObject.Find("playerRespawn").SetActive(value: false);
            Object.Destroy(GameObject.Find("playerRespawn"));
            GameObject gameObject3 = GameObject.Find("rock");
            gameObject3.animation["lift"].speed = 0f;
            GameObject.Find("door_fine").SetActive(false);
            GameObject.Find("door_broke").SetActive(true);
            Object.Destroy(GameObject.Find("ppl"));
        }
        else if (info.type == GAMEMODE.BOSS_FIGHT_CT)
        {
            GameObject.Find("playerRespawnTrost").SetActive(value: false);
            Object.Destroy(GameObject.Find("playerRespawnTrost"));
        }
        if (needChooseSide)
        {
            ShowHUDInfoTopCenterADD("\n\nPRESS 1 TO ENTER GAME");
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
            {
                Screen.lockCursor = true;
            }
            else
            {
                Screen.lockCursor = false;
            }
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
            {
                if ((int)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.isTitan] == 2)
                {
                    checkpoint = GameObject.Find("PVPchkPtT");
                }
                else
                {
                    checkpoint = GameObject.Find("PVPchkPtH");
                }
            }
            if ((int)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.isTitan] == 2)
            {
                SpawnNonAITitan(myLastHero);
            }
            else
            {
                SpawnPlayer(myLastHero, myLastRespawnTag);
            }
        }
        if (info.type == GAMEMODE.BOSS_FIGHT_CT)
        {
            Object.Destroy(GameObject.Find("rock"));
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (info.type == GAMEMODE.TROST)
            {
                if (!isPlayerAllDead())
                {
                    GameObject gameObject4 = PhotonNetwork.Instantiate("TITAN_EREN_trost", new Vector3(-200f, 0f, -194f), Quaternion.Euler(0f, 180f, 0f), 0);
                    gameObject4.GetComponent<TITAN_EREN>().rockLift = true;
                    int rate2 = 90;
                    if (difficulty == 1)
                    {
                        rate2 = 70;
                    }
                    GameObject[] array3 = GameObject.FindGameObjectsWithTag("titanRespawn");
                    GameObject gameObject5 = GameObject.Find("titanRespawnTrost");
                    if (gameObject5 != null)
                    {
                        GameObject[] array4 = array3;
                        foreach (GameObject gameObject6 in array4)
                        {
                            if (gameObject6.transform.parent.gameObject == gameObject5)
                            {
                                spawnTitan(rate2, gameObject6.transform.position, gameObject6.transform.rotation);
                            }
                        }
                    }
                }
            }
            else if (info.type == GAMEMODE.BOSS_FIGHT_CT)
            {
                if (!isPlayerAllDead())
                {
                    PhotonNetwork.Instantiate("COLOSSAL_TITAN", -Vector3.up * 10000f, Quaternion.Euler(0f, 180f, 0f), 0);
                }
            }
            else if (info.type == GAMEMODE.KILL_TITAN || info.type == GAMEMODE.ENDLESS_TITAN || info.type == GAMEMODE.SURVIVE_MODE)
            {
                if (info.name == "Annie" || info.name == "Annie II")
                {
                    PhotonNetwork.Instantiate("FEMALE_TITAN", GameObject.Find("titanRespawn").transform.position, GameObject.Find("titanRespawn").transform.rotation, 0);
                }
                else
                {
                    int rate3 = 90;
                    if (difficulty == 1)
                    {
                        rate3 = 70;
                    }
                    randomSpawnTitan("titanRespawn", rate3, info.enemyNumber);
                }
            }
            else if (info.type != GAMEMODE.TROST && info.type == GAMEMODE.PVP_CAPTURE && LevelInfo.getInfo(Level).mapName == "OutSide")
            {
                GameObject[] array5 = GameObject.FindGameObjectsWithTag("titanRespawn");
                if (array5.Length <= 0)
                {
                    return;
                }
                for (int k = 0; k < array5.Length; k++)
                {
                    GameObject gameObject7 = spawnTitanRaw(array5[k].transform.position, array5[k].transform.rotation);
                    gameObject7.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER, forceCrawler: true);
                }
            }
        }
        if (!info.supply)
        {
            Object.Destroy(GameObject.Find("aot_supply"));
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            base.photonView.RPC("RequireStatus", PhotonTargets.MasterClient);
        }
        if (LevelInfo.getInfo(Level).lavaMode)
        {
            Object.Instantiate(Resources.Load("levelBottom"), new Vector3(0f, -29.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
            GameObject.Find("aot_supply").transform.position = GameObject.Find("aot_supply_lava_position").transform.position;
            GameObject.Find("aot_supply").transform.rotation = GameObject.Find("aot_supply_lava_position").transform.rotation;
        }
    }

    public void OnPhotonPlayerConnected()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonPlayerConnected");
    }

    public void OnPhotonPlayerDisconnected()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonPlayerDisconnected");
        if (!gameTimesUp)
        {
            oneTitanDown(string.Empty, onPlayerLeave: true);
            someOneIsDead(0);
        }
    }

    public void OnPhotonRandomJoinFailed()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonRandomJoinFailed");
    }

    public void OnConnectedToMaster()
    {
        UnityEngine.MonoBehaviour.print("OnConnectedToMaster");
    }

    public void OnPhotonSerializeView()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonSerializeView");
    }

    public void OnPhotonInstantiate()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonInstantiate");
    }

    public void OnPhotonMaxCccuReached()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonMaxCccuReached");
    }

    public void OnPhotonCustomRoomPropertiesChanged()
    {
        UnityEngine.MonoBehaviour.print("OnPhotonCustomRoomPropertiesChanged");
    }

    public void OnPhotonPlayerPropertiesChanged()
    {
    }

    public void OnUpdatedFriendList()
    {
        UnityEngine.MonoBehaviour.print("OnUpdatedFriendList");
    }

    public void OnCustomAuthenticationFailed()
    {
        UnityEngine.MonoBehaviour.print("OnCustomAuthenticationFailed");
    }

    public void restartGame(bool masterclientSwitched = false)
    {
        UnityEngine.MonoBehaviour.print("reset game :" + gameTimesUp);
        if (!gameTimesUp)
        {
            PVPtitanScore = 0;
            PVPhumanScore = 0;
            startRacing = false;
            endRacing = false;
            checkpoint = null;
            timeElapse = 0f;
            roundTime = 0f;
            isWinning = false;
            isLosing = false;
            isPlayer1Winning = false;
            isPlayer2Winning = false;
            wave = 1;
            myRespawnTime = 0f;
            kicklist = new ArrayList();
            killInfoGO = new ArrayList();
            racingResult = new ArrayList();
            ShowHUDInfoCenter(string.Empty);
            PhotonNetwork.DestroyAll();
            base.photonView.RPC("RPCLoadLevel", PhotonTargets.All);
            if (masterclientSwitched)
            {
                sendChatContentInfo("<color=#A8FF24>MasterClient has switched to </color>" + PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.name]);
            }
        }
    }

    public void restartGameSingle()
    {
        startRacing = false;
        endRacing = false;
        checkpoint = null;
        single_kills = 0;
        single_maxDamage = 0;
        single_totalDamage = 0;
        timeElapse = 0f;
        roundTime = 0f;
        timeTotalServer = 0f;
        isWinning = false;
        isLosing = false;
        isPlayer1Winning = false;
        isPlayer2Winning = false;
        wave = 1;
        myRespawnTime = 0f;
        ShowHUDInfoCenter(string.Empty);
        Application.LoadLevel(Application.loadedLevel);
    }

    public void checkPVPpts()
    {
        if (PVPtitanScore >= PVPtitanScoreMax)
        {
            PVPtitanScore = PVPtitanScoreMax;
            gameLose();
        }
        else if (PVPhumanScore >= PVPhumanScoreMax)
        {
            PVPhumanScore = PVPhumanScoreMax;
            gameWin();
        }
    }

    public void gameLose()
    {
        if (!isWinning && !isLosing)
        {
            isLosing = true;
            titanScore++;
            gameEndCD = gameEndTotalCDtime;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                base.photonView.RPC("netGameLose", PhotonTargets.Others, titanScore);
            }
        }
    }

    public void gameWin()
    {
        if (isLosing || isWinning)
        {
            return;
        }
        isWinning = true;
        humanScore++;
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
        {
            gameEndCD = 20f;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                base.photonView.RPC("netGameWin", PhotonTargets.Others, 0);
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
        {
            gameEndCD = gameEndTotalCDtime;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                base.photonView.RPC("netGameWin", PhotonTargets.Others, teamWinner);
            }
            teamScores[teamWinner - 1]++;
        }
        else
        {
            gameEndCD = gameEndTotalCDtime;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                base.photonView.RPC("netGameWin", PhotonTargets.Others, humanScore);
            }
        }
    }

    public void multiplayerRacingFinsih()
    {
        float num = roundTime - 20f;
        if (PhotonNetwork.IsMasterClient)
        {
            getRacingResult(LoginFengKAI.player.name, num);
        }
        else
        {
            base.photonView.RPC("getRacingResult", PhotonTargets.MasterClient, LoginFengKAI.player.name, num);
        }
        gameWin();
    }

    private void refreshRacingResult()
    {
        localRacingResult = "Result\n";
        IComparer comparer = new IComparerRacingResult();
        racingResult.Sort(comparer);
        int count = racingResult.Count;
        count = Mathf.Min(count, 6);
        for (int i = 0; i < count; i++)
        {
            string text = localRacingResult;
            localRacingResult = text + "Rank " + (i + 1) + " : ";
            localRacingResult += (racingResult[i] as RacingResult).name;
            localRacingResult = localRacingResult + "   " + (float)(int)((racingResult[i] as RacingResult).time * 100f) * 0.01f + "s";
            localRacingResult += "\n";
        }
        base.photonView.RPC("netRefreshRacingResult", PhotonTargets.All, localRacingResult);
    }

    public void randomSpawnTitan(string place, int rate, int num, bool punk = false)
    {
        if (num == -1)
        {
            num = 1;
        }
        GameObject[] array = GameObject.FindGameObjectsWithTag(place);
        if (array.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < num; i++)
        {
            int num2 = Random.Range(0, array.Length);
            GameObject gameObject = array[num2];
            while (array[num2] == null)
            {
                num2 = Random.Range(0, array.Length);
                gameObject = array[num2];
            }
            array[num2] = null;
            spawnTitan(rate, gameObject.transform.position, gameObject.transform.rotation, punk);
        }
    }

    public GameObject randomSpawnOneTitan(string place, int rate)
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag(place);
        int num = Random.Range(0, array.Length);
        GameObject gameObject = array[num];
        while (array[num] == null)
        {
            num = Random.Range(0, array.Length);
            gameObject = array[num];
        }
        array[num] = null;
        return spawnTitan(rate, gameObject.transform.position, gameObject.transform.rotation);
    }

    private GameObject spawnTitanRaw(Vector3 position, Quaternion rotation)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return (GameObject)Object.Instantiate(Resources.Load("TITAN_VER3.1"), position, rotation);
        }
        return PhotonNetwork.Instantiate("TITAN_VER3.1", position, rotation, 0);
    }

    public GameObject spawnTitan(int rate, Vector3 position, Quaternion rotation, bool punk = false)
    {
        GameObject gameObject = spawnTitanRaw(position, rotation);
        if (punk)
        {
            gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_PUNK);
        }
        else if (Random.Range(0, 100) < rate)
        {
            if (IN_GAME_MAIN_CAMERA.difficulty == 2)
            {
                if (Random.Range(0f, 1f) < 0.7f || LevelInfo.getInfo(Level).noCrawler)
                {
                    gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                }
                else
                {
                    gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                }
            }
        }
        else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
        {
            if (Random.Range(0f, 1f) < 0.7f || LevelInfo.getInfo(Level).noCrawler)
            {
                gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
            }
            else
            {
                gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
            }
        }
        else if (Random.Range(0, 100) < rate)
        {
            if (Random.Range(0f, 1f) < 0.8f || LevelInfo.getInfo(Level).noCrawler)
            {
                gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_I);
            }
            else
            {
                gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
            }
        }
        else if (Random.Range(0f, 1f) < 0.8f || LevelInfo.getInfo(Level).noCrawler)
        {
            gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
        }
        else
        {
            gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
        }
        GameObject gameObject2 = ((IN_GAME_MAIN_CAMERA.GameType != 0) ? PhotonNetwork.Instantiate("FX/FXtitanSpawn", gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0) : ((GameObject)Object.Instantiate(Resources.Load("FX/FXtitanSpawn"), gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f))));
        gameObject2.transform.localScale = gameObject.transform.localScale;
        return gameObject;
    }

    public void titanGetKillbyServer(int Damage, string name)
    {
        Damage = Mathf.Max(10, Damage);
        sendKillInfo(t1: false, LoginFengKAI.player.name, t2: true, name, Damage);
        netShowDamage(Damage);
        oneTitanDown(name);
        playerKillInfoUpdate(PhotonNetwork.Player, Damage);
    }

    public void playerKillInfoUpdate(PhotonPlayer player, int dmg)
    {
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {
        {
            PhotonPlayerProperty.kills,
            player.Kills + 1
        } });
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {
        {
            PhotonPlayerProperty.max_dmg,
            Mathf.Max(dmg, player.HighestDmg)
        } });
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {
        {
            PhotonPlayerProperty.total_dmg,
            player.TotalDmg + dmg
        } });
    }

    public void playerKillInfoSingleUpdate(int dmg)
    {
        single_kills++;
        single_maxDamage = Mathf.Max(dmg, single_maxDamage);
        single_totalDamage += dmg;
    }

    private bool checkIsTitanAllDie()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        GameObject[] array2 = array;
        foreach (GameObject gameObject in array2)
        {
            if ((bool)gameObject.GetComponent<TITAN>() && !gameObject.GetComponent<TITAN>().hasDie)
            {
                return false;
            }
            if ((bool)gameObject.GetComponent<FEMALE_TITAN>())
            {
                return false;
            }
        }
        return true;
    }

    public void sendKillInfo(bool t1, string killer, bool t2, string victim, int dmg = 0)
    {
        base.photonView.RPC("updateKillInfo", PhotonTargets.All, t1, killer, t2, victim, dmg);
    }

    public void sendChatContentInfo(string content)
    {
        base.photonView.RPC("Chat", PhotonTargets.All, content, string.Empty);
    }

    public void KickPlayer(string kickPlayer, string kicker)
    {
        bool flag = false;
        for (int i = 0; i < kicklist.Count; i++)
        {
            if (((KickState)kicklist[i]).name == kickPlayer)
            {
                KickState kickState = (KickState)kicklist[i];
                kickState.addKicker(kicker);
                tryKick(kickState);
                flag = true;
                break;
            }
        }
        if (!flag)
        {
            KickState kickState = new KickState();
            kickState.init(kickPlayer);
            kickState.addKicker(kicker);
            kicklist.Add(kickState);
            tryKick(kickState);
        }
    }

    private void tryKick(KickState tmp)
    {
        sendChatContentInfo("kicking #" + tmp.name + ", " + tmp.getKickCount() + "/" + (int)((float)PhotonNetwork.PlayerList.Length * 0.5f) + "vote");
        if (tmp.getKickCount() >= (int)((float)PhotonNetwork.PlayerList.Length * 0.5f))
        {
            kickPhotonPlayer(tmp.name.ToString());
        }
    }

    private void kickPhotonPlayer(string name)
    {
        UnityEngine.MonoBehaviour.print("KICK " + name + "!!!");
        PhotonPlayer[] playerList = PhotonNetwork.PlayerList;
        foreach (PhotonPlayer photonPlayer in playerList)
        {
            if (photonPlayer.ID.ToString() == name && !photonPlayer.IsMasterClient)
            {
                PhotonNetwork.CloseConnection(photonPlayer);
                break;
            }
        }
    }

    private void ShowHUDInfoTopCenter(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoTopCenter");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopCenterADD(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoTopCenter");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text += content;
        }
    }

    private void ShowHUDInfoTopLeft(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoTopLeft");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopRight(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoTopRight");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopRightMAPNAME(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoTopRight");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text += content;
        }
    }

    public void ShowHUDInfoCenter(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoCenter");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text = content;
        }
    }

    public void ShowHUDInfoCenterADD(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoCenter");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text += content;
        }
    }

    public bool isPlayerAllDead()
    {
        int num = 0;
        int num2 = 0;
        PhotonPlayer[] playerList = PhotonNetwork.PlayerList;
        foreach (PhotonPlayer photonPlayer in playerList)
        {
            if (!photonPlayer.IsTitan)
            {
                num++;
                if (photonPlayer.IsDead)
                {
                    num2++;
                }
            }
        }
        if (num == num2)
        {
            return true;
        }
        return false;
    }

    public bool isTeamAllDead(int team)
    {
        int num = 0;
        int num2 = 0;
        PhotonPlayer[] playerList = PhotonNetwork.PlayerList;
        foreach (PhotonPlayer photonPlayer in playerList)
        {
            if (!photonPlayer.IsTitan && photonPlayer.Team == team)
            {
                num++;
                if ((bool)photonPlayer.CustomProperties[PhotonPlayerProperty.dead])
                {
                    num2++;
                }
            }
        }
        if (num == num2)
        {
            return true;
        }
        return false;
    }

    private void SingleShowHUDInfoTopLeft(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoTopLeft");
        if ((bool)gameObject)
        {
            content = content.Replace("[0]", "[*^_^*]");
            gameObject.GetComponent<UILabel>().text = content;
        }
    }

    private void SingleShowHUDInfoTopCenter(string content)
    {
        GameObject gameObject = GameObject.Find("LabelInfoTopCenter");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text = content;
        }
    }
}