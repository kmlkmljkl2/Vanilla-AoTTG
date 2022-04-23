using ExitGames.Client.Photon;
using UnityEngine;

public class BTN_choose_human : MonoBehaviour
{
	private void OnClick()
	{
		string selection = GameObject.Find("PopupListCharacterHUMAN").GetComponent<UIPopupList>().selection;
		NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], state: true);
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().needChooseSide = false;
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint = GameObject.Find("PVPchkPtH");
		}
		if (!PhotonNetwork.IsMasterClient && GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().roundTime > 60f)
		{
            if (!isPlayerAllDead())
            {
				// GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().NOTSpawnPlayer(selection);
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().SpawnPlayer(selection);
			}
			else
            {
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().NOTSpawnPlayer(selection);
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("restartGameByClient", PhotonTargets.MasterClient);
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.TROST || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
		{
			if (isPlayerAllDead())
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().NOTSpawnPlayer(selection);
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("restartGameByClient", PhotonTargets.MasterClient);
			}
			else
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().SpawnPlayer(selection);
			}
		}
		else
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().SpawnPlayer(selection);
		}
		NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], state: false);
		NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], state: false);
		NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], state: false);
		IN_GAME_MAIN_CAMERA.usingTitan = false;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setHUDposition();
		Hashtable hashtable = new Hashtable();
		hashtable.Add(PhotonPlayerProperty.character, selection);
		Hashtable customProperties = hashtable;
		PhotonNetwork.Player.SetCustomProperties(customProperties);
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
}
