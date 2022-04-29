using System.Collections;
using Photon;
using UnityEngine;

public class PVPcheckPoint : Photon.MonoBehaviour
{
	public int id;

	public GameObject humanCyc;

	public GameObject titanCyc;

	public CheckPointState state;

	private bool titanOn;

	private bool playerOn;

	public float humanPt;

	public float titanPt;

	public float humanPtMax = 40f;

	public float titanPtMax = 40f;

	public int normalTitanRate = 70;

	private bool annie;

	private float spawnTitanTimer;

	public float titanInterval = 30f;

	public float size = 1f;

	private float hitTestR = 15f;

	private GameObject supply;

	private float syncTimer;

	private float syncInterval = 0.6f;

	private float getPtsTimer;

	private float getPtsInterval = 20f;

	public GameObject[] chkPtNextArr;

	public GameObject[] chkPtPreviousArr;

	public static ArrayList chkPts;

	public bool hasAnnie;

	public bool isBase;

	public GameObject chkPtNext
	{
		get
		{
			if (chkPtNextArr.Length <= 0)
			{
				return null;
			}
			return chkPtNextArr[Random.Range(0, chkPtNextArr.Length)];
		}
	}

	public GameObject chkPtPrevious
	{
		get
		{
			if (chkPtPreviousArr.Length <= 0)
			{
				return null;
			}
			return chkPtPreviousArr[Random.Range(0, chkPtPreviousArr.Length)];
		}
	}

	private void Start()
	{
		if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.PVP_CAPTURE)
		{
			if (base.photonView.IsMine)
			{
				Object.Destroy(base.gameObject);
			}
			Object.Destroy(base.gameObject);
			return;
		}
		chkPts.Add(this);
		IComparer comparer = new IComparerPVPchkPtID();
		chkPts.Sort(comparer);
		if (humanPt == humanPtMax)
		{
			state = CheckPointState.Human;
			if (base.photonView.IsMine && LevelInfo.getInfo(FengGameManagerMKII.Level).mapName != "The City I")
			{
				supply = PhotonNetwork.Instantiate("aot_supply", base.transform.position - Vector3.up * (base.transform.position.y - getHeight(base.transform.position)), base.transform.rotation, 0);
			}
		}
		else if (base.photonView.IsMine && !hasAnnie)
		{
			if (Random.Range(0, 100) < 50)
			{
				int num = Random.Range(1, 2);
				for (int i = 0; i < num; i++)
				{
					newTitan();
				}
			}
			if (isBase)
			{
				newTitan();
			}
		}
		if (titanPt == titanPtMax)
		{
			state = CheckPointState.Titan;
		}
		hitTestR = 15f * size;
		base.transform.localScale = new Vector3(size, size, size);
	}

	private void newTitan()
	{
		GameObject gameObject = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().spawnTitan(normalTitanRate, base.transform.position - Vector3.up * (base.transform.position.y - getHeight(base.transform.position)), base.transform.rotation);
		if (LevelInfo.getInfo(FengGameManagerMKII.Level).mapName == "The City I")
		{
			gameObject.GetComponent<TITAN>().chaseDistance = 120f;
		}
		else
		{
			gameObject.GetComponent<TITAN>().chaseDistance = 200f;
		}
		gameObject.GetComponent<TITAN>().PVPfromCheckPt = this;
	}

	private float getHeight(Vector3 pt)
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = layerMask;
		if (Physics.Raycast(pt, -Vector3.up, out var hitInfo, 1000f, layerMask2.value))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	private void Update()
	{
		float num = humanPt / humanPtMax;
		float num2 = titanPt / titanPtMax;
		if (!base.photonView.IsMine)
		{
			num = humanPt / humanPtMax;
			num2 = titanPt / titanPtMax;
			humanCyc.transform.localScale = new Vector3(num, num, 1f);
			titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
			syncTimer += Time.deltaTime;
			if (syncTimer > syncInterval)
			{
				syncTimer = 0f;
				checkIfBeingCapture();
			}
			return;
		}
		if (state == CheckPointState.Non)
		{
			if (playerOn && !titanOn)
			{
				humanGetsPoint();
				titanLosePoint();
			}
			else if (titanOn && !playerOn)
			{
				titanGetsPoint();
				humanLosePoint();
			}
			else
			{
				humanLosePoint();
				titanLosePoint();
			}
		}
		else if (state == CheckPointState.Human)
		{
			if (titanOn && !playerOn)
			{
				titanGetsPoint();
			}
			else
			{
				titanLosePoint();
			}
			getPtsTimer += Time.deltaTime;
			if (getPtsTimer > getPtsInterval)
			{
				getPtsTimer = 0f;
				if (!isBase)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().PVPhumanScore++;
				}
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkPVPpts();
			}
		}
		else if (state == CheckPointState.Titan)
		{
			if (playerOn && !titanOn)
			{
				humanGetsPoint();
			}
			else
			{
				humanLosePoint();
			}
			getPtsTimer += Time.deltaTime;
			if (getPtsTimer > getPtsInterval)
			{
				getPtsTimer = 0f;
				if (!isBase)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().PVPtitanScore++;
				}
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkPVPpts();
			}
			spawnTitanTimer += Time.deltaTime;
			if (spawnTitanTimer > titanInterval)
			{
				spawnTitanTimer = 0f;
				if (LevelInfo.getInfo(FengGameManagerMKII.Level).mapName == "The City I")
				{
					if (GameObject.FindGameObjectsWithTag("titan").Length < 12)
					{
						newTitan();
					}
				}
				else if (GameObject.FindGameObjectsWithTag("titan").Length < 20)
				{
					newTitan();
				}
			}
		}
		syncTimer += Time.deltaTime;
		if (syncTimer > syncInterval)
		{
			syncTimer = 0f;
			checkIfBeingCapture();
			syncPts();
		}
		num = humanPt / humanPtMax;
		num2 = titanPt / titanPtMax;
		humanCyc.transform.localScale = new Vector3(num, num, 1f);
		titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
	}

	private void checkIfBeingCapture()
	{
		playerOn = false;
		titanOn = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("titan");
		for (int i = 0; i < array.Length; i++)
		{
			if (!(Vector3.Distance(array[i].transform.position, base.transform.position) < hitTestR))
			{
				continue;
			}
			playerOn = true;
			if (state == CheckPointState.Human && array[i].GetPhotonView().IsMine)
			{
				if (GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint != base.gameObject)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint = base.gameObject;
					GameObject.Find("Chatroom").GetComponent<InRoomChat>().AddLine("<color=#A8FF24>Respawn point changed to point" + id + "</color>");
				}
				break;
			}
		}
		for (int i = 0; i < array2.Length; i++)
		{
			if (!(Vector3.Distance(array2[i].transform.position, base.transform.position) < hitTestR + 5f) || ((bool)array2[i].GetComponent<TITAN>() && array2[i].GetComponent<TITAN>().hasDie))
			{
				continue;
			}
			titanOn = true;
			if (state == CheckPointState.Titan && array2[i].GetPhotonView().IsMine && (bool)array2[i].GetComponent<TITAN>() && array2[i].GetComponent<TITAN>().nonAI)
			{
				if (GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint != base.gameObject)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint = base.gameObject;
					GameObject.Find("Chatroom").GetComponent<InRoomChat>().AddLine("<color=#A8FF24>Respawn point changed to point" + id + "</color>");
				}
				break;
			}
		}
	}

	private void humanGetsPoint()
	{
		if (humanPt >= humanPtMax)
		{
			humanPt = humanPtMax;
			titanPt = 0f;
			syncPts();
			state = CheckPointState.Human;
			base.photonView.RPC("changeState", PhotonTargets.All, 1);
			if (LevelInfo.getInfo(FengGameManagerMKII.Level).mapName != "The City I")
			{
				supply = PhotonNetwork.Instantiate("aot_supply", base.transform.position - Vector3.up * (base.transform.position.y - getHeight(base.transform.position)), base.transform.rotation, 0);
			}
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().PVPhumanScore += 2;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkPVPpts();
			if (checkIfHumanWins())
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameWin();
			}
		}
		else
		{
			humanPt += Time.deltaTime;
		}
	}

	private void titanGetsPoint()
	{
		if (titanPt >= titanPtMax)
		{
			titanPt = titanPtMax;
			humanPt = 0f;
			syncPts();
			if (state == CheckPointState.Human && supply != null)
			{
				PhotonNetwork.Destroy(supply);
			}
			state = CheckPointState.Titan;
			base.photonView.RPC("changeState", PhotonTargets.All, 2);
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().PVPtitanScore += 2;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkPVPpts();
			if (checkIfTitanWins())
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameLose();
			}
			if (hasAnnie)
			{
				if (!annie)
				{
					annie = true;
					PhotonNetwork.Instantiate("FEMALE_TITAN", base.transform.position - Vector3.up * (base.transform.position.y - getHeight(base.transform.position)), base.transform.rotation, 0);
				}
				else
				{
					newTitan();
				}
			}
			else
			{
				newTitan();
			}
		}
		else
		{
			titanPt += Time.deltaTime;
		}
	}

	private void titanLosePoint()
	{
		if (!(titanPt > 0f))
		{
			return;
		}
		titanPt -= Time.deltaTime * 3f;
		if (titanPt <= 0f)
		{
			titanPt = 0f;
			syncPts();
			if (state != CheckPointState.Human)
			{
				state = CheckPointState.Non;
				base.photonView.RPC("changeState", PhotonTargets.All, 0);
			}
		}
	}

	private void humanLosePoint()
	{
		if (!(humanPt > 0f))
		{
			return;
		}
		humanPt -= Time.deltaTime * 3f;
		if (humanPt <= 0f)
		{
			humanPt = 0f;
			syncPts();
			if (state != CheckPointState.Titan)
			{
				state = CheckPointState.Non;
				base.photonView.RPC("changeState", PhotonTargets.Others, 0);
			}
		}
	}

	private void syncPts()
	{
		base.photonView.RPC("changeTitanPt", PhotonTargets.Others, titanPt);
		base.photonView.RPC("changeHumanPt", PhotonTargets.Others, humanPt);
	}

	public string getStateString()
	{
		if (state == CheckPointState.Human)
		{
			return "[" + ColorSet.color_human + "]H[-]";
		}
		if (state == CheckPointState.Titan)
		{
			return "[" + ColorSet.color_titan_player + "]T[-]";
		}
		return "[" + ColorSet.color_D + "]_[-]";
	}

	private bool checkIfHumanWins()
	{
		for (int i = 0; i < chkPts.Count; i++)
		{
			if ((chkPts[i] as PVPcheckPoint).state != CheckPointState.Human)
			{
				return false;
			}
		}
		return true;
	}

	private bool checkIfTitanWins()
	{
		for (int i = 0; i < chkPts.Count; i++)
		{
			if ((chkPts[i] as PVPcheckPoint).state != CheckPointState.Titan)
			{
				return false;
			}
		}
		return true;
	}

	[RPC]
	private void changeHumanPt(float pt)
	{
		humanPt = pt;
	}

	[RPC]
	private void changeTitanPt(float pt)
	{
		titanPt = pt;
	}

	[RPC]
	private void changeState(int num)
	{
		if (num == 0)
		{
			state = CheckPointState.Non;
		}
		if (num == 1)
		{
			state = CheckPointState.Human;
		}
		if (num == 2)
		{
			state = CheckPointState.Titan;
		}
	}
}
