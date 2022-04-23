using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class InRoomChat
{
    public void AddError(string str)
    {
        AddLine($"<color=red>ERROR</color> => {str}");
    }
    public void HandleCommand(string[] Command)
    {

        switch(Command[0].ToLower())
        {
            case "restart":
                if (!PhotonNetwork.IsMasterClient)
                {
                    AddError("Not Masterclient, cannot restart");
                    break;
                }
                FengGameManagerMKII.Instance.restartGame();
                break;
            case "kick":
                if (!PhotonNetwork.IsMasterClient || Command.Length <= 1 || !int.TryParse(Command[1], out int Id) || !PhotonNetwork.NetworkingPeer.mActors.ContainsKey(Id))
                {
                    AddError("Not Masterclient or Id did not exist!");
                    break;
                }
                PhotonNetwork.CloseConnection(PhotonNetwork.NetworkingPeer.mActors[Id]);
                break;


        }
    }


}

