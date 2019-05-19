using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UserManager_TicTacToe : BaseUserManager {
	[Header("Settings TicTacToe")]
	[SerializeField]
	private int difGameCount = 3;

	private int countPlayedLevel;
	private int[] countWinLevel;

	private bool[] tutorialVievList;

	[SerializeField]
	private DateTime firstEnterInGame;
	[SerializeField]
	private DateTime lastEnterInGame;

	[SerializeField]
	private string language = "EN";

	// main logic
	private void ClearScore() {
		SetScore (0);
	}

	private void ClearCountPlayedLevel() {
		countPlayedLevel = 0;
	}

	private void ClearCountWinLevel() {
		for (int i = 0; i < difGameCount; i++) {
			countWinLevel [i] = 0;
		}
	}

	public override void GetDefaultData()
	{
		base.GetDefaultData ();

		ClearCountPlayedLevel ();

		countWinLevel = new int[difGameCount];
		ClearCountWinLevel ();

		language = "EN";
	}

	//==================

	public void ClearPlayerProgress() {
		ClearScore ();

		ClearCountPlayedLevel ();
		ClearCountWinLevel ();
	}

	public int GetPlayedLevelCount() {
		return countPlayedLevel;
	}

	public void SetPlayedLevelCount(int count) {
		countPlayedLevel = count;
	}

	public void AddPlayedLevel() {
		countPlayedLevel += 1;
	}

	public int GetWinLevelCount(int difGame = -1) {
		int result = 0;

		for (int i = 0; i < difGameCount; i++) {
			if (
				(difGame == i) || (difGame == -1)
			) {
				result = result + countWinLevel [i];
			}
		}

		return result;
	}

	public void SetWinLevelCount(int difGame, int count) {
		countWinLevel[difGame] = count;
	}

	public void AddWinLevel(int difGame) {
		countWinLevel[difGame] = countWinLevel[difGame] + 1;
	}

	private void AddTutorialSize() {
		bool[] tutorialVievListNew = new bool[tutorialVievList.Length + 1];

		for (int i = 0; i < tutorialVievList.Length; i++) {
			tutorialVievListNew [i] = tutorialVievList [i];
		}

		tutorialVievList = tutorialVievListNew;
	}

	public void SetTuorialState(int numPos, bool boVal) {
		if (numPos < tutorialVievList.Length) {
			tutorialVievList [numPos] = boVal;
		} else {
			AddTutorialSize ();
			SetTuorialState (numPos, boVal);
		}
	}

	public bool GetTutorialState(int numPos) {
		if (numPos < tutorialVievList.Length) {
			return tutorialVievList [numPos];
		} else {
			return false;
		}
	}

	public void SetCurrentAsFirsEnter() {
		firstEnterInGame = DateTime.Now;
	}

	public DateTime GetFirstEnter() {
		return firstEnterInGame;
	}

	public void SetCurrentAsLastEnter() {
		lastEnterInGame = DateTime.Now;
	}

	public DateTime GetLastEnter() {
		return lastEnterInGame;
	}

	public string GetLanguage()
	{
		return language;
	}

	public void SetLanguage(string val)
	{
		language = val;
	}

	//=for save data=====================
	private FileStream filePlayerData;

	private void OpenPlayerDataFileForWrite() {
		filePlayerData = File.Create(Application.persistentDataPath + "/playerinfo.dat");
	}

	private void OpenPlayerDataFileForRead() {
		filePlayerData = File.Open (Application.persistentDataPath + "/playerinfo.dat", FileMode.Open);
	}

	public void ClosePlayerDateFile() {
		filePlayerData.Close ();
	}

	/// <summary>
	/// save player data in file with cripting, not use for Web-application (we can't write file)
	/// </summary>
	public void SavePrivateDataPlayer()
	{
		BinaryFormatter bf = new BinaryFormatter();
		OpenPlayerDataFileForWrite ();

		PlayerData_TicTacToe data = new PlayerData_TicTacToe();
		data.playerName = playerName;

		data.score = GetScore();

		data.countPlayedLevel = countPlayedLevel;

		data.countWinLevel = countWinLevel;

		if (data.firstEnterInGame != firstEnterInGame) {
			data.firstEnterInGame = firstEnterInGame;
		}
		if (data.lastEnterInGame != lastEnterInGame) {
			data.lastEnterInGame = lastEnterInGame;
		}

		data.language = language;

		bf.Serialize(filePlayerData, data);
		ClosePlayerDateFile();
	}

	/// <summary>
	/// restore player data from cripting file.
	/// </summary>
	public void LoadPrivateDataPlayer()
	{
		if (File.Exists (Application.persistentDataPath + "/playerinfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			OpenPlayerDataFileForRead ();

			PlayerData_TicTacToe data = (PlayerData_TicTacToe)bf.Deserialize (filePlayerData);
			playerName = data.playerName;

			SetScore (data.score);

			if (data != null) {
				countPlayedLevel = data.countPlayedLevel;
			}

			if (data != null) {
				countWinLevel = data.countWinLevel;
			}

			if (firstEnterInGame != data.firstEnterInGame) {
				firstEnterInGame = data.firstEnterInGame;
			}
			if (lastEnterInGame != data.lastEnterInGame) {
				lastEnterInGame = data.lastEnterInGame;
			}

			language = data.language;

			ClosePlayerDateFile ();
		} else {
			GetDefaultData ();

			SetCurrentAsFirsEnter ();
			SetCurrentAsLastEnter ();
		}
	}
}

[Serializable]
class PlayerData_TicTacToe
{
	public string playerName;

	public int score;

	public int countPlayedLevel;
	public int[] countWinLevel;

	public DateTime firstEnterInGame;
	public DateTime lastEnterInGame;

	public string language;
}