using UnityEngine;

public class BTN_PAUSE_MENU_QUIT : MonoBehaviour
{
	private void OnClick()
	{
		if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
		{
			Time.timeScale = 1f;
		}
		else
		{
			PhotonNetwork.Disconnect();
		}
		Screen.lockCursor = false;
		Screen.showCursor = true;
		IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameStart = false;
		FengGameManagerMKII.InputManager.menuOn = false;
		Object.Destroy(GameObject.Find("MultiplayerManager"));
		Application.LoadLevel("menu");
	}
}
