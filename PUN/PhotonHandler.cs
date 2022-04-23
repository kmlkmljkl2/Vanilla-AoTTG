using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

internal class PhotonHandler : Photon.MonoBehaviour, IPhotonPeerListener
{
	private const string PlayerPrefsKey = "PUNCloudBestRegion";

	public static PhotonHandler SP;

	public int updateInterval;

	public int updateIntervalOnSerialize;

	private int nextSendTickCount;

	private int nextSendTickCountOnSerialize;

	private static bool sendThreadShouldRun;

	public static bool AppQuits;

	public static Type PingImplementation;

	internal static CloudRegionCode BestRegionCodeCurrently = CloudRegionCode.none;

	internal static CloudRegionCode BestRegionCodeInPreferences
	{
		get
		{
			string @string = PlayerPrefs.GetString("PUNCloudBestRegion", string.Empty);
			if (!string.IsNullOrEmpty(@string))
			{
				return Region.Parse(@string);
			}
			return CloudRegionCode.none;
		}
		set
		{
			if (value == CloudRegionCode.none)
			{
				PlayerPrefs.DeleteKey("PUNCloudBestRegion");
			}
			else
			{
				PlayerPrefs.SetString("PUNCloudBestRegion", value.ToString());
			}
		}
	}

	protected void Awake()
	{
		if (SP != null && SP != this && SP.gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(SP.gameObject);
		}
		SP = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		updateInterval = 1000 / PhotonNetwork.SendRate;
		updateIntervalOnSerialize = 1000 / PhotonNetwork.SendRateOnSerialize;
		StartFallbackSendAckThread();
	}

	protected void OnApplicationQuit()
	{
		AppQuits = true;
		StopFallbackSendAckThread();
		PhotonNetwork.Disconnect();
	}

	protected void Update()
	{
		if (PhotonNetwork.NetworkingPeer == null)
		{
			Debug.LogError("NetworkPeer broke!");
		}
		else
		{
			if (PhotonNetwork.ConnectionStateDetailed == PeerState.PeerCreated || PhotonNetwork.ConnectionStateDetailed == PeerState.Disconnected || PhotonNetwork.OfflineMode || !PhotonNetwork.IsMessageQueueRunning)
			{
				return;
			}
			bool flag = true;
			while (PhotonNetwork.IsMessageQueueRunning && flag)
			{
				flag = PhotonNetwork.NetworkingPeer.DispatchIncomingCommands();
			}
			int num = (int)(Time.realtimeSinceStartup * 1000f);
			if (PhotonNetwork.IsMessageQueueRunning && num > nextSendTickCountOnSerialize)
			{
				PhotonNetwork.NetworkingPeer.RunViewUpdate();
				nextSendTickCountOnSerialize = num + updateIntervalOnSerialize;
				nextSendTickCount = 0;
			}
			num = (int)(Time.realtimeSinceStartup * 1000f);
			if (num > nextSendTickCount)
			{
				bool flag2 = true;
				while (PhotonNetwork.IsMessageQueueRunning && flag2)
				{
					flag2 = PhotonNetwork.NetworkingPeer.SendOutgoingCommands();
				}
				nextSendTickCount = num + updateInterval;
			}
		}
	}

	protected void OnLevelWasLoaded(int level)
	{
		PhotonNetwork.NetworkingPeer.NewSceneLoaded();
		PhotonNetwork.NetworkingPeer.SetLevelInPropsIfSynced(Application.loadedLevelName);
	}

	protected void OnJoinedRoom()
	{
		PhotonNetwork.NetworkingPeer.LoadLevelIfSynced();
	}

	protected void OnCreatedRoom()
	{
		PhotonNetwork.NetworkingPeer.SetLevelInPropsIfSynced(Application.loadedLevelName);
	}

	public static void StartFallbackSendAckThread()
	{
		if (!sendThreadShouldRun)
		{
			sendThreadShouldRun = true;
			SupportClass.CallInBackground(FallbackSendAckThread);
		}
	}

	public static void StopFallbackSendAckThread()
	{
		sendThreadShouldRun = false;
	}

	public static bool FallbackSendAckThread()
	{
		if (sendThreadShouldRun && PhotonNetwork.NetworkingPeer != null)
		{
			PhotonNetwork.NetworkingPeer.SendAcksOnly();
		}
		return sendThreadShouldRun;
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		switch (level)
		{
		case DebugLevel.ERROR:
			Debug.LogError(message);
			return;
		case DebugLevel.WARNING:
			Debug.LogWarning(message);
			return;
		case DebugLevel.INFO:
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log(message);
				return;
			}
			break;
		}
		if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
		{
			Debug.Log(message);
		}
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
	}

	public void OnEvent(EventData photonEvent)
	{
	}

	protected internal static void PingAvailableRegionsAndConnectToBest()
	{
		SP.StartCoroutine(SP.PingAvailableRegionsCoroutine(connectToBest: true));
	}

	internal IEnumerator PingAvailableRegionsCoroutine(bool connectToBest)
	{
		BestRegionCodeCurrently = CloudRegionCode.none;
		while (PhotonNetwork.NetworkingPeer.AvailableRegions == null)
		{
			if (PhotonNetwork.ConnectionStateDetailed != PeerState.ConnectingToNameServer && PhotonNetwork.ConnectionStateDetailed != PeerState.ConnectedToNameServer)
			{
				Debug.LogError("Call ConnectToNameServer to ping available regions.");
				yield break;
			}
			Debug.Log(string.Concat("Waiting for AvailableRegions. State: ", PhotonNetwork.ConnectionStateDetailed, " Server: ", PhotonNetwork.Server, " PhotonNetwork.networkingPeer.AvailableRegions ", PhotonNetwork.NetworkingPeer.AvailableRegions != null));
			yield return new WaitForSeconds(0.25f);
		}
		if (PhotonNetwork.NetworkingPeer.AvailableRegions == null || PhotonNetwork.NetworkingPeer.AvailableRegions.Count == 0)
		{
			Debug.LogError("No regions available. Are you sure your appid is valid and setup?");
			yield break;
		}
		PhotonPingManager pingManager = new PhotonPingManager();
		foreach (Region region in PhotonNetwork.NetworkingPeer.AvailableRegions)
		{
			SP.StartCoroutine(pingManager.PingSocket(region));
		}
		while (!pingManager.Done)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Region best = pingManager.BestRegion;
		BestRegionCodeCurrently = best.Code;
		BestRegionCodeInPreferences = best.Code;
		Debug.Log(string.Concat("Found best region: ", best.Code, " ping: ", best.Ping, ". Calling ConnectToRegionMaster() is: ", connectToBest));
		if (connectToBest)
		{
			PhotonNetwork.NetworkingPeer.ConnectToRegionMaster(best.Code);
		}
	}
}
