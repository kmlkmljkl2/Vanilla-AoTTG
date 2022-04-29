using UnityEngine;

public class BTN_SetDefault : MonoBehaviour
{
	private void OnClick()
	{
		FengGameManagerMKII.InputManager.setToDefault();
		FengGameManagerMKII.InputManager.showKeyMap();
	}
}
