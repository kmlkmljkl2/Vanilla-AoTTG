using UnityEngine;

public class BTN_PAUSE_MENU_CONTINUE : MonoBehaviour
{
	private void OnClick()
	{
		if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
		{
			Time.timeScale = 1f;
		}
		GameObject gameObject = GameObject.Find("UI_IN_GAME");
		NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[0], state: true);
		NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[1], state: false);
		NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[2], state: false);
		NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[3], state: false);
		if (!GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled)
		{
			Screen.showCursor = true;
			Screen.lockCursor = true;
			FengGameManagerMKII.InputManager.menuOn = false;
			GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = false;
			GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = false;
			return;
		}
		IN_GAME_MAIN_CAMERA.isPausing = false;
		if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
		{
			Screen.showCursor = false;
			Screen.lockCursor = true;
		}
		else
		{
			Screen.showCursor = false;
			Screen.lockCursor = false;
		}
		FengGameManagerMKII.InputManager.menuOn = false;
		FengGameManagerMKII.InputManager.justUPDATEME();
	}
}
