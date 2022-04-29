using UnityEngine;

public class LevelTriggerGas : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
			{
				other.gameObject.GetComponent<HERO>().fillGas();
				Object.Destroy(base.gameObject);
			}
			else if (other.gameObject.GetComponent<HERO>().photonView.IsMine)
			{
				other.gameObject.GetComponent<HERO>().fillGas();
				Object.Destroy(base.gameObject);
			}
		}
	}
}
