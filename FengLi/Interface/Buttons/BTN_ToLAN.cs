using UnityEngine;

public class BTN_ToLAN : MonoBehaviour
{
	//this is the fcking MULTIPLAYER BUTTON
	private void OnClick()
	{
	//	return;
		NGUITools.SetActive(base.transform.parent.gameObject, state: false);
		NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiStart, state: true);
	}
}
