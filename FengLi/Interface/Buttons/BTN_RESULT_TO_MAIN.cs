using UnityEngine;

public class BTN_RESULT_TO_MAIN : MonoBehaviour
{
	private void OnClick()
	{
		Time.timeScale = 1f;
		if (PhotonNetwork.Connected)
		{
			PhotonNetwork.Disconnect();
		}
		IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameStart = false;
		Screen.lockCursor = false;
		Screen.showCursor = true;
		FengGameManagerMKII.InputManager.menuOn = false;
		Object.Destroy(GameObject.Find("MultiplayerManager"));
		Application.LoadLevel("menu");
	}
}
