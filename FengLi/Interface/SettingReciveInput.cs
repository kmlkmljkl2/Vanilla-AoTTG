using UnityEngine;

public class SettingReciveInput : MonoBehaviour
{
	public int id;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		FengGameManagerMKII.InputManager.startListening(id);
		base.transform.Find("Label").gameObject.GetComponent<UILabel>().text = "*wait for input";
	}
}
