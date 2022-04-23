using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class InRoomChat
{
    public void HandleCommand(string[] Command)
    {
        switch(Command[0].ToLower().Remove(0,1))
        {
            case "restart":
                if (!PhotonNetwork.IsMasterClient)
                {
                    //Logs
                    break;
                }
                FengGameManagerMKII.Instance.restartGame();
                this.addLINE("test");
               // GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().restartGame();
                break;
            case "kick":
                if(!PhotonNetwork.IsMasterClient || Command.Length <= 1)
                {
                    //Log
                    break;
                    }
                break;


        }
    }


}

