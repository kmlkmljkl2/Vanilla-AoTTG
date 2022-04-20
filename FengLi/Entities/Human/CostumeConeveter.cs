using ExitGames.Client.Photon;
using UnityEngine;

public class CostumeConeveter
{
	public static HeroCostume PhotonDataToHeroCostume(PhotonPlayer player)
	{
		HeroCostume heroCostume = new HeroCostume();
		heroCostume = new HeroCostume();
		heroCostume.sex = IntToSex((int)player.customProperties[PhotonPlayerProperty.sex]);
		heroCostume.costumeId = (int)player.customProperties[PhotonPlayerProperty.costumeId];
		heroCostume.id = (int)player.customProperties[PhotonPlayerProperty.heroCostumeId];
		heroCostume.cape = (bool)player.customProperties[PhotonPlayerProperty.cape];
		heroCostume.hairInfo = ((heroCostume.sex != 0) ? CostumeHair.hairsF[(int)player.customProperties[PhotonPlayerProperty.hairInfo]] : CostumeHair.hairsM[(int)player.customProperties[PhotonPlayerProperty.hairInfo]]);
		heroCostume.eye_texture_id = (int)player.customProperties[PhotonPlayerProperty.eye_texture_id];
		heroCostume.beard_texture_id = (int)player.customProperties[PhotonPlayerProperty.beard_texture_id];
		heroCostume.glass_texture_id = (int)player.customProperties[PhotonPlayerProperty.glass_texture_id];
		heroCostume.skin_color = (int)player.customProperties[PhotonPlayerProperty.skin_color];
		heroCostume.hair_color = new Color((float)player.customProperties[PhotonPlayerProperty.hair_color1], (float)player.customProperties[PhotonPlayerProperty.hair_color2], (float)player.customProperties[PhotonPlayerProperty.hair_color3]);
		heroCostume.division = IntToDivision((int)player.customProperties[PhotonPlayerProperty.division]);
		heroCostume.stat = new HeroStat();
		heroCostume.stat.SPD = (int)player.customProperties[PhotonPlayerProperty.statSPD];
		heroCostume.stat.GAS = (int)player.customProperties[PhotonPlayerProperty.statGAS];
		heroCostume.stat.BLA = (int)player.customProperties[PhotonPlayerProperty.statBLA];
		heroCostume.stat.ACL = (int)player.customProperties[PhotonPlayerProperty.statACL];
		heroCostume.stat.skillId = (string)player.customProperties[PhotonPlayerProperty.statSKILL];
		heroCostume.setBodyByCostumeId();
		heroCostume.setMesh();
		heroCostume.setTexture();
		return heroCostume;
	}

	public static void HeroCostumeToPhotonData(HeroCostume costume, PhotonPlayer player)
	{
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.sex,
			SexToInt(costume.sex)
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.costumeId,
			costume.costumeId
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.heroCostumeId,
			costume.id
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.cape,
			costume.cape
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.hairInfo,
			costume.hairInfo.id
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.eye_texture_id,
			costume.eye_texture_id
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.beard_texture_id,
			costume.beard_texture_id
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.glass_texture_id,
			costume.glass_texture_id
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.skin_color,
			costume.skin_color
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.hair_color1,
			costume.hair_color.r
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.hair_color2,
			costume.hair_color.g
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.hair_color3,
			costume.hair_color.b
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.division,
			DivisionToInt(costume.division)
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.statSPD,
			costume.stat.SPD
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.statGAS,
			costume.stat.GAS
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.statBLA,
			costume.stat.BLA
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.statACL,
			costume.stat.ACL
		} });
		player.SetCustomProperties(new Hashtable { 
		{
			PhotonPlayerProperty.statSKILL,
			costume.stat.skillId
		} });
	}

	public static void HeroCostumeToLocalData(HeroCostume costume, string slot)
	{
		slot = slot.ToUpper();
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.sex, SexToInt(costume.sex));
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.costumeId, costume.costumeId);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.heroCostumeId, costume.id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.cape, costume.cape ? 1 : 0);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.hairInfo, costume.hairInfo.id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.eye_texture_id, costume.eye_texture_id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.beard_texture_id, costume.beard_texture_id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.glass_texture_id, costume.glass_texture_id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.skin_color, costume.skin_color);
		PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.hair_color1, costume.hair_color.r);
		PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.hair_color2, costume.hair_color.g);
		PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.hair_color3, costume.hair_color.b);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.division, DivisionToInt(costume.division));
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.statSPD, costume.stat.SPD);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.statGAS, costume.stat.GAS);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.statBLA, costume.stat.BLA);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.statACL, costume.stat.ACL);
		PlayerPrefs.SetString(slot + PhotonPlayerProperty.statSKILL, costume.stat.skillId);
	}

	public static HeroCostume LocalDataToHeroCostume(string slot)
	{
		slot = slot.ToUpper();
		if (!PlayerPrefs.HasKey(slot + PhotonPlayerProperty.sex))
		{
			return HeroCostume.costume[0];
		}
		HeroCostume heroCostume = new HeroCostume();
		heroCostume = new HeroCostume();
		heroCostume.sex = IntToSex(PlayerPrefs.GetInt(slot + PhotonPlayerProperty.sex));
		heroCostume.id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.heroCostumeId);
		heroCostume.costumeId = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.costumeId);
		heroCostume.cape = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.cape) == 1;
		heroCostume.hairInfo = ((heroCostume.sex != 0) ? CostumeHair.hairsF[PlayerPrefs.GetInt(slot + PhotonPlayerProperty.hairInfo)] : CostumeHair.hairsM[PlayerPrefs.GetInt(slot + PhotonPlayerProperty.hairInfo)]);
		heroCostume.eye_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.eye_texture_id);
		heroCostume.beard_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.beard_texture_id);
		heroCostume.glass_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.glass_texture_id);
		heroCostume.skin_color = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.skin_color);
		heroCostume.hair_color = new Color(PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.hair_color1), PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.hair_color2), PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.hair_color3));
		heroCostume.division = IntToDivision(PlayerPrefs.GetInt(slot + PhotonPlayerProperty.division));
		heroCostume.stat = new HeroStat();
		heroCostume.stat.SPD = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.statSPD);
		heroCostume.stat.GAS = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.statGAS);
		heroCostume.stat.BLA = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.statBLA);
		heroCostume.stat.ACL = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.statACL);
		heroCostume.stat.skillId = PlayerPrefs.GetString(slot + PhotonPlayerProperty.statSKILL);
		heroCostume.setBodyByCostumeId();
		heroCostume.setMesh();
		heroCostume.setTexture();
		return heroCostume;
	}

	private static DIVISION IntToDivision(int id)
	{
		return id switch
		{
			0 => DIVISION.TheGarrison, 
			1 => DIVISION.TheMilitaryPolice, 
			2 => DIVISION.TheSurveryCorps, 
			3 => DIVISION.TraineesSquad, 
			_ => DIVISION.TheSurveryCorps, 
		};
	}

	private static int DivisionToInt(DIVISION id)
	{
		return id switch
		{
			DIVISION.TheGarrison => 0, 
			DIVISION.TheMilitaryPolice => 1, 
			DIVISION.TheSurveryCorps => 2, 
			DIVISION.TraineesSquad => 3, 
			_ => 2, 
		};
	}

	private static SEX IntToSex(int id)
	{
		return id switch
		{
			0 => SEX.FEMALE, 
			1 => SEX.MALE, 
			_ => SEX.MALE, 
		};
	}

	private static int SexToInt(SEX id)
	{
		return id switch
		{
			SEX.FEMALE => 0, 
			SEX.MALE => 1, 
			_ => 1, 
		};
	}

	private static UNIFORM_TYPE IntToUniformType(int id)
	{
		return id switch
		{
			0 => UNIFORM_TYPE.CasualA, 
			1 => UNIFORM_TYPE.CasualB, 
			2 => UNIFORM_TYPE.UniformA, 
			3 => UNIFORM_TYPE.UniformB, 
			4 => UNIFORM_TYPE.CasualAHSS, 
			_ => UNIFORM_TYPE.UniformA, 
		};
	}

	private static int UniformTypeToInt(UNIFORM_TYPE id)
	{
		return id switch
		{
			UNIFORM_TYPE.CasualA => 0, 
			UNIFORM_TYPE.CasualB => 1, 
			UNIFORM_TYPE.UniformA => 2, 
			UNIFORM_TYPE.UniformB => 3, 
			UNIFORM_TYPE.CasualAHSS => 4, 
			_ => 2, 
		};
	}
}
