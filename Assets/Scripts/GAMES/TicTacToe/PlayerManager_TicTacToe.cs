using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerManager_TicTacToe : BasePlayerManager {
	[System.NonSerialized]
	public static PlayerManager_TicTacToe Instance;

	public UserManager_TicTacToe DataManager_TicTacToe {
		get { return (UserManager_TicTacToe)DataManager; }
		set { DataManager = value; }
	}

	// main events
	void Start () {
		// keep this object alive
		DontDestroyOnLoad (this.gameObject);
	}

	// main logic
	public override void Init ()
	{
		base.Init ();

		// activate instance
		if (Instance == null) {
			Instance = this;

			if (!didInit)
				Init ();
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	public bool IsInit() {
		return didInit;
	}

	public override void GameFinished ()
	{
		base.GameFinished ();
	}

	public override void GameStart ()
	{
		base.GameStart ();
	}

	public void SetPlayerDefaultData() {
		if (!DataManager_TicTacToe)
			Init ();
		
		DataManager_TicTacToe.GetDefaultData ();
	}

	public void SetPlayerName(string stVal) {
		DataManager_TicTacToe.SetName (stVal);
	}

	public string GetPlayerName() {
		return DataManager_TicTacToe.GetName ();
	}

	public void SetScore(int itVal) {
		DataManager_TicTacToe.SetScore (itVal);
	}

	public int GetScore() {
		return DataManager_TicTacToe.GetScore ();
	}

	public void AddScore(int num) {
		DataManager_TicTacToe.AddScore (num);
	}

	// my methods
	public void ClearPlayerProgress() {
		DataManager_TicTacToe.ClearPlayerProgress ();
	}

	public void AddPlayedLevel() {
		DataManager_TicTacToe.AddPlayedLevel ();
	}

	public int GetPlayedLevel() {
		return DataManager_TicTacToe.GetPlayedLevelCount ();
	}

	public void AddWinLevel(DifficultyStates.DifficultyState difGame) {
		DataManager_TicTacToe.AddWinLevel ((int)difGame);
	}

	public int GetWinLevel(List<DifficultyStates.DifficultyState> difGame) {
		int result = 0;

		int countDifGame = difGame.Count;

		if ((countDifGame > 0)) {
			foreach (DifficultyStates.DifficultyState itemDifGame in difGame) {
				result = result + GetWinLevel (itemDifGame);
			}
		} else {
			result = result + GetWinLevel ();
		}

		return result;
	}

	public int GetWinLevel(DifficultyStates.DifficultyState difGame) {
		return DataManager_TicTacToe.GetWinLevelCount ((int)difGame);
	}

	public int GetWinLevel() {
		return DataManager_TicTacToe.GetWinLevelCount (-1);
	}

	public void SetTuorialState(int numPos, bool boVal) {
		DataManager_TicTacToe.SetTuorialState (numPos, boVal);
	}

	public bool GetTutorialState(int numPos) {
		return DataManager_TicTacToe.GetTutorialState (numPos);
	}

	public DateTime GetFirstEnterDataTime() {
		return DataManager_TicTacToe.GetFirstEnter ();
	}

	public void SetCurrentAsLastEnterDataTime() {
		DateTime last = DataManager_TicTacToe.GetLastEnter ();

		if (DateTime.Now > last) {
			DataManager_TicTacToe.SetCurrentAsLastEnter ();
		}
	}

	public DateTime GetLastEnterDataTime() {
		return DataManager_TicTacToe.GetLastEnter ();
	}

	public bool IsFirstEnter() {
		return GetScore () == 0;
	}

	public string GetLanguage()
	{
		return DataManager_TicTacToe.GetLanguage();
	}

	public void SetLanguage(string val)
	{
		DataManager_TicTacToe.SetLanguage (val);
	}

	//============

	public void LoadPrivateDataPlayer() {
		DataManager_TicTacToe.LoadPrivateDataPlayer ();
	}

	public void SavePrivateDataPlayer() {
		DataManager_TicTacToe.SavePrivateDataPlayer ();
	}

	public void ClosePrivateDataPlayerFile() {
		DataManager_TicTacToe.ClosePlayerDateFile ();
	}
}
