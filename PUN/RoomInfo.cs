using ExitGames.Client.Photon;

public class RoomInfo
{
	private Hashtable CustomPropertiesField = new Hashtable();

	protected byte MaxPlayersField;

	protected bool OpenField = true;

	protected bool VisibleField = true;

	protected bool AutoCleanUpField = PhotonNetwork.AutoCleanUpPlayerObjects;

	protected string NameField;

	public bool RemovedFromList { get; internal set; }

	public Hashtable CustomProperties => CustomPropertiesField;

	public string Name => NameField;

	public int PlayerCount { get; private set; }

	public bool IsLocalClientInside { get; set; }

	public byte MaxPlayers => MaxPlayersField;

	public bool Open => OpenField;

	public bool Visible => VisibleField;

	protected internal RoomInfo(string roomName, Hashtable properties)
	{
		CacheProperties(properties);
		NameField = roomName;
	}

	public override bool Equals(object p)
	{
		return p is Room room && NameField.Equals(room.NameField);
	}

	public override int GetHashCode()
	{
		return NameField.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", NameField, (!VisibleField) ? "hidden" : "visible", (!OpenField) ? "closed" : "open", MaxPlayersField, PlayerCount);
	}

	public string ToStringFull()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", NameField, (!VisibleField) ? "hidden" : "visible", (!OpenField) ? "closed" : "open", MaxPlayersField, PlayerCount, CustomPropertiesField.ToStringFull());
	}

	protected internal void CacheProperties(Hashtable propertiesToCache)
	{
		if (propertiesToCache == null || propertiesToCache.Count == 0 || CustomPropertiesField.Equals(propertiesToCache))
		{
			return;
		}
		if (propertiesToCache.ContainsKey((byte)251))
		{
			RemovedFromList = (bool)propertiesToCache[(byte)251];
			if (RemovedFromList)
			{
				return;
			}
		}
		if (propertiesToCache.ContainsKey(byte.MaxValue))
		{
			MaxPlayersField = (byte)propertiesToCache[byte.MaxValue];
		}
		if (propertiesToCache.ContainsKey((byte)253))
		{
			OpenField = (bool)propertiesToCache[(byte)253];
		}
		if (propertiesToCache.ContainsKey((byte)254))
		{
			VisibleField = (bool)propertiesToCache[(byte)254];
		}
		if (propertiesToCache.ContainsKey((byte)252))
		{
			PlayerCount = (byte)propertiesToCache[(byte)252];
		}
		if (propertiesToCache.ContainsKey((byte)249))
		{
			AutoCleanUpField = (bool)propertiesToCache[(byte)249];
		}
		CustomPropertiesField.MergeStringKeys(propertiesToCache);
	}
}
