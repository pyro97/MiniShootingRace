using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handle player preferences, saving all data in PlayerPrefs class
public class PlayerPrefsHandler : MonoBehaviour
{

	public const string PLAYER_KEY_S = "playerKey"; //contains the key of the user to check if it is the first time or no.
	public const string MUTE_MUSICS_INT = "muteMusics"; //is set to true to mute music
	public const string MUTE_EFFECTS_INT = "muteEffects";// is set to true to mute effects
	public const string RECORD_INT = "record"; //contains the user record
	public const string NUM_MATCH_INT = "nMatch"; // contains the number of the match that the user has played.
	public const string MONEY_INT = "money"; // contains the collected money during the game
	public const string CHARACTER_S = "character"; // contains the name of the selected character (ball)

	//create user preferences for the first time
	public void CreateFirstTimePref()
	{
		SetMutedEffects(false);
		SetMutedMusic(false);
		SetPlayerKey("Player1");
		SetMoney(0);
		SetRecord(0);
		SetCharacter("Basket Ball");
		SetPlayerByName("Basket Ball");
		SetNumMatch(0);
	}

	//if the player key is empty, so this is the first time 
	public bool IsFirstTime()
	{
		if (GetPlayerKey().Length > 0)
		{
			return false;
		}
		else
		{
			return true;
		}


	}

	public string GetPlayerKey()
	{
		return PlayerPrefs.GetString(PLAYER_KEY_S, "");
	}

	public void SetPlayerKey(string val)
	{
		PlayerPrefs.SetString(PLAYER_KEY_S, val);
	}

	public bool GetPlayerByName(string s)
	{
		string nome = PlayerPrefs.GetString(s);
		if (nome.Equals(s)) return true;
		else return false;
	}

	//set all characters name with a key of the same name. It is used to check if a character has already been purchased
	public void SetPlayerByName(string s)
	{
		PlayerPrefs.SetString(s, s);
	}

	public int GetMoney()
	{
		return PlayerPrefs.GetInt(MONEY_INT);
	}

	public void SetMoney(int val)
	{
		PlayerPrefs.SetInt(MONEY_INT, val);
	}

	public int GetRecord()
	{
		return PlayerPrefs.GetInt(RECORD_INT);
	}

	public void SetRecord(int val)
	{
		PlayerPrefs.SetInt(RECORD_INT, val);
	}

	public int GetNumMatch()
	{
		return PlayerPrefs.GetInt(NUM_MATCH_INT);
	}

	public void SetNumMatch(int val)
	{
		PlayerPrefs.SetInt(NUM_MATCH_INT, val);
	}

	public void SetMutedEffects(bool muted)
	{
		PlayerPrefs.SetInt(MUTE_EFFECTS_INT, muted ? 1 : 0);

		// Pausing the AudioListener will disable all sounds.
		//AudioListener.pause = muted;
	}

	public bool GetIsMutedMusic()
	{
		// If the value of the MUTE_INT key is 1 then sound is muted, otherwise it is not muted.
		// The default value of the MUTE_INT key is 0 (i.e. not muted).
		return PlayerPrefs.GetInt(MUTE_MUSICS_INT, 0) == 1;
	}

	public void SetMutedMusic(bool muted)
	{
		PlayerPrefs.SetInt(MUTE_MUSICS_INT, muted ? 1 : 0);

		// Pausing the AudioListener will disable all sounds.
		//AudioListener.pause = muted;
	}

	public bool GetIsMutedEffects()
	{
		// If the value of the MUTE_INT key is 1 then sound is muted, otherwise it is not muted.
		// The default value of the MUTE_INT key is 0 (i.e. not muted).
		return PlayerPrefs.GetInt(MUTE_EFFECTS_INT, 0) == 1;
	}

	public string GetCharacter()
	{
		return PlayerPrefs.GetString(CHARACTER_S);
	}

	public void SetCharacter(string val)
	{
		PlayerPrefs.SetString(CHARACTER_S, val);
	}

}
