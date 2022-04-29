using UnityEngine;

public class BTN_BackToMain : MonoBehaviour
{
	private void OnClick()
	{
		NGUITools.SetActive(base.transform.parent.gameObject, state: false);
		NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMain, state: true);
		FengGameManagerMKII.InputManager.menuOn = false;
		PhotonNetwork.Disconnect();
	}
}
