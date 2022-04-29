using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class PhotonNetwork
{
    public delegate void EventCallback(byte eventCode, object content, int senderId);

    public const string VersionPUN = "1.28";

    public const string ServerSettingsAssetFile = "PhotonServerSettings";

    public const string ServerSettingsAssetPath = "Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset";

    internal static readonly PhotonHandler PhotonMono;

    internal static NetworkingPeer NetworkingPeer;

    public static readonly int MAX_VIEW_IDS;

    public static ServerSettings PhotonServerSettings;

    public static float PrecisionForVectorSynchronization;

    public static float PrecisionForQuaternionSynchronization;

    public static float PrecisionForFloatSynchronization;

    public static bool InstantiateInRoomOnly;

    public static PhotonLogLevel logLevel;

    public static bool UsePrefabCache;

    public static Dictionary<string, GameObject> PrefabCache;

    private static bool IsOfflineMode;

    private static Room OfflineModeRoom;

    public static bool UseNameServer;

    public static HashSet<GameObject> SendMonoMessageTargets;

    private static bool _mAutomaticallySyncScene;

    private static bool M_autoCleanUpPlayerObjects;

    private static bool AutoJoinLobbyField;

    private static int SendInterval;

    private static int SendIntervalOnSerialize;

    private static bool M_isMessageQueueRunning;

    public static EventCallback OnEventCall;

    internal static int LastUsedViewSubId;

    internal static int LastUsedViewSubIdStatic;

    internal static List<int> ManuallyAllocatedViewIds;

    public static string GameVersion
    {
        get
        {
            return NetworkingPeer.mAppVersion;
        }
        set
        {
            NetworkingPeer.mAppVersion = value;
        }
    }

    public static string ServerAddress => (NetworkingPeer == null) ? "<not connected>" : NetworkingPeer.ServerAddress;

    public static bool Connected
    {
        get
        {
            if (OfflineMode)
            {
                return true;
            }
            if (NetworkingPeer == null)
            {
                return false;
            }
            return !NetworkingPeer.IsInitialConnect && NetworkingPeer.State != PeerState.PeerCreated && NetworkingPeer.State != PeerState.Disconnected && NetworkingPeer.State != PeerState.Disconnecting && NetworkingPeer.State != PeerState.ConnectingToNameServer;
        }
    }

    public static bool Connecting => NetworkingPeer.IsInitialConnect && !OfflineMode;

    public static bool ConnectedAndReady
    {
        get
        {
            if (!Connected)
            {
                return false;
            }
            if (OfflineMode)
            {
                return true;
            }
            switch (ConnectionStateDetailed)
            {
                case PeerState.PeerCreated:
                case PeerState.ConnectingToGameserver:
                case PeerState.Joining:
                case PeerState.Leaving:
                case PeerState.ConnectingToMasterserver:
                case PeerState.Disconnecting:
                case PeerState.Disconnected:
                case PeerState.ConnectingToNameServer:
                case PeerState.Authenticating:
                    return false;

                default:
                    return true;
            }
        }
    }

    public static ConnectionState ConnectionState
    {
        get
        {
            if (OfflineMode)
            {
                return ConnectionState.Connected;
            }
            if (NetworkingPeer == null)
            {
                return ConnectionState.Disconnected;
            }
            return NetworkingPeer.PeerState switch
            {
                PeerStateValue.Disconnected => ConnectionState.Disconnected,
                PeerStateValue.Connecting => ConnectionState.Connecting,
                PeerStateValue.Connected => ConnectionState.Connected,
                PeerStateValue.Disconnecting => ConnectionState.Disconnecting,
                PeerStateValue.InitializingApplication => ConnectionState.InitializingApplication,
                _ => ConnectionState.Disconnected,
            };
        }
    }

    public static PeerState ConnectionStateDetailed
    {
        get
        {
            if (OfflineMode)
            {
                return (OfflineModeRoom == null) ? PeerState.ConnectedToMaster : PeerState.Joined;
            }
            if (NetworkingPeer == null)
            {
                return PeerState.Disconnected;
            }
            return NetworkingPeer.State;
        }
    }

    public static AuthenticationValues AuthValues
    {
        get
        {
            return (NetworkingPeer == null) ? null : NetworkingPeer.CustomAuthenticationValues;
        }
        set
        {
            if (NetworkingPeer != null)
            {
                NetworkingPeer.CustomAuthenticationValues = value;
            }
        }
    }
    public static Room Room
    {
        get
        {
            if (IsOfflineMode)
            {
                return OfflineModeRoom;
            }
            return NetworkingPeer.mCurrentGame;
        }
    }

    public static PhotonPlayer Player
    {
        get
        {
            if (NetworkingPeer == null)
            {
                return null;
            }
            return NetworkingPeer.mLocalActor;
        }
    }

    public static PhotonPlayer MasterClient
    {
        get
        {
            if (NetworkingPeer == null)
            {
                return null;
            }
            return NetworkingPeer.mMasterClient;
        }
    }

    public static string PlayerName
    {
        get
        {
            return NetworkingPeer.PlayerName;
        }
        set
        {
            NetworkingPeer.PlayerName = value;
        }
    }

    public static PhotonPlayer[] PlayerList
    {
        get
        {
            if (NetworkingPeer == null)
            {
                return new PhotonPlayer[0];
            }
            return NetworkingPeer.mPlayerListCopy;
        }
    }

    public static PhotonPlayer[] OtherPlayers
    {
        get
        {
            if (NetworkingPeer == null)
            {
                return new PhotonPlayer[0];
            }
            return NetworkingPeer.mOtherPlayerListCopy;
        }
    }

    public static List<FriendInfo> Friends { get; internal set; }

    public static int FriendsListAge => (NetworkingPeer != null) ? NetworkingPeer.FriendsListAge : 0;

    public static bool OfflineMode
    {
        get
        {
            return IsOfflineMode;
        }
        set
        {
            if (value == IsOfflineMode)
            {
                return;
            }
            if (value && Connected)
            {
                Debug.LogError("Can't start OFFLINE mode while connected!");
                return;
            }
            if (NetworkingPeer.PeerState != 0)
            {
                NetworkingPeer.Disconnect();
            }
            IsOfflineMode = value;
            if (IsOfflineMode)
            {
                NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
                NetworkingPeer.ChangeLocalID(1);
                NetworkingPeer.mMasterClient = Player;
            }
            else
            {
                OfflineModeRoom = null;
                NetworkingPeer.ChangeLocalID(-1);
                NetworkingPeer.mMasterClient = null;
            }
        }
    }

    [Obsolete("Used for compatibility with Unity networking only.")]
    public static int MaxConnections
    {
        get
        {
            if (Room == null)
            {
                return 0;
            }
            return Room.MaxPlayers;
        }
        set
        {
            Room.MaxPlayers = value;
        }
    }

    public static bool AutomaticallySyncScene
    {
        get
        {
            return _mAutomaticallySyncScene;
        }
        set
        {
            _mAutomaticallySyncScene = value;
            if (_mAutomaticallySyncScene && Room != null)
            {
                NetworkingPeer.LoadLevelIfSynced();
            }
        }
    }

    public static bool AutoCleanUpPlayerObjects
    {
        get
        {
            return M_autoCleanUpPlayerObjects;
        }
        set
        {
            if (Room != null)
            {
                Debug.LogError("Setting autoCleanUpPlayerObjects while in a room is not supported.");
            }
            else
            {
                M_autoCleanUpPlayerObjects = value;
            }
        }
    }

    public static bool AutoJoinLobby
    {
        get
        {
            return AutoJoinLobbyField;
        }
        set
        {
            AutoJoinLobbyField = value;
        }
    }

    public static bool InsideLobby => NetworkingPeer.insideLobby;

    public static TypedLobby Lobby
    {
        get
        {
            return NetworkingPeer.lobby;
        }
        set
        {
            NetworkingPeer.lobby = value;
        }
    }

    public static int SendRate
    {
        get
        {
            return 1000 / SendInterval;
        }
        set
        {
            SendInterval = 1000 / value;
            if (PhotonMono != null)
            {
                PhotonMono.updateInterval = SendInterval;
            }
            if (value < SendRateOnSerialize)
            {
                SendRateOnSerialize = value;
            }
        }
    }

    public static int SendRateOnSerialize
    {
        get
        {
            return 1000 / SendIntervalOnSerialize;
        }
        set
        {
            if (value > SendRate)
            {
                Debug.LogError("Error, can not set the OnSerialize SendRate more often then the overall SendRate");
                value = SendRate;
            }
            SendIntervalOnSerialize = 1000 / value;
            if (PhotonMono != null)
            {
                PhotonMono.updateIntervalOnSerialize = SendIntervalOnSerialize;
            }
        }
    }

    public static bool IsMessageQueueRunning
    {
        get
        {
            return M_isMessageQueueRunning;
        }
        set
        {
            if (value)
            {
                PhotonHandler.StartFallbackSendAckThread();
            }
            NetworkingPeer.IsSendingOnlyAcks = !value;
            M_isMessageQueueRunning = value;
        }
    }

    public static int UnreliableCommandsLimit
    {
        get
        {
            return NetworkingPeer.LimitOfUnreliableCommands;
        }
        set
        {
            NetworkingPeer.LimitOfUnreliableCommands = value;
        }
    }

    public static double Time
    {
        get
        {
            if (OfflineMode)
            {
                return UnityEngine.Time.time;
            }
            return (uint)NetworkingPeer.ServerTimeInMilliSeconds / 1000.0;
        }
    }

    public static bool IsMasterClient
    {
        get
        {
            if (OfflineMode)
            {
                return true;
            }
            return NetworkingPeer.mMasterClient == NetworkingPeer.mLocalActor;
        }
    }

    public static bool InRoom => ConnectionStateDetailed == PeerState.Joined;

    public static bool IsNonMasterClientInRoom => !IsMasterClient && Room != null;

    public static int CountOfPlayersOnMaster => NetworkingPeer.mPlayersOnMasterCount;

    public static int CountOfPlayersInRooms => NetworkingPeer.mPlayersInRoomsCount;

    public static int CountOfPlayers => NetworkingPeer.mPlayersInRoomsCount + NetworkingPeer.mPlayersOnMasterCount;

    public static int CountOfRooms => NetworkingPeer.mGameCount;

    public static bool NetworkStatisticsEnabled
    {
        get
        {
            return NetworkingPeer.TrafficStatsEnabled;
        }
        set
        {
            NetworkingPeer.TrafficStatsEnabled = value;
        }
    }

    public static int ResentReliableCommands => NetworkingPeer.ResentReliableCommands;

    public static bool CrcCheckEnabled
    {
        get
        {
            return NetworkingPeer.CrcEnabled;
        }
        set
        {
            if (!Connected && !Connecting)
            {
                NetworkingPeer.CrcEnabled = value;
            }
            else
            {
                Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + NetworkingPeer.CrcEnabled);
            }
        }
    }

    public static int PacketLossByCrcCheck => NetworkingPeer.PacketLossByCrc;

    public static int MaxResendsBeforeDisconnect
    {
        get
        {
            return NetworkingPeer.SentCountAllowance;
        }
        set
        {
            if (value < 3)
            {
                value = 3;
            }
            if (value > 10)
            {
                value = 10;
            }
            NetworkingPeer.SentCountAllowance = value;
        }
    }

    public static ServerConnection Server => NetworkingPeer.server;

    static PhotonNetwork()
    {
        MAX_VIEW_IDS = 1000;
        PhotonServerSettings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));
        PrecisionForVectorSynchronization = 9.9E-05f;
        PrecisionForQuaternionSynchronization = 1f;
        PrecisionForFloatSynchronization = 0.01f;
        InstantiateInRoomOnly = true;
        logLevel = PhotonLogLevel.ErrorsOnly;
        UsePrefabCache = true;
        PrefabCache = new Dictionary<string, GameObject>();
        IsOfflineMode = false;
        OfflineModeRoom = null;
        UseNameServer = true;
        _mAutomaticallySyncScene = false;
        M_autoCleanUpPlayerObjects = true;
        AutoJoinLobbyField = true;
        SendInterval = 50;
        SendIntervalOnSerialize = 100;
        M_isMessageQueueRunning = true;
        LastUsedViewSubId = 0;
        LastUsedViewSubIdStatic = 0;
        ManuallyAllocatedViewIds = new List<int>();
        Application.runInBackground = true;
        GameObject gameObject = new GameObject();
        PhotonMono = gameObject.AddComponent<PhotonHandler>();
        gameObject.name = "PhotonMono";
        gameObject.hideFlags = HideFlags.HideInHierarchy;
        NetworkingPeer = new NetworkingPeer(PhotonMono, string.Empty, ConnectionProtocol.Udp);
        CustomTypes.Register();
    }

    public static bool SetMasterClient(PhotonPlayer masterClientPlayer)
    {
        if (!VerifyCanUseNetwork() || !IsMasterClient)
        {
            return false;
        }
        return NetworkingPeer.SetMasterClient(masterClientPlayer.ID, sync: true);
    }

    public static void NetworkStatisticsReset()
    {
        NetworkingPeer.TrafficStatsReset();
    }

    public static string NetworkStatisticsToString()
    {
        if (NetworkingPeer == null || OfflineMode)
        {
            return "Offline or in OfflineMode. No VitalStats available.";
        }
        return NetworkingPeer.VitalStatsToString(all: false);
    }

    public static void SwitchToProtocol(ConnectionProtocol cp)
    {
        if (NetworkingPeer.UsedProtocol != cp)
        {
            try
            {
                NetworkingPeer.Disconnect();
                NetworkingPeer.StopThread();
            }
            catch
            {
            }
            NetworkingPeer = new NetworkingPeer(PhotonMono, string.Empty, cp);
            Debug.Log("Protocol switched to: " + cp);
        }
    }

    public static void InternalCleanPhotonMonoFromSceneIfStuck()
    {
        if (!(UnityEngine.Object.FindObjectsOfType(typeof(PhotonHandler)) is PhotonHandler[] array) || array.Length <= 0)
        {
            return;
        }
        Debug.Log("Cleaning up hidden PhotonHandler instances in scene. Please save it. This is not an issue.");
        PhotonHandler[] array2 = array;
        foreach (PhotonHandler photonHandler in array2)
        {
            photonHandler.gameObject.hideFlags = HideFlags.None;
            if (photonHandler.gameObject != null && photonHandler.gameObject.name == "PhotonMono")
            {
                UnityEngine.Object.DestroyImmediate(photonHandler.gameObject);
            }
            UnityEngine.Object.DestroyImmediate(photonHandler);
        }
    }

    public static bool ConnectUsingSettings(string gameVersion)
    {
        if (PhotonServerSettings == null)
        {
            Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
            return false;
        }
        SwitchToProtocol(PhotonServerSettings.Protocol);
        NetworkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
        {
            OfflineMode = true;
            return true;
        }
        if (OfflineMode)
        {
            Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
        }
        OfflineMode = false;
        IsMessageQueueRunning = true;
        NetworkingPeer.IsInitialConnect = true;
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted)
        {
            NetworkingPeer.IsUsingNameServer = false;
            NetworkingPeer.MasterServerAddress = PhotonServerSettings.ServerAddress + ":" + PhotonServerSettings.ServerPort;
            return NetworkingPeer.Connect(NetworkingPeer.MasterServerAddress, ServerConnection.MasterServer);
        }
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
        {
            return ConnectToBestCloudServer(gameVersion);
        }
        return NetworkingPeer.ConnectToRegionMaster(PhotonServerSettings.PreferredRegion);
    }

    public static bool ConnectToMaster(string masterServerAddress, int port, string appID, string gameVersion)
    {
        if (NetworkingPeer.PeerState != 0)
        {
            Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + NetworkingPeer.PeerState);
            return false;
        }
        if (OfflineMode)
        {
            OfflineMode = false;
            Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
        }
        if (!IsMessageQueueRunning)
        {
            IsMessageQueueRunning = true;
            Debug.LogWarning("ConnectToMaster() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
        }
        NetworkingPeer.SetApp(appID, gameVersion);
        NetworkingPeer.IsUsingNameServer = false;
        NetworkingPeer.IsInitialConnect = true;
        NetworkingPeer.MasterServerAddress = masterServerAddress + ":" + port;
        return NetworkingPeer.Connect(NetworkingPeer.MasterServerAddress, ServerConnection.MasterServer);
    }

    public static bool ConnectToBestCloudServer(string gameVersion)
    {
        if (PhotonServerSettings == null)
        {
            Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
            return false;
        }
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
        {
            return ConnectUsingSettings(gameVersion);
        }
        NetworkingPeer.IsInitialConnect = true;
        NetworkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
        CloudRegionCode bestRegionCodeInPreferences = PhotonHandler.BestRegionCodeInPreferences;
        if (bestRegionCodeInPreferences != CloudRegionCode.none)
        {
            Debug.Log("Best region found in PlayerPrefs. Connecting to: " + bestRegionCodeInPreferences);
            return NetworkingPeer.ConnectToRegionMaster(bestRegionCodeInPreferences);
        }
        return NetworkingPeer.ConnectToNameServer();
    }

    public static void OverrideBestCloudServer(CloudRegionCode region)
    {
        PhotonHandler.BestRegionCodeInPreferences = region;
    }

    public static void RefreshCloudServerRating()
    {
        throw new NotImplementedException("not available at the moment");
    }

    public static void Disconnect()
    {
        if (OfflineMode)
        {
            OfflineMode = false;
            OfflineModeRoom = null;
            NetworkingPeer.State = PeerState.Disconnecting;
            NetworkingPeer.OnStatusChanged(StatusCode.Disconnect);
        }
        else if (NetworkingPeer != null)
        {
            NetworkingPeer.Disconnect();
        }
    }

    [Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
    public static void InitializeSecurity()
    {
    }

    public static bool FindFriends(string[] friendsToFind)
    {
        if (NetworkingPeer == null || IsOfflineMode)
        {
            return false;
        }
        return NetworkingPeer.OpFindFriends(friendsToFind);
    }

    [Obsolete("Use overload with RoomOptions and TypedLobby parameters.")]
    public static bool CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.isVisible = isVisible;
        roomOptions.isOpen = isOpen;
        roomOptions.maxPlayers = maxPlayers;
        return CreateRoom(roomName, roomOptions, null);
    }

    [Obsolete("Use overload with RoomOptions and TypedLobby parameters.")]
    public static bool CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers, Hashtable customRoomProperties, string[] propsToListInLobby)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.isVisible = isVisible;
        roomOptions.isOpen = isOpen;
        roomOptions.maxPlayers = maxPlayers;
        roomOptions.customRoomProperties = customRoomProperties;
        roomOptions.customRoomPropertiesForLobby = propsToListInLobby;
        return CreateRoom(roomName, roomOptions, null);
    }

    public static bool CreateRoom(string roomName)
    {
        return CreateRoom(roomName, null, null);
    }

    public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
    {
        if (OfflineMode)
        {
            if (OfflineModeRoom != null)
            {
                Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            OfflineModeRoom = new Room(roomName, roomOptions);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
            return true;
        }
        if (NetworkingPeer.server != 0 || !ConnectedAndReady)
        {
            Debug.LogError("CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }
        return NetworkingPeer.OpCreateGame(roomName, roomOptions, typedLobby);
    }

    [Obsolete("Use overload with roomOptions and TypedLobby parameter.")]
    public static bool JoinRoom(string roomName, bool createIfNotExists)
    {
        if (ConnectionStateDetailed == PeerState.Joining || ConnectionStateDetailed == PeerState.Joined || ConnectionStateDetailed == PeerState.ConnectedToGameserver)
        {
            Debug.LogError("JoinRoom aborted: You can only join a room while not currently connected/connecting to a room.");
        }
        else if (Room != null)
        {
            Debug.LogError("JoinRoom aborted: You are already in a room!");
        }
        else
        {
            if (!(roomName == string.Empty))
            {
                if (OfflineMode)
                {
                    OfflineModeRoom = new Room(roomName, null);
                    NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
                    return true;
                }
                return NetworkingPeer.OpJoinRoom(roomName, null, null, createIfNotExists);
            }
            Debug.LogError("JoinRoom aborted: You must specifiy a room name!");
        }
        return false;
    }

    public static bool JoinRoom(string roomName)
    {
        if (OfflineMode)
        {
            if (OfflineModeRoom != null)
            {
                Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            OfflineModeRoom = new Room(roomName, null);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
            return true;
        }
        if (NetworkingPeer.server != 0 || !ConnectedAndReady)
        {
            Debug.LogError("JoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
            return false;
        }
        return NetworkingPeer.OpJoinRoom(roomName, null, null, createIfNotExists: false);
    }

    public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
    {
        if (OfflineMode)
        {
            if (OfflineModeRoom != null)
            {
                Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            OfflineModeRoom = new Room(roomName, roomOptions);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
            return true;
        }
        if (NetworkingPeer.server != 0 || !ConnectedAndReady)
        {
            Debug.LogError("JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
            return false;
        }
        return NetworkingPeer.OpJoinRoom(roomName, roomOptions, typedLobby, createIfNotExists: true);
    }

    public static bool JoinRandomRoom()
    {
        return JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null);
    }

    public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
    {
        return JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null);
    }

    public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
    {
        if (OfflineMode)
        {
            if (OfflineModeRoom != null)
            {
                Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            OfflineModeRoom = new Room("offline room", null);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
            return true;
        }
        if (NetworkingPeer.server != 0 || !ConnectedAndReady)
        {
            Debug.LogError("JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }
        Hashtable hashtable = new Hashtable();
        hashtable.MergeStringKeys(expectedCustomRoomProperties);
        if (expectedMaxPlayers > 0)
        {
            hashtable[byte.MaxValue] = expectedMaxPlayers;
        }
        return NetworkingPeer.OpJoinRandomRoom(hashtable, 0, null, matchingType, typedLobby, sqlLobbyFilter);
    }

    public static bool JoinLobby()
    {
        return JoinLobby(null);
    }

    public static bool JoinLobby(TypedLobby typedLobby)
    {
        if (Connected && Server == ServerConnection.MasterServer)
        {
            if (typedLobby == null)
            {
                typedLobby = TypedLobby.Default;
            }
            bool flag = NetworkingPeer.OpJoinLobby(typedLobby);
            if (flag)
            {
                NetworkingPeer.lobby = typedLobby;
            }
            return flag;
        }
        return false;
    }

    public static bool LeaveLobby()
    {
        if (Connected && Server == ServerConnection.MasterServer)
        {
            return NetworkingPeer.OpLeaveLobby();
        }
        return false;
    }

    public static bool LeaveRoom()
    {
        if (OfflineMode)
        {
            OfflineModeRoom = null;
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
            return true;
        }
        if (Room == null)
        {
            Debug.LogWarning("PhotonNetwork.room is null. You don't have to call LeaveRoom() when you're not in one. State: " + ConnectionStateDetailed);
        }
        return NetworkingPeer.OpLeave();
    }

    public static RoomInfo[] GetRoomList()
    {
        if (OfflineMode || NetworkingPeer == null)
        {
            return new RoomInfo[0];
        }
        return NetworkingPeer.mGameListCopy;
    }

    public static void SetPlayerCustomProperties(Hashtable customProperties)
    {
        if (customProperties == null)
        {
            customProperties = new Hashtable();
            foreach (object key in Player.CustomProperties.Keys)
            {
                customProperties[(string)key] = null;
            }
        }
        if (Room != null && Room.IsLocalClientInside)
        {
            Player.SetCustomProperties(customProperties);
        }
        else
        {
            Player.InternalCacheProperties(customProperties);
        }
    }

    public static bool RaiseEvent(byte eventCode, object eventContent, bool sendReliable, RaiseEventOptions options)
    {
        if (!InRoom || eventCode >= 200)
        {
            Debug.LogWarning("RaiseEvent() failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
            return false;
        }
        return NetworkingPeer.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
    }

    public static int AllocateViewID()
    {
        int num = AllocateViewID(Player.ID);
        ManuallyAllocatedViewIds.Add(num);
        return num;
    }

    public static void UnAllocateViewID(int viewID)
    {
        ManuallyAllocatedViewIds.Remove(viewID);
        if (NetworkingPeer.photonViewList.ContainsKey(viewID))
        {
            Debug.LogWarning($"Unallocated manually used viewID: {viewID} but found it used still in a PhotonView: {NetworkingPeer.photonViewList[viewID]}");
        }
    }

    private static int AllocateViewID(int ownerId)
    {
        if (ownerId == 0)
        {
            int num = LastUsedViewSubIdStatic;
            int num2 = ownerId * MAX_VIEW_IDS;
            for (int i = 1; i < MAX_VIEW_IDS; i++)
            {
                num = (num + 1) % MAX_VIEW_IDS;
                if (num != 0)
                {
                    int num3 = num + num2;
                    if (!NetworkingPeer.photonViewList.ContainsKey(num3))
                    {
                        LastUsedViewSubIdStatic = num;
                        return num3;
                    }
                }
            }
            throw new Exception($"AllocateViewID() failed. Room (user {ownerId}) is out of subIds, as all room viewIDs are used.");
        }
        int num4 = LastUsedViewSubId;
        int num5 = ownerId * MAX_VIEW_IDS;
        for (int j = 1; j < MAX_VIEW_IDS; j++)
        {
            num4 = (num4 + 1) % MAX_VIEW_IDS;
            if (num4 != 0)
            {
                int num6 = num4 + num5;
                if (!NetworkingPeer.photonViewList.ContainsKey(num6) && !ManuallyAllocatedViewIds.Contains(num6))
                {
                    LastUsedViewSubId = num4;
                    return num6;
                }
            }
        }
        throw new Exception($"AllocateViewID() failed. User {ownerId} is out of subIds, as all viewIDs are used.");
    }

    private static int[] AllocateSceneViewIDs(int countOfNewViews)
    {
        int[] array = new int[countOfNewViews];
        for (int i = 0; i < countOfNewViews; i++)
        {
            array[i] = AllocateViewID(0);
        }
        return array;
    }

    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group)
    {
        return Instantiate(prefabName, position, rotation, group, null);
    }

    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
    {
        if (!Connected || (InstantiateInRoomOnly && !InRoom))
        {
            Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + ConnectionStateDetailed);
            return null;
        }
        if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out var value))
        {
            value = (GameObject)Resources.Load(prefabName, typeof(GameObject));
            if (UsePrefabCache)
            {
                PrefabCache.Add(prefabName, value);
            }
        }
        if (value == null)
        {
            Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }
        if (value.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            return null;
        }
        Component[] photonViewsInChildren = value.GetPhotonViewsInChildren();
        int[] array = new int[photonViewsInChildren.Length];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = AllocateViewID(Player.ID);
        }
        Hashtable evData = NetworkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, isGlobalObject: false);
        return NetworkingPeer.DoInstantiate(evData, NetworkingPeer.mLocalActor, value);
    }

    public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
    {
        if (!Connected || (InstantiateInRoomOnly && !InRoom))
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + ConnectionStateDetailed);
            return null;
        }
        if (!IsMasterClient)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client is not the MasterClient in this room.");
            return null;
        }
        if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out var value))
        {
            value = (GameObject)Resources.Load(prefabName, typeof(GameObject));
            if (UsePrefabCache)
            {
                PrefabCache.Add(prefabName, value);
            }
        }
        if (value == null)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }
        if (value.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            return null;
        }
        Component[] photonViewsInChildren = value.GetPhotonViewsInChildren();
        int[] array = AllocateSceneViewIDs(photonViewsInChildren.Length);
        if (array == null)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". No ViewIDs are free to use. Max is: " + MAX_VIEW_IDS);
            return null;
        }
        Hashtable evData = NetworkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, isGlobalObject: true);
        return NetworkingPeer.DoInstantiate(evData, NetworkingPeer.mLocalActor, value);
    }

    public static int GetPing()
    {
        return NetworkingPeer.RoundTripTime;
    }

    public static void FetchServerTimestamp()
    {
        if (NetworkingPeer != null)
        {
            NetworkingPeer.FetchServerTimestamp();
        }
    }

    public static void SendOutgoingCommands()
    {
        if (VerifyCanUseNetwork())
        {
            while (NetworkingPeer.SendOutgoingCommands())
            {
            }
        }
    }

    public static bool CloseConnection(PhotonPlayer kickPlayer)
    {
        if (!VerifyCanUseNetwork())
        {
            return false;
        }
        if (!Player.IsMasterClient)
        {
            Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
            return false;
        }
        if (kickPlayer == null)
        {
            Debug.LogError("CloseConnection: No such player connected!");
            return false;
        }
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.TargetActors = new int[1] { kickPlayer.ID };
        RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
        return NetworkingPeer.OpRaiseEvent(203, null, sendReliable: true, raiseEventOptions2);
    }

    public static void Destroy(PhotonView targetView)
    {
        if (targetView != null)
        {
            NetworkingPeer.RemoveInstantiatedGO(targetView.gameObject, !InRoom);
        }
        else
        {
            Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
        }
    }

    public static void Destroy(GameObject targetGo)
    {
        NetworkingPeer.RemoveInstantiatedGO(targetGo, !InRoom);
    }

    public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
    {
        if (Player == null)
        {
            Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
        }
        DestroyPlayerObjects(targetPlayer.ID);
    }

    public static void DestroyPlayerObjects(int targetPlayerId)
    {
        if (VerifyCanUseNetwork())
        {
            if (Player.IsMasterClient || targetPlayerId == Player.ID)
            {
                NetworkingPeer.DestroyPlayerObjects(targetPlayerId, localOnly: false);
            }
            else
            {
                Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + IsMasterClient);
            }
        }
    }

    public static void DestroyAll()
    {
        if (IsMasterClient)
        {
            NetworkingPeer.DestroyAll(localOnly: false);
        }
        else
        {
            Debug.LogError("Couldn't call DestroyAll() as only the master client is allowed to call this.");
        }
    }

    public static void RemoveRPCs(PhotonPlayer targetPlayer)
    {
        if (VerifyCanUseNetwork())
        {
            if (!targetPlayer.IsLocal && !IsMasterClient)
            {
                Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
            }
            else
            {
                NetworkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
            }
        }
    }

    public static void RemoveRPCs(PhotonView targetPhotonView)
    {
        if (VerifyCanUseNetwork())
        {
            NetworkingPeer.CleanRpcBufferIfMine(targetPhotonView);
        }
    }

    public static void RemoveRPCsInGroup(int targetGroup)
    {
        if (VerifyCanUseNetwork())
        {
            NetworkingPeer.RemoveRPCsInGroup(targetGroup);
        }
    }

    internal static void RPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
    {
        if (VerifyCanUseNetwork())
        {
            if (Room == null)
            {
                Debug.LogWarning("Cannot send RPCs in Lobby! RPC dropped.");
            }
            else if (NetworkingPeer != null)
            {
                NetworkingPeer.RPC(view, methodName, target, parameters);
            }
            else
            {
                Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
            }
        }
    }

    internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, params object[] parameters)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        if (Room == null)
        {
            Debug.LogWarning("Cannot send RPCs in Lobby, only processed locally");
            return;
        }
        if (Player == null)
        {
            Debug.LogError("Error; Sending RPC to player null! Aborted \"" + methodName + "\"");
        }
        if (NetworkingPeer != null)
        {
            NetworkingPeer.RPC(view, methodName, targetPlayer, parameters);
        }
        else
        {
            Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
        }
    }

    public static void SetReceivingEnabled(int group, bool enabled)
    {
        if (VerifyCanUseNetwork())
        {
            NetworkingPeer.SetReceivingEnabled(group, enabled);
        }
    }

    public static void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if (VerifyCanUseNetwork())
        {
            NetworkingPeer.SetReceivingEnabled(enableGroups, disableGroups);
        }
    }

    public static void SetSendingEnabled(int group, bool enabled)
    {
        if (VerifyCanUseNetwork())
        {
            NetworkingPeer.SetSendingEnabled(group, enabled);
        }
    }

    public static void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if (VerifyCanUseNetwork())
        {
            NetworkingPeer.SetSendingEnabled(enableGroups, disableGroups);
        }
    }

    public static void SetLevelPrefix(short prefix)
    {
        if (VerifyCanUseNetwork())
        {
            NetworkingPeer.SetLevelPrefix(prefix);
        }
    }

    private static bool VerifyCanUseNetwork()
    {
        if (Connected)
        {
            return true;
        }
        Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
        return false;
    }

    public static void LoadLevel(int levelNumber)
    {
        NetworkingPeer.SetLevelInPropsIfSynced(levelNumber);
        IsMessageQueueRunning = false;
        NetworkingPeer.loadingLevelAndPausedNetwork = true;
        Application.LoadLevel(levelNumber);
    }

    public static void LoadLevel(string levelName)
    {
        NetworkingPeer.SetLevelInPropsIfSynced(levelName);
        IsMessageQueueRunning = false;
        NetworkingPeer.loadingLevelAndPausedNetwork = true;
        Application.LoadLevel(levelName);
    }

    public static bool WebRpc(string name, object parameters)
    {
        return NetworkingPeer.WebRpc(name, parameters);
    }
}