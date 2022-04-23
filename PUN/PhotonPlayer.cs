using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhotonPlayer
{
    private int actorID = -1;

    private string nameField = string.Empty;

    public readonly bool isLocal;

    public object TagObject;

    public int ID => actorID;

    public bool IsDead
    {
        get
        {
            if (customProperties.ContainsKey(PhotonPlayerProperty.dead) && customProperties[PhotonPlayerProperty.dead] is bool State)
            {
                return State;
            }

            return false;
        }
    }

    public int Kills
    {
        get
        {
            if (customProperties.ContainsKey(PhotonPlayerProperty.kills) && customProperties[PhotonPlayerProperty.kills] is int Kills)
            {
                return Kills;
            }
            return -1;
        }
    }

    public int Deaths
    {
        get
        {
            if (customProperties.ContainsKey(PhotonPlayerProperty.deaths) && customProperties[PhotonPlayerProperty.deaths] is int Deaths)
            {
                return Deaths;
            }
            return -1;
        }
    }

    public int HighestDmg
    {
        get
        {
            if (customProperties.ContainsKey(PhotonPlayerProperty.max_dmg) && customProperties[PhotonPlayerProperty.max_dmg] is int HighestDmg)
            {
                return HighestDmg;
            }
            return -1;
        }
    }

    public int TotalDmg
    {
        get
        {
            if(customProperties.ContainsKey(PhotonPlayerProperty.total_dmg) &&customProperties[PhotonPlayerProperty.total_dmg] is int TotalDmg)
            {
                return TotalDmg;
            }
            return -1;
        }
    }
    public string Name
    {
        get
        {
            if(customProperties.ContainsKey(PhotonPlayerProperty.name) && customProperties[PhotonPlayerProperty.name] is string name && name.RemoveHex().Trim() != "")
            {
                return name.RemoveHex();
            }
            return "Nameless fag";
        }
    }
    /// <summary>
    /// True if value = 2
    /// </summary>
    public bool IsTitan
    {
        get
        {
            return customProperties.ContainsKey(PhotonPlayerProperty.isTitan) && customProperties[PhotonPlayerProperty.isTitan] is int value && value == 2;
        }
    }
    public bool IsAhss
    {
        get
        {
            return customProperties.ContainsKey(PhotonPlayerProperty.isTitan) && customProperties[PhotonPlayerProperty.isTitan] is int value && value == 3;
        }
    }

    public int Team
    {
        get
        {
            if( customProperties.ContainsKey(PhotonPlayerProperty.team) && customProperties[PhotonPlayerProperty.team] is int TeamId)
            {
                return TeamId;
            }
            return 1;
        }
    }
    public string PhotonName
    {
        get
        {
            return nameField;
        }
        set
        {
            if (!isLocal)
            {
                Debug.LogError("Error: Cannot change the name of a remote player!");
            }
            else
            {
                nameField = value;
            }
        }
    }

    public bool isMasterClient => PhotonNetwork.NetworkingPeer.mMasterClient == this;

    public Hashtable customProperties { get; private set; }

    public Hashtable allProperties
    {
        get
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Merge(customProperties);
            hashtable[byte.MaxValue] = PhotonName;
            return hashtable;
        }
    }

    public PhotonPlayer(bool isLocal, int actorID, string name)
    {
        customProperties = new Hashtable();
        this.isLocal = isLocal;
        this.actorID = actorID;
        nameField = name;
    }

    protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
    {
        customProperties = new Hashtable();
        this.isLocal = isLocal;
        this.actorID = actorID;
        InternalCacheProperties(properties);
    }

    public override bool Equals(object p)
    {
        return p is PhotonPlayer photonPlayer && GetHashCode() == photonPlayer.GetHashCode();
    }

    public override int GetHashCode()
    {
        return ID;
    }
    public string PlayerListString()
    {
        try
        {
            string text = "";

            string text2 = text;
            text = text2 + "[ffffff]#" + ID + " ";
            if (isLocal)
            {
                text += "> ";
            }
            if (isMasterClient)
            {
                text += "M ";
            }
            if (IsDead)
            {
                text = text + "[" + ColorSet.color_red + "] *dead* ";
            }
            if (IsTitan)
            {
                text = text + "[" + ColorSet.color_titan_player + "] T ";
            }
            if (!IsTitan)
            {
                text = Team == 1 ? $"{text} [{ColorSet.color_human}] {(IsAhss ? "A" : "H")} " : $"({text} [{ColorSet.color_human_1}] {(IsAhss ? "A" : "H")} )";
            }

            text2 = text;
            text = string.Concat(text2, string.Empty, Name, "[ffffff]:", Kills, "/", Deaths, "/", HighestDmg, "/", TotalDmg);
            if (IsDead)
            {
                text += "[-]";
            }
            text += "\n";
            return text;

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return "";
        }

    }
    public static string GetPlayerList()
    {
        string text = "";
        foreach(var player in PhotonNetwork.PlayerList.OrderBy(x => x.ID))
        {
            text += player.PlayerListString();
        }
        return text;
    }
    internal void InternalChangeLocalID(int newID)
    {
        if (!isLocal)
        {
            Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
        }
        else
        {
            actorID = newID;
        }
    }

    internal void InternalCacheProperties(Hashtable properties)
    {
        if (properties != null && properties.Count != 0 && !customProperties.Equals(properties))
        {
            if (properties.ContainsKey(byte.MaxValue))
            {
                nameField = (string)properties[byte.MaxValue];
            }
            customProperties.MergeStringKeys(properties);
            customProperties.StripKeysWithNullValues();
        }
    }

    public void SetCustomProperties(Hashtable propertiesToSet)
    {
        if (propertiesToSet != null)
        {
            customProperties.MergeStringKeys(propertiesToSet);
            customProperties.StripKeysWithNullValues();
            Hashtable actorProperties = propertiesToSet.StripToStringKeys();
            if (actorID > 0 && !PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.NetworkingPeer.OpSetCustomPropertiesOfActor(actorID, actorProperties, broadcast: true, 0);
            }
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, this, propertiesToSet);
        }
    }

    public static PhotonPlayer Find(int ID)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonPlayer photonPlayer = PhotonNetwork.PlayerList[i];
            if (photonPlayer.ID == ID)
            {
                return photonPlayer;
            }
        }
        return null;
    }

    public PhotonPlayer Get(int id)
    {
        return Find(id);
    }

    public PhotonPlayer GetNext()
    {
        return GetNextFor(ID);
    }

    public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
    {
        if (currentPlayer == null)
        {
            return null;
        }
        return GetNextFor(currentPlayer.ID);
    }

    public PhotonPlayer GetNextFor(int currentPlayerId)
    {
        if (PhotonNetwork.NetworkingPeer == null || PhotonNetwork.NetworkingPeer.mActors == null || PhotonNetwork.NetworkingPeer.mActors.Count < 2)
        {
            return null;
        }
        Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.NetworkingPeer.mActors;
        int num = int.MaxValue;
        int num2 = currentPlayerId;
        foreach (int key in mActors.Keys)
        {
            if (key < num2)
            {
                num2 = key;
            }
            else if (key > currentPlayerId && key < num)
            {
                num = key;
            }
        }
        return (num == int.MaxValue) ? mActors[num2] : mActors[num];
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(PhotonName))
        {
            return string.Format("#{0:00}{1}", ID, (!isMasterClient) ? string.Empty : "(master)");
        }
        return string.Format("'{0}'{1}", PhotonName, (!isMasterClient) ? string.Empty : "(master)");
    }

    public string ToStringFull()
    {
        return $"#{ID:00} '{PhotonName}' {customProperties.ToStringFull()}";
    }
}