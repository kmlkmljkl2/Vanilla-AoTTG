using ExitGames.Client.Photon;
using UnityEngine;

internal static class TeamExtensions
{
	public static PunTeams.Team GetTeam(this PhotonPlayer player)
	{
		if (player.CustomProperties.TryGetValue("team", out var value))
		{
			return (PunTeams.Team)(byte)value;
		}
		return PunTeams.Team.none;
	}

	public static void SetTeam(this PhotonPlayer player, PunTeams.Team team)
	{
		if (!PhotonNetwork.ConnectedAndReady)
		{
			Debug.LogWarning(string.Concat("JoinTeam was called in state: ", PhotonNetwork.ConnectionStateDetailed, ". Not connectedAndReady."));
		}
		PunTeams.Team team2 = PhotonNetwork.Player.GetTeam();
		if (team2 != team)
		{
			PhotonNetwork.Player.SetCustomProperties(new Hashtable { 
			{
				"team",
				(byte)team
			} });
		}
	}
}
