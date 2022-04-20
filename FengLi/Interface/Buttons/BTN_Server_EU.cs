using UnityEngine;

public class BTN_Server_EU : MonoBehaviour
{
	private void OnClick()
	{
		PhotonNetwork.Disconnect();
		PhotonNetwork.ConnectToMaster("135.125.239.180", 5055, "", UIMainReferences.version);
	}
}
