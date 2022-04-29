using System.Collections.Generic;
using Photon;
using UnityEngine;

public partial class InRoomChat : Photon.MonoBehaviour
{
	public static Rect GuiRect = new Rect(0f, 100f, 300f, 470f);

	public static Rect GuiRect2 = new Rect(30f, 575f, 300f, 25f);

	public bool IsVisible = true;

	private bool AlignBottom = true;

	public static List<string> messages = new List<string>();

	private string inputLine = string.Empty;

	private Vector2 scrollPos = Vector2.zero;

	public static readonly string ChatRPC = "Chat";

	public void Start()
	{
		setPosition();
		FengGameManagerMKII.InroomChat = this;
	}

	public void setPosition()
	{
		if (AlignBottom)
		{
			GuiRect = new Rect(0f, Screen.height - 500, 300f, 470f);
			GuiRect2 = new Rect(30f, Screen.height - 300 + 275, 300f, 25f);
		}
	}

	public void OnGUI()
	{
		if (!IsVisible || PhotonNetwork.ConnectionStateDetailed != PeerState.Joined)
		{
			return;
		}
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(inputLine))
			{
				if (inputLine == "\t")
				{
					inputLine = string.Empty;
					GUI.FocusControl(string.Empty);
					return;
				}
				if(inputLine.StartsWith("/"))
                {
					HandleCommand(inputLine.Remove(0, 1).Split(' '));
					inputLine = string.Empty;
					GUI.FocusControl(string.Empty);
					return;
                }
				//True Gem
				//if (inputLine.Length > 7 && inputLine.Substring(0, 7) == "/kick #")
				//{
				//	if (inputLine.Remove(0, 7) == PhotonNetwork.MasterClient.ID.ToString())
				//	{
				//		AddLine("error:can't kick master client.");
				//	}
				//	else if (inputLine.Remove(0, 7) == PhotonNetwork.Player.ID.ToString())
				//	{
				//		AddLine("error:can't kick yourself.");
				//	}
				//	else
				//	{
				//		bool flag = false;
				//		PhotonPlayer[] playerList = PhotonNetwork.PlayerList;
				//		foreach (PhotonPlayer photonPlayer in playerList)
				//		{
				//			if (photonPlayer.ID.ToString() == inputLine.Remove(0, 7))
				//			{
				//				flag = true;
				//				break;
				//			}
				//		}
				//		if (!flag)
				//		{
				//			AddLine("error:no such player.");
				//		}
				//		else
				//		{
				//			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("Chat", PhotonTargets.All, inputLine, LoginFengKAI.player.name);
				//		}
				//	}
				//}
				//else
				//{
				//	GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("Chat", PhotonTargets.All, inputLine, LoginFengKAI.player.name);
				//}
				FengGameManagerMKII.Instance.photonView.RPC("Chat", PhotonTargets.All, inputLine, LoginFengKAI.player.name);

				inputLine = string.Empty;
				GUI.FocusControl(string.Empty);
				return;
			}
			inputLine = "\t";
			GUI.FocusControl("ChatInput");
		}
		GUI.SetNextControlName(string.Empty);
		GUILayout.BeginArea(GuiRect);
		GUILayout.FlexibleSpace();
		string text = string.Empty;
		if (messages.Count < 10)
		{
			for (int j = 0; j < messages.Count; j++)
			{
				text = text + messages[j] + "\n";
			}
		}
		else
		{
			for (int k = messages.Count - 10; k < messages.Count; k++)
			{
				text = text + messages[k] + "\n";
			}
		}
		GUILayout.Label(text);
		GUILayout.EndArea();
		GUILayout.BeginArea(GuiRect2);
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("ChatInput");
		inputLine = GUILayout.TextField(inputLine);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	public void AddLine(string newLine)
	{
		messages.Add(newLine);
	}
}
