using ExitGames.Client.Photon;
using UnityEngine;

public class Room : RoomInfo
{

    public int Time
    {
        get
        {
            var stringarray = PhotonName.Split('`');
            if (stringarray.Length > 2 && int.TryParse(stringarray[3], out int Time))
            {
                return Time;
            }
            return 10;
        }
    }
    public new int PlayerCount
	{
		get
		{
			if (PhotonNetwork.PlayerList != null)
			{
				return PhotonNetwork.PlayerList.Length;
			}
			return 0;
		}
	}
	public string MapName
    {
		get
        {
			var stringarray = PhotonName.Split('`');
			if(stringarray.Length > 1)
            {
				return stringarray[1];
            }
			return "";
		}
	}
    public int Difficulty
    {
        get
        {
			var stringarray = PhotonName.Split('`');
			if (stringarray.Length > 1)
			{
				switch(stringarray[2].ToLower())
                {
					case "normal":
						return 0;
					case "hard":
						return 1;
					case "abnormal":
						return 2;
                }
			}
			return 0;
        }
    }
	public DayLight DayTime
	{
		get
		{
			var stringarray = PhotonName.Split('`');
			if (stringarray.Length > 3)
			{
				switch(stringarray[4].ToLower())
                {
					case "day":
						return DayLight.Day;
					case "dawn":
						return DayLight.Dawn;
					case "night":
						return DayLight.Night;
                }
			}
			return DayLight.Day;
		}
	}
	public string PhotonName
    {
        get
        {
            return NameField;
		}
		internal set
		{
			NameField = value;
		}
	}
	public string RoomName
    {
		get
		{ 
			var stringarray = PhotonName.Split('`');
			if(stringarray.Length > 0)
            {
                return stringarray[0];
            }

            return PhotonName;
		
		}
    }

	public new int MaxPlayers
	{
		get
		{
			return MaxPlayersField;
		}
		set
		{
			if (!Equals(PhotonNetwork.Room))
			{
				Debug.LogWarning("Can't set maxPlayers when not in that room.");
			}
			if (value > 255)
			{
				Debug.LogWarning("Can't set Room.maxPlayers to: " + value + ". Using max value: 255.");
				value = 255;
			}
			if (value != MaxPlayersField && !PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.NetworkingPeer.OpSetPropertiesOfRoom(new Hashtable { 
				{
					byte.MaxValue,
					(byte)value
				} }, broadcast: true, 0);
			}
			MaxPlayersField = (byte)value;
		}
	}

	public new bool Open
	{
		get
		{
			return OpenField;
		}
		set
		{
			if (!Equals(PhotonNetwork.Room))
			{
				Debug.LogWarning("Can't set open when not in that room.");
			}
			if (value != OpenField && !PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.NetworkingPeer.OpSetPropertiesOfRoom(new Hashtable { 
				{
					(byte)253,
					value
				} }, broadcast: true, 0);
			}
			OpenField = value;
		}
	}

	public new bool Visible
	{
		get
		{
			return VisibleField;
		}
		set
		{
			if (!Equals(PhotonNetwork.Room))
			{
				Debug.LogWarning("Can't set visible when not in that room.");
			}
			if (value != VisibleField && !PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.NetworkingPeer.OpSetPropertiesOfRoom(new Hashtable { 
				{
					(byte)254,
					value
				} }, broadcast: true, 0);
			}
			VisibleField = value;
		}
	}

	public string[] PropertiesListedInLobby { get; private set; }

	public bool AutoCleanUp => AutoCleanUpField;

	internal Room(string roomName, RoomOptions options)
		: base(roomName, null)
	{
		if (options == null)
		{
			options = new RoomOptions();
		}
		VisibleField = options.isVisible;
		OpenField = options.isOpen;
		MaxPlayersField = (byte)options.maxPlayers;
		AutoCleanUpField = false;
		CacheProperties(options.customRoomProperties);
		PropertiesListedInLobby = options.customRoomPropertiesForLobby;
	}

	public void SetCustomProperties(Hashtable propertiesToSet)
	{
		if (propertiesToSet != null)
		{
			base.CustomProperties.MergeStringKeys(propertiesToSet);
			base.CustomProperties.StripKeysWithNullValues();
			Hashtable gameProperties = propertiesToSet.StripToStringKeys();
			if (!PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.NetworkingPeer.OpSetCustomPropertiesOfRoom(gameProperties, broadcast: true, 0);
			}
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, propertiesToSet);
		}
	}

	public void SetPropertiesListedInLobby(string[] propsListedInLobby)
	{
        Hashtable hashtable = new Hashtable
        {
            [(byte)250] = propsListedInLobby
        };
        PhotonNetwork.NetworkingPeer.OpSetPropertiesOfRoom(hashtable, broadcast: false, 0);
		PropertiesListedInLobby = propsListedInLobby;
	}

	public override string ToString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", NameField, (!VisibleField) ? "hidden" : "visible", (!OpenField) ? "closed" : "open", MaxPlayersField, PlayerCount);
	}

	public new string ToStringFull()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", NameField, (!VisibleField) ? "hidden" : "visible", (!OpenField) ? "closed" : "open", MaxPlayersField, PlayerCount, CustomProperties.ToStringFull());
	}
}
