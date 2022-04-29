using UnityEngine;

public partial class FengGameManagerMKII
{
    [RPC]
    private void RequireStatus()
    {
        base.photonView.RPC("refreshStatus", PhotonTargets.Others, humanScore, titanScore, wave, highestwave, roundTime, timeTotalServer, startRacing, endRacing);
        base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
        base.photonView.RPC("refreshPVPStatus_AHSS", PhotonTargets.Others, teamScores);
    }

    [RPC]
    private void refreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2, bool startRacin, bool endRacin)
    {
        humanScore = score1;
        titanScore = score2;
        wave = wav;
        highestwave = highestWav;
        roundTime = time1;
        timeTotalServer = time2;
        startRacing = startRacin;
        endRacing = endRacin;
        if (startRacing && (bool)GameObject.Find("door"))
        {
            GameObject.Find("door").SetActive(value: false);
        }
    }

    [RPC]
    private void refreshPVPStatus(int score1, int score2)
    {
        PVPhumanScore = score1;
        PVPtitanScore = score2;
    }

    [RPC]
    private void refreshPVPStatus_AHSS(int[] score1)
    {
        UnityEngine.MonoBehaviour.print(score1);
        teamScores = score1;
    }

    [RPC]
    private void restartGameByClient()
    {
        restartGame();
    }

    [RPC]
    private void RPCLoadLevel()
    {
        PhotonNetwork.LoadLevel(LevelInfo.getInfo(Level).mapName);
    }

    [RPC]
    public void someOneIsDead(int id = -1)
    {
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
        {
            if (id != 0)
            {
                PVPtitanScore += 2;
            }
            checkPVPpts();
            base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
        {
            titanScore++;
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.TROST)
        {
            if (isPlayerAllDead())
            {
                gameLose();
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
        {
            if (isPlayerAllDead())
            {
                gameLose();
                teamWinner = 0;
            }
            if (isTeamAllDead(1))
            {
                teamWinner = 2;
                gameWin();
            }
            if (isTeamAllDead(2))
            {
                teamWinner = 1;
                gameWin();
            }
        }
    }

    [RPC]
    private void netGameLose(int score)
    {
        isLosing = true;
        titanScore = score;
        gameEndCD = gameEndTotalCDtime;
    }

    [RPC]
    private void netGameWin(int score)
    {
        humanScore = score;
        isWinning = true;
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
        {
            teamWinner = score;
            teamScores[teamWinner - 1]++;
            gameEndCD = gameEndTotalCDtime;
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
        {
            gameEndCD = 20f;
        }
        else
        {
            gameEndCD = gameEndTotalCDtime;
        }
    }

    [RPC]
    private void netRefreshRacingResult(string tmp)
    {
        localRacingResult = tmp;
    }

    [RPC]
    public void titanGetKill(PhotonPlayer player, int Damage, string name)
    {
        Damage = Mathf.Max(10, Damage);
        base.photonView.RPC("netShowDamage", player, Damage);
        base.photonView.RPC("oneTitanDown", PhotonTargets.MasterClient, name, false);
        sendKillInfo(t1: false, (string)player.CustomProperties[PhotonPlayerProperty.name], t2: true, name, Damage);
        playerKillInfoUpdate(player, Damage);
    }

    [RPC]
    public void oneTitanDown(string name1 = "", bool onPlayerLeave = false)
    {
        if (IN_GAME_MAIN_CAMERA.GameType != 0 && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
        {
            if (!(name1 == string.Empty))
            {
                switch (name1)
                {
                    case "Titan":
                        PVPhumanScore++;
                        break;

                    case "Aberrant":
                        PVPhumanScore += 2;
                        break;

                    case "Jumper":
                        PVPhumanScore += 3;
                        break;

                    case "Crawler":
                        PVPhumanScore += 4;
                        break;

                    case "Female Titan":
                        PVPhumanScore += 10;
                        break;

                    default:
                        PVPhumanScore += 3;
                        break;
                }
            }
            checkPVPpts();
            base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
            {
                return;
            }
            if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
            {
                if (checkIsTitanAllDie())
                {
                    gameWin();
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                }
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
            {
                if (!checkIsTitanAllDie())
                {
                    return;
                }
                wave++;
                if (LevelInfo.getInfo(Level).respawnMode == RespawnMode.NEWROUND && IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                {
                    base.photonView.RPC("respawnHeroInNewRound", PhotonTargets.All);
                }
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                {
                    sendChatContentInfo("<color=#A8FF24>Wave : " + wave + "</color>");
                }
                if (wave > highestwave)
                {
                    highestwave = wave;
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    RequireStatus();
                }
                if (wave > 20)
                {
                    gameWin();
                    return;
                }
                int rate = 90;
                if (difficulty == 1)
                {
                    rate = 70;
                }
                if (!LevelInfo.getInfo(Level).punk)
                {
                    randomSpawnTitan("titanRespawn", rate, wave + 2);
                }
                else if (wave == 5)
                {
                    randomSpawnTitan("titanRespawn", rate, 1, punk: true);
                }
                else if (wave == 10)
                {
                    randomSpawnTitan("titanRespawn", rate, 2, punk: true);
                }
                else if (wave == 15)
                {
                    randomSpawnTitan("titanRespawn", rate, 3, punk: true);
                }
                else if (wave == 20)
                {
                    randomSpawnTitan("titanRespawn", rate, 4, punk: true);
                }
                else
                {
                    randomSpawnTitan("titanRespawn", rate, wave + 2);
                }
            }
            else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
            {
                if (!onPlayerLeave)
                {
                    humanScore++;
                    int rate2 = 90;
                    if (difficulty == 1)
                    {
                        rate2 = 70;
                    }
                    randomSpawnTitan("titanRespawn", rate2, 1);
                }
            }
            else if (LevelInfo.getInfo(Level).enemyNumber != -1)
            {
            }
        }
    }

    [RPC]
    private void respawnHeroInNewRound()
    {
        if (!needChooseSide && GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            SpawnPlayer(myLastHero, myLastRespawnTag);
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
            ShowHUDInfoCenter(string.Empty);
        }
    }

    [RPC]
    public void netShowDamage(int speed)
    {
        GameObject.Find("Stylish").GetComponent<StylishComponent>().Style(speed);
        GameObject gameObject = GameObject.Find("LabelScore");
        if ((bool)gameObject)
        {
            gameObject.GetComponent<UILabel>().text = speed.ToString();
            gameObject.transform.localScale = Vector3.zero;
            speed = (int)((float)speed * 0.1f);
            speed = Mathf.Max(40, speed);
            speed = Mathf.Min(150, speed);
            iTween.Stop(gameObject);
            iTween.ScaleTo(gameObject, iTween.Hash("x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f));
            iTween.ScaleTo(gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f));
        }
    }

    [RPC]
    private void showResult(string text0, string text1, string text2, string text3, string text4, string text6, PhotonMessageInfo t)
    {
        if (!gameTimesUp)
        {
            gameTimesUp = true;
            GameObject gameObject = GameObject.Find("UI_IN_GAME");
            NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[0], state: false);
            NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[1], state: false);
            NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[2], state: true);
            NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[3], state: false);
            GameObject.Find("LabelName").GetComponent<UILabel>().text = text0;
            GameObject.Find("LabelKill").GetComponent<UILabel>().text = text1;
            GameObject.Find("LabelDead").GetComponent<UILabel>().text = text2;
            GameObject.Find("LabelMaxDmg").GetComponent<UILabel>().text = text3;
            GameObject.Find("LabelTotalDmg").GetComponent<UILabel>().text = text4;
            GameObject.Find("LabelResultTitle").GetComponent<UILabel>().text = text6;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
            gameStart = false;
        }
    }

    [RPC]
    private void updateKillInfo(bool t1, string killer, bool t2, string victim, int dmg)
    {
        GameObject gameObject = GameObject.Find("UI_IN_GAME");
        GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/KillInfo"));
        for (int i = 0; i < killInfoGO.Count; i++)
        {
            GameObject gameObject3 = (GameObject)killInfoGO[i];
            if (gameObject3 != null)
            {
                gameObject3.GetComponent<KillInfoComponent>().moveOn();
            }
        }
        if (killInfoGO.Count > 4)
        {
            GameObject gameObject3 = (GameObject)killInfoGO[0];
            if (gameObject3 != null)
            {
                gameObject3.GetComponent<KillInfoComponent>().destory();
            }
            killInfoGO.RemoveAt(0);
        }
        gameObject2.transform.parent = gameObject.GetComponent<UIReferArray>().panels[0].transform;
        gameObject2.GetComponent<KillInfoComponent>().show(t1, killer, t2, victim, dmg);
        killInfoGO.Add(gameObject2);
    }

    [RPC]
    private void Chat(string content, string sender, PhotonMessageInfo info)
    {
        if(info != null && !info.Sender.IsLocal && info.Sender.Muted)
        {
            return;
        }
        //if (content.Length > 7 && content.Substring(0, 7) == "/kick #")
        //{
        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        KickPlayer(content.Remove(0, 7), sender);
        //    }
        //    return;
        //}
        if (sender != string.Empty)
        {
            content = sender + ":" + content;
        }
        GameObject.Find("Chatroom").GetComponent<InRoomChat>().AddLine(content);
    }

    [RPC]
    private void showChatContent(string content)
    {
        chatContent.Add(content);
        if (chatContent.Count > 10)
        {
            chatContent.RemoveAt(0);
        }
        GameObject.Find("LabelChatContent").GetComponent<UILabel>().text = string.Empty;
        for (int i = 0; i < chatContent.Count; i++)
        {
            GameObject.Find("LabelChatContent").GetComponent<UILabel>().text += chatContent[i];
        }
    }

    [RPC]
    private void getRacingResult(string player, float time)
    {
        RacingResult racingResult = new RacingResult();
        racingResult.name = player;
        racingResult.time = time;
        this.racingResult.Add(racingResult);
        refreshRacingResult();
    }
}