using ExitGames.Client.Photon;
using UnityEngine;

public class BTN_choose_titan : MonoBehaviour
{
	private void Start()
	{
		if (!LevelInfo.getInfo(FengGameManagerMKII.Level).teamTitan)
		{
			base.gameObject.GetComponent<UIButton>().isEnabled = false;
		}
	}

	private void OnClick()
	{
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
		{
			string text = "AHSS";
			NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], state: true);
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().needChooseSide = false;
			if (!PhotonNetwork.IsMasterClient && GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().roundTime > 60f)
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().NOTSpawnPlayer(text);
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("restartGameByClient", PhotonTargets.MasterClient);
			}
			else
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().SpawnPlayer(text, "playerRespawn2");
			}
			NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], state: false);
			NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], state: false);
			NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], state: false);
			IN_GAME_MAIN_CAMERA.usingTitan = false;
			GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setHUDposition();
			Hashtable hashtable = new Hashtable();
			hashtable.Add(PhotonPlayerProperty.character, text);
			Hashtable customProperties = hashtable;
			PhotonNetwork.Player.SetCustomProperties(customProperties);
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint = GameObject.Find("PVPchkPtT");
			}
			string selection = GameObject.Find("PopupListCharacterTITAN").GetComponent<UIPopupList>().selection;
			NGUITools.SetActive(base.transform.parent.gameObject, state: false);
			NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0], state: true);
			if ((!PhotonNetwork.IsMasterClient && GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().roundTime > 60f) || GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().justSuicide)
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().justSuicide = false;
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().NOTSpawnNonAITitan(selection);
			}
			else
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().SpawnNonAITitan(selection);
			}
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().needChooseSide = false;
			NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], state: false);
			NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], state: false);
			NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], state: false);
			IN_GAME_MAIN_CAMERA.usingTitan = true;
			GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setHUDposition();
		}
	}
}
