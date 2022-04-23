using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhotonPlayer
{
    private int ActorID = -1;

    private string NameField = string.Empty;

    public readonly bool IsLocal;

    public object TagObject;

    public bool Muted;
    public int ID => ActorID;

    public bool IsDead
    {
        get
        {
            if (CustomProperties.ContainsKey(PhotonPlayerProperty.dead) && CustomProperties[PhotonPlayerProperty.dead] is bool State)
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
            if (CustomProperties.ContainsKey(PhotonPlayerProperty.kills) && CustomProperties[PhotonPlayerProperty.kills] is int Kills)
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
            if (CustomProperties.ContainsKey(PhotonPlayerProperty.deaths) && CustomProperties[PhotonPlayerProperty.deaths] is int Deaths)
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
            if (CustomProperties.ContainsKey(PhotonPlayerProperty.max_dmg) && CustomProperties[PhotonPlayerProperty.max_dmg] is int HighestDmg)
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
            if(CustomProperties.ContainsKey(PhotonPlayerProperty.total_dmg) &&CustomProperties[PhotonPlayerProperty.total_dmg] is int TotalDmg)
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
            if(CustomProperties.ContainsKey(PhotonPlayerProperty.name) && CustomProperties[PhotonPlayerProperty.name] is string name && name.RemoveHex().Trim() != "")
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
            return CustomProperties.ContainsKey(PhotonPlayerProperty.isTitan) && CustomProperties[PhotonPlayerProperty.isTitan] is int value && value == 2;
        }
    }
    public bool IsAhss
    {
        get
        {
            return CustomProperties.ContainsKey(PhotonPlayerProperty.isTitan) && CustomProperties[PhotonPlayerProperty.isTitan] is int value && value == 3;
        }
    }

    public int Team
    {
        get
        {
            if( CustomProperties.ContainsKey(PhotonPlayerProperty.team) && CustomProperties[PhotonPlayerProperty.team] is int TeamId)
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
            return NameField;
        }
        set
        {
            if (!IsLocal)
            {
                Debug.LogError("Error: Cannot change the name of a remote player!");
            }
            else
            {
                NameField = value;
            }
        }
    }

    public bool IsMasterClient => PhotonNetwork.NetworkingPeer.mMasterClient == this;

    public Hashtable CustomProperties { get; private set; }

    public Hashtable AllProperties
    {
        get
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Merge(CustomProperties);
            hashtable[byte.MaxValue] = PhotonName;
            return hashtable;
        }
    }

    public PhotonPlayer(bool isLocal, int actorID, string name)
    {
        CustomProperties = new Hashtable();
        this.IsLocal = isLocal;
        this.ActorID = actorID;
        NameField = name;
    }

    protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
    {
        CustomProperties = new Hashtable();
        this.IsLocal = isLocal;
        this.ActorID = actorID;
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
            if (IsLocal)
            {
                text += "> ";
            }
            if (IsMasterClient)
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
        if (!IsLocal)
        {
            Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
        }
        else
        {
            ActorID = newID;
        }
    }

    internal void InternalCacheProperties(Hashtable properties)
    {
        if (properties != null && properties.Count != 0 && !CustomProperties.Equals(properties))
        {
            if (properties.ContainsKey(byte.MaxValue))
            {
                NameField = (string)properties[byte.MaxValue];
            }
            CustomProperties.MergeStringKeys(properties);
            CustomProperties.StripKeysWithNullValues();
        }
    }

    public void SetCustomProperties(Hashtable propertiesToSet)
    {
        if (propertiesToSet != null)
        {
            CustomProperties.MergeStringKeys(propertiesToSet);
            CustomProperties.StripKeysWithNullValues();
            Hashtable actorProperties = propertiesToSet.StripToStringKeys();
            if (ActorID > 0 && !PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.NetworkingPeer.OpSetCustomPropertiesOfActor(ActorID, actorProperties, broadcast: true, 0);
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
            return string.Format("#{0:00}{1}", ID, (!IsMasterClient) ? string.Empty : "(master)");
        }
        return string.Format("'{0}'{1}", PhotonName, (!IsMasterClient) ? string.Empty : "(master)");
    }

    public string ToStringFull()
    {
        return $"#{ID:00} '{PhotonName}' {CustomProperties.ToStringFull()}";
    }
}