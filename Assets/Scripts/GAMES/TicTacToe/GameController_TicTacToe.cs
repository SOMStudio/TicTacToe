using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DifficultyStates;

public class GameController_TicTacToe : BaseGameController {
	public bool initGamePool = false;

	[Header("Play States")]
	public bool startLevel = false;
	public bool prepearingGamePool = false;
	public bool clearingLevel = false;
	public bool activateDataLevel = false;
	public bool fillingLevel = false;
	public bool pauseLevel = false;
	public bool completeLevel = false;

	[Header("Develop States")]
	public bool developState = false;

	[Header("Timer")]
	public float secUpdateLevelTimer = 1;

	[Header("Game")]
	public bool playerStep = true;

	public int countPointInRow = 3;
	public int countPointInCol = 3;

	public int selNumPoint = -1;
	public int selNumGlobalRow = -1;
	public int selNumGlobalCol = -1;
	public string selNumber = "-1";
	public bool lockNumber = false;
	public string signPlayer = "X";

	public Color colorHighlight;

	[Header("Help Value")]
	public int scoreLevel = 0;
	public int playedLevel = 0;
	public int winLevel = 0;
	public int loseLevel = 0;
	public bool isPlayerLose = false;
	public bool isPlayerWin = false;

	public DifficultyState difficultyState = 0;

	[Header("User Settings")]
	public bool useCoroutineMain = true;

	[Header("Error St")]
	public string errorString = "";

	//saveLeveldate
	private bool firstPlayerStep_save = true;
	private int isLavelComplete_save = -1;
	private int isPlayerWin_save = -1;
	private int bonusForPlayer_save = -1;

	[System.NonSerialized]
	public static GameController_TicTacToe Instance;

	public SceneManager_TicTacToe SceneGameManager;
	public PlayerManager_TicTacToe PlayerManager;
	public PlayFieldManager_TicTacToe PlayFieldManager;
	public GameMenuController_TicTacToe GameMenu;
	public BaseSoundController SoundManager;

	private void Init() {
		// activate instance
		if (Instance == null) {
			Instance = this;

			GameMenu = GameMenuController_TicTacToe.Instance;
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	public SceneManager_TicTacToe GetSceneManager() {
		return SceneGameManager;
	}

	public PlayerManager_TicTacToe GetPlayerManager() {
		return PlayerManager;
	}

	public PlayFieldManager_TicTacToe GetPlayFieldManager() {
		return PlayFieldManager;
	}

	public void SetPlayFieldManager(PlayFieldManager_TicTacToe val) {
		PlayFieldManager = val;
	}

	public GameMenuController_TicTacToe GetMenuManager() {
		return GameMenu;
	}

	public void SetMenuManager(GameMenuController_TicTacToe val) {
		GameMenu = val;
	}

	public BaseSoundController GetSoundManager() {
		return SoundManager;
	}

	public override void StartGame () {
		// keep this object alive
		DontDestroyOnLoad (this.gameObject);

		//init player data
		LoadDataPlayer ();

		//main menu
		GameMenu.ShowWindowStartGame ();

		//update system time in game
		InvokeRepeating ("UpdateSystemTime", 5, 20);
	}

	public void InitGamePool() {
		if (!initGamePool) {
			initGamePool = true;

			//update UI
			HeadMenuPlayerBonusUpdate ();
			HeadMenuPlayerPlayedGameUpdate ();

			Debug.Log ("- Game Pool Init");
		}
	}

	private void StopActiveLevel() {
		//save res in PlayerManager
		if (scoreLevel > 0) {
			//add score in data
			PlayerManager.AddScore (scoreLevel);

			if (isPlayerWin) {
				//add count complete level
				AddCountCompleteLevel (difficultyState);
			}
		}

		//save Player Progress
		if (scoreLevel > 0) {
			SaveDataProgress ();
		}
	}

	// player
	public string GetUserDataPath() {
		string pathFile = "";

		switch (Application.platform) {
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.Android:
		case RuntimePlatform.IPhonePlayer:
			pathFile = Application.persistentDataPath;

			break;
		default:
			break;
		}

		return pathFile;
	}

	private void SaveDataProgress() {
		//save data progress
		SaveDataPlayer ();
	}

	public void SaveDataBeforeExit() {
		PlayerManager.SetCurrentAsLastEnterDataTime ();

		//save data progress
		SaveDataPlayerInExitGame ();
	}

	private void ClearDataProgress() {
		PlayerManager.ClearPlayerProgress ();

		// update bonus in game window
		HeadMenuPlayerBonusUpdate ();
	}

	public void ActivateMenuParametrs() {
		//set params for GameMenu
		if (PlayerManager.didInit) {
			GameMenu.SetLanguagePlayer (GetIntLocalization (GetLanguagePlayer ()));
			GameMenu.SetNamePlayer (PlayerManager.GetPlayerName ());
		}
	}

	private void LoadDataPlayer () {
		string userDataPath = GetUserDataPath ();

		PlayerManager.SetPlayerDefaultData ();

		if (userDataPath != "") {
			try {
				PlayerManager.LoadPrivateDataPlayer ();
			} catch {
				PlayerManager.ClosePrivateDataPlayerFile ();

				errorString = "Error load PlayerData.";
				Debug.Log ("Error load PlayerData.");

				WindowConsoleSmolShowError (errorString);
			}
		} else {
			Debug.Log ("Don't have access to file system.");
		}

		//get active level from save
		int countScore = PlayerManager.GetScore ();

		//chack for first enter
		if (PlayerManager.GetPlayerName () == "Anonim"
			&& countScore == 0)
		{
			//firs input or without progress
			InitLanguage ();
		}

		//set params for GameMenu
		ActivateMenuParametrs ();
	}

	private void AddCountCompleteLevel(DifficultyState difLevel) {
		PlayerManager.AddCompleteLevel (difficultyState);
	}

	private void SaveDataPlayer () {
		string userDataPath = GetUserDataPath ();

		if (userDataPath != "") {
			try {
				if (scoreLevel > 0) {
					PlayerManager.SavePrivateDataPlayer ();
				}
			} catch {
				PlayerManager.ClosePrivateDataPlayerFile ();

				errorString = "Error save PlayerData.";
				Debug.Log ("Error save PlayerData.");

				WindowConsoleSmolShowError (errorString);
			}
		} else {
			Debug.Log ("Don't have access to file system.");
		}
	}

	private void SaveDataPlayerInExitGame () {
		string userDataPath = GetUserDataPath ();

		if (userDataPath != "") {
			try {
				PlayerManager.SavePrivateDataPlayer ();
			} catch {
				PlayerManager.ClosePrivateDataPlayerFile ();

				errorString = "Error save PlayerData.";
				Debug.Log ("Error save PlayerData.");

				WindowConsoleSmolShowError (errorString);
			}
		} else {
			Debug.Log ("Don't have access to file system.");
		}
	}

	public string GetPlayerInfo () {
		string res = "[n][n][t][c=green]Player Information:[c]";

		res = res + "[n] - Name" + " = " + PlayerManager.GetPlayerName ();
		res = res + "[n] - Score" + " = " + PlayerManager.GetScore ();
		res = res + "[n] - Language" + " = " + PlayerManager.GetLanguage ();

		return res;
	}

	public string GetCompleteLevelCountInfo () {
		string res = "[n][n][t][c=green]Win Level Count Info [c]";

		int difGameCount = 3;

		for (int j = 0; j < difGameCount; j++) {
				res = res + "[n]";

				DifficultyState carDifGame = (DifficultyState)j;

				int intResult = PlayerManager.GetCompleteLevel (carDifGame);

				string stResult = " - " + carDifGame.ToString () + " = " + intResult;

				res = res + stResult;
		}

		res = res + "[n] * cout win Level = " + PlayerManager.GetCompleteLevel ();

		return res;
	}

	//Scene
	private void GoInGame() {
		SceneGameManager.LoadLevel ("SceneMain");
	}

	private void GoInMenu() {
		initGamePool = false;

		SceneGameManager.LoadLevel ("SceneMenu");
	}

	// language part
	private void InitLanguage () {
		switch (Application.systemLanguage) {
		case SystemLanguage.Russian: SetLanguagePlayer("RU"); break;
		case SystemLanguage.Ukrainian: SetLanguagePlayer("UK"); break;
		default: SetLanguagePlayer("EN"); break;
		}
	}

	public string GetStLocalization(int val) {
		string stRes = "EN";

		switch (val) {
		case 0:
			stRes = "EN";
			break;
		case 1:
			stRes = "UK";
			break;
		case 2:
			stRes = "RU";
			break;
		default:
			break;
		}

		return stRes;
	}

	public int GetIntLocalization(string val) {
		int stRes = 0;

		switch (val) {
		case "EN":
			stRes = 0;
			break;
		case "UK":
			stRes = 1;
			break;
		case "RU":
			stRes = 2;
			break;
		default:
			break;
		}

		return stRes;
	}

	public void SetLanguagePlayer(string val) {
		PlayerManager.SetLanguage (val);
	}

	public string GetLanguagePlayer() {
		return PlayerManager.GetLanguage ();
	}

	// Sound
	public void UpdateSoundVolume() {
		if (SoundManager == null)
			SoundManager = BaseSoundController.Instance;

		SoundManager.UpdateVolume ();
	}

	public void PlaySoundByIndex(int indexNumber) {
		if (SoundManager == null) {
			UpdateSoundVolume ();
		}

		if (SoundManager) {
			SoundManager.PlaySoundByIndex (indexNumber, Vector3.zero);
		}
	}

	public void PlayButtonSound() {
		if (SoundManager == null) {
			UpdateSoundVolume ();
		}

		if (SoundManager) {
			SoundManager.PlaySoundByIndex (0, Vector3.zero);
		}
	}

	//Music
	public void UpdateMusicVolume() {
		BaseMusicController.Instance.UpdateValume ();
	}

	//Menu
	private bool IsLevelComplete() {
		return !isPlayerLose;
	}

	private bool IsPlayerWin() {
		if (isPlayerWin) {
			return true;
		} else {
			return false;
		}
	}

	private int BonusForPlayer() {
		int res = 0;

		res += isLavelComplete_save = (IsLevelComplete() ? 1 : 0);

		res += isPlayerWin_save = (IsPlayerWin() ? 1 : 0);

		return bonusForPlayer_save = res;
	}

	private void MenuInfoTextWin() {
		if (PlayFieldManager) {
			string textRes = "";

			if (isPlayerWin) {
				textRes = "[c=green]You Win :-)[c]";
			} else if (isPlayerLose) {
				textRes = "[c=red]You Lose :-([c]";
			} else {
				textRes = "No one Win :-o";
			}

			PlayFieldManager.WindowInformSetText_1 (textRes);
		}
	}

	private void MenuInfoLevelCompleteUpdate() {
		if (PlayFieldManager) {
			PlayFieldManager.WindowInformSetText_2 (isLavelComplete_save.ToString ());
		}

		//play sound DropCoin
		if (isLavelComplete_save > 0) {
			PlaySoundByIndex (6);
		}
	}

	private void MenuInfoPlayerWinUpdate() {
		if (PlayFieldManager) {
			PlayFieldManager.WindowInformSetText_3 (isPlayerWin_save.ToString ());
		}

		//play sound DropCoin
		if (isPlayerWin_save > 0) {
			PlaySoundByIndex (6);
		}
	}

	private void MenuInfoResultUpdate() {
		if (PlayFieldManager) {
			PlayFieldManager.WindowInformSetText (bonusForPlayer_save.ToString (), 3);
		}

		//play sound DropCoins
		if (bonusForPlayer_save > 0) {
			PlaySoundByIndex (7);
		}
	}

	private void HeadMenuPlayerBonusUpdate() {
		if (PlayFieldManager) {
			PlayFieldManager.SetBonusPlayer (PlayerManager.GetScore ().ToString ());
		}
	}

	private void HeadMenuPlayerPlayedGameUpdate() {
		if (PlayFieldManager) {
			PlayFieldManager.SetPlayedGame (playedLevel.ToString ());
			PlayFieldManager.SetWinGame (winLevel.ToString ());
			PlayFieldManager.SetLoseGame (loseLevel.ToString ());
		}
	}

	public void HeadMenuPlayerPanelUpdateColor() {
		if (PlayFieldManager) {
			if (playerStep) {
				PlayFieldManager.SetPanelPlayerColor (colorHighlight);
				PlayFieldManager.SetPanelAIDefColor ();
			} else {
				PlayFieldManager.SetPanelPlayerDefColor ();
				PlayFieldManager.SetPanelAIColor (colorHighlight);
			}
		}
	}

	private void MenuInformSetText(bool withDelay = false) {
		if (PlayFieldManager) {
			if (withDelay) {
				//text win
				MenuInfoTextWin ();

				//completeed level
				Invoke ("MenuInfoLevelCompleteUpdate", 1);

				//best time
				Invoke ("MenuInfoPlayerWinUpdate", 1.2f);

				//result
				Invoke ("MenuInfoResultUpdate", 1.5f);

				//head menu
				HeadMenuPlayerPlayedGameUpdate ();
				Invoke ("HeadMenuPlayerBonusUpdate", 1.6f);
			} else {
				//text win
				MenuInfoTextWin ();

				//completeed level
				MenuInfoLevelCompleteUpdate ();

				//best time
				MenuInfoPlayerWinUpdate ();

				//result
				MenuInfoResultUpdate ();

				//head menu
				HeadMenuPlayerPlayedGameUpdate ();
				HeadMenuPlayerBonusUpdate ();
			}
		} else {
			errorString = "!!! Not Active PlayFieldManager in MenuInformSetText()";
			Debug.LogError (errorString);
		}
	}

	private void MenuInfoClearText() {
		PlayFieldManager.WindowInformSetText ("0", 0);
		PlayFieldManager.WindowInformSetText ("0", 1);
		PlayFieldManager.WindowInformSetText ("0", 2);
		PlayFieldManager.WindowInformSetText ("0", 3);
	}

	public void OpenMenuInform() {
		if (PlayFieldManager) {
			PlayFieldManager.ShowWindowInform ();
		} else {
			errorString = "! Not Find PlayerFieldManager";
			Debug.LogError (errorString);
		}
	}

	public void CloseMenuInform() {
		if (PlayFieldManager) {
			PlayFieldManager.HideWindowInform ();

			Invoke ("MenuInfoClearText", 1);
		} else {
			errorString = "! Not Find PlayerFieldManager";
			Debug.LogError (errorString);
		}
	}

	private void WindowAdwiceShowLevelText() {
		string stDifficultyKey = difficultyState.ToString ();
		string stDifficulty = "Difficulty " + ": " + stDifficultyKey;

		string stRes = stDifficulty;

		if (PlayFieldManager) {
			PlayFieldManager.WindowAdviceClearText ();
			PlayFieldManager.WindowAdviceSetText (stRes);
		} else {
			errorString = "! Not Find PlayerFieldManager";
			Debug.LogError (errorString);
		}
	}

	public void WindowAdwiceShowText(string textVal, int timeVal = 0) {
		if (PlayFieldManager) {
			PlayFieldManager.WindowAdviceClearText ();
			PlayFieldManager.WindowAdviceSetText (textVal);
		} else {
			errorString = "! Not Find PlayerFieldManager";
			Debug.LogError (errorString);
		}

		if (timeVal == 0) {
			if (IsInvoking ("WindowAdwiceShowLevelText")) {
				CancelInvoke ("WindowAdwiceShowLevelText");
			}

			if (textVal.Length <= 40) {
				Invoke ("WindowAdwiceShowLevelText", 5);
			} else {
				Invoke ("WindowAdwiceShowLevelText", 10);
			}
		} else if (timeVal > 0) {
			if (IsInvoking ("WindowAdwiceShowLevelText")) {
				CancelInvoke ("WindowAdwiceShowLevelText");
			}

			Invoke ("WindowAdwiceShowLevelText", timeVal);
		}
	}

	public void WindowAdwiceShowProgressText() {
		string stRes = "start level";

		WindowAdwiceShowText (stRes, -1);
	}

	public void WindowAdwiceShowProgressCompleteText() {
		string stRes = "Start Game!!!";

		WindowAdwiceShowText (stRes, 2);
	}

	public void WindowAdwiceUpdateText () {
		WindowAdwiceShowLevelText ();
	}

	public void WindowAdwiceShowTextWithKey(string stValKey, int timeVal = 0) {
		string stShow = stValKey;

		if (stShow == "") {
			errorString = "!!! TextManager: Not find string with Key:" + stValKey;
			Debug.Log (errorString);
		} else {
			WindowAdwiceShowText (stShow, timeVal);
		}
	}

	public void CloseAllMenuWindows() {
		if (GameMenu) {
			GameMenu.DisActivateMenu ();
			GameMenu.DisActivateWindow ();
		} else {
			PlayFieldManager.DisActivateMenu ();
			PlayFieldManager.DisActivateWindow ();
		}
	}

	public void HideMenuPanel() {
		if (GameMenu) {
			GameMenu.HideMenu ();
		}
	}

	public void ShowMenuPanel() {
		if (GameMenu) {
			GameMenu.ShowMenu ();
		}
	}

	public void WindowConsoleBigOpen() {
		if (GameMenu) {
			GameMenu.ActivateConsoleWindow (13);
		} else {
			PlayFieldManager.ActivateConsoleWindow (13);
		}
	}

	public void WindowConsoleSmolOpen() {
		if (GameMenu) {
			GameMenu.ActivateConsoleWindow (14);
		} else {
			PlayFieldManager.ActivateConsoleWindow (14);
		}
	}

	public void WindowConsoleYesNoOpen() {
		if (GameMenu) {
			GameMenu.ActivateConsoleWindow (15);
		} else {
			
		}
	}

	public void WindowConsoleBigShowMessage(string textHead, string textInformation, string pictureResors = "") {
		if (GameMenu) {
			GameMenu.ConsoleWinInformationBig_SetInf (textHead, textInformation, pictureResors);
		} else {
			PlayFieldManager.ConsoleWinInformationBig_SetInf (textHead, textInformation, pictureResors);
		}

		WindowConsoleBigOpen ();
	}

	public void WindowConsoleSmolShowMessage(string textHead, string textInformation, string pictureResors = "") {
		if (GameMenu) {
			GameMenu.ConsoleWinInformationSmol_SetInf (textHead, textInformation, pictureResors);
		} else {
			PlayFieldManager.ConsoleWinInformationSmol_SetInf (textHead, textInformation, pictureResors);
		}

		WindowConsoleSmolOpen ();
	}

	public void WindowConsoleSmolShowError(string textInformation) {
		WindowConsoleSmolShowMessage ("ERROR", textInformation);
	}

	// System Time
	public string GetSystemTime() {
		return System.DateTime.Now.ToShortTimeString ();
	}

	// My predifine action
	public int GetRandomInt(int minNum, int maxNum, int multNum = 1) {
		int resNumber = minNum;

		if (multNum <= 1) {
			resNumber = Random.Range (minNum, maxNum + 1);
		} else {
			resNumber = Random.Range (minNum, maxNum * multNum + 1);
			resNumber = Mathf.RoundToInt(resNumber / multNum);

			if (resNumber < minNum)
				resNumber = minNum;
		}

		return resNumber;
	}

	public void PauseGame() {
		if (!Paused) {
			Paused = true;
		}
	}

	public void ContinueGame() {
		if (Paused) {
			Paused = false;
		}
	}

	public void PauseGameWithDelay(float val) {
		Invoke ("PauseGame", val);
	}

	private string FormatTimeHHHMMMorMMMSSS(int milSec) {
		// grab hours
		int aHour = (int)milSec / 3600;
		aHour=aHour % 24;

		// grab minutes
		int aMinute=(int)milSec / 60;
		aMinute=aMinute % 60;

		// grab seconds
		int aSecond=(int)milSec%60;

		// format strings for individual mm/ss/mills
		string hour = aHour.ToString();
		if(hour.Length < 2)
			hour="0"+hour;
		
		string minutes = aMinute.ToString();
		if(minutes.Length < 2)
			minutes="0"+minutes;

		string seconds = aSecond.ToString();
		if(seconds.Length < 2)
			seconds="0"+seconds;

		// pull together a formatted string to return
		string timeString = "";

		if (aHour == 0) {
			timeString = minutes + " " + "m" + " " + seconds + " " + "s";
		} else {
			timeString = hour + " " + "h" + " " + minutes + " " + "m";
		}

		return timeString;
	}

	private string FormatTimeHHMMSSorMMSS(int milSec) {
		// grab hours
		int aHour = (int)milSec / 3600;
		aHour=aHour % 24;

		// grab minutes
		int aMinute=(int)milSec / 60;
		aMinute=aMinute % 60;

		// grab seconds
		int aSecond=(int)milSec%60;

		// format strings for individual mm/ss/mills
		string hour = aHour.ToString();
		if(hour.Length < 2)
			hour="0"+hour;

		string minutes = aMinute.ToString();
		if(minutes.Length < 2)
			minutes="0"+minutes;

		string seconds = aSecond.ToString();
		if(seconds.Length < 2)
			seconds="0"+seconds;

		// pull together a formatted string to return
		string timeString = "";

		if (aHour == 0) {
			timeString = minutes + ":" + seconds;
		} else {
			timeString = hour + ":" + minutes + ":" + seconds;
		}

		return timeString;
	}

	public void SetNamePlayer(string val) {
		PlayerManager.SetPlayerName (val);
	}

	public string GetNamePlayer() {
		return PlayerManager.GetPlayerName ();
	}

	public void SetDifficaltyPlayer(int val) {
		DifficultyState difficaltyNew = (DifficultyState)val;

		if (difficaltyNew != difficultyState) {
			difficultyState = difficaltyNew;

			playedLevel = 0;
			winLevel = 0;
			loseLevel = 0;
		}
	}

	public DifficultyState GetDifficaltyPlayer() {
		return difficultyState;
	}

	public bool CursorOverUI() {
		if (GameMenu) {
			return GameMenu.overMenuUI;
		} else {
			return PlayFieldManager.overMenuUI;
		}
	}

	public void CloseAllUI() {
		//windows
		CloseAllMenuWindows();

		//close all windows
		HideMenuPanel();

		//menu information
		CloseMenuInform ();
	}

	private List<int> ConvertNumberPointInRowCol(int numPoint) {
		int resNumGlobalRow = -1;
		int resNumGlobalCol = -1;

		if (numPoint >= 0) {
			int divResPoint = numPoint % countPointInRow;
			int divGlobPoint = (numPoint - divResPoint) / countPointInRow;

			resNumGlobalRow = divGlobPoint;
			resNumGlobalCol = divResPoint;
		}

		return new List<int> () { resNumGlobalRow, resNumGlobalCol };
	}

	public int ConvertRowColInNumberPoint(int numRow, int numCol) {
		int resNumPoint = -1;

		if (numRow >= 0 && numCol >= 0) {
			resNumPoint = (numRow * countPointInRow) + numCol;
		}

		return resNumPoint;
	}

	public int ConvertRowColInNumberPoint(RowCol rc) {
		return ConvertRowColInNumberPoint (rc.row, rc.col);
	}

	private bool GetLockPointNP(int numPoint) {
		if (numPoint >= 0) {
			if (PlayFieldManager) {
				return PlayFieldManager.GetLockPanel (numPoint);
			} else {
				return true;
			}
		} else {
			return true;
		}
	}

	public bool GetLockPointRC(int numRow, int numCol) {
		if (numRow >= 0) {
			if (numCol >= 0) {
				// find global row, col possition
				int numPoint = ConvertRowColInNumberPoint (numRow, numCol);

				return GetLockPointNP (numPoint);
			} else {
				return true;
			}
		} else {
			return true;
		}
	}

	private void SetLockPointNP(int numPoint, bool val = true) {
		if (numPoint >= 0) {
			if (PlayFieldManager) {
				PlayFieldManager.SetLockPanel (numPoint, val);
			}
		}
	}

	private void SetLockPointRC(int numRow, int numCol, bool val = true) {
		if (numRow >= 0) {
			if (numCol >= 0) {
				// find global row, col possition
				int numPoint = ConvertRowColInNumberPoint (numRow, numCol);

				SetLockPointNP (numPoint, val);
			}
		}
	}

	private string GetTextPointNP(int numPoint, int numHelp = -1) {
		if (numPoint >= 0) {
			if (PlayFieldManager) {
				return PlayFieldManager.GetTextPanel (numPoint);
			} else {
				return "";
			}
		} else {
			return "";
		}
	}

	public string GetTextPointRC(int numRow, int numCol, int numHelp = -1) {
		if (numRow >= 0) {
			if (numCol >= 0) {
				// find global row, col possition
				int numPoint = ConvertRowColInNumberPoint (numRow, numCol);

				return GetTextPointNP (numPoint, numHelp);
			} else {
				return "";
			}
		} else {
			return "";
		}
	}

	private void SetTextPointNP(int numPoint, string numSet) {
		if (numPoint >= 0) {
			if (PlayFieldManager) {
				PlayFieldManager.SetTextPanel (numPoint, numSet);
			}
		}
	}

	public void SetTextPointRC(int numRow, int numCol, string numSet) {
		if (numRow >= 0) {
			if (numCol >= 0) {
				// find global row, col possition
				int numPoint = ConvertRowColInNumberPoint (numRow, numCol);

				SetTextPointNP (numPoint, numSet);
			}
		}
	}

	private void SetTextPointNP_Style(int numPoint, FontStyle val) {
		if (numPoint >= 0) {
			if (PlayFieldManager) {
				PlayFieldManager.SetTextStylePanel (numPoint, val);
			}
		}
	}

	private void SetTextPointRC_Style(int numRow, int numCol, FontStyle val) {
		if (numRow >= 0) {
			if (numCol >= 0) {
				// find global row, col possition
				int numPoint = ConvertRowColInNumberPoint (numRow, numCol);

				SetTextPointNP_Style (numPoint, val);
			}
		}
	}

	private void SetTextPointNP_Color(int numPoint, Color val) {
		if (numPoint >= 0) {
			if (PlayFieldManager) {
				PlayFieldManager.SetTextColorPanel (numPoint, val);
			}
		}
	}

	private void SetTextPointRC_Color(int numRow, int numCol, Color val) {
		if (numRow >= 0) {
			if (numCol >= 0) {
				// find global row, col possition
				int numPoint = ConvertRowColInNumberPoint (numRow, numCol);

				SetTextPointNP_Color (numPoint, val);
			}
		}
	}

	private void SetPanelPointNP_Color(int numPoint, Color val) {
		if (numPoint >= 0) {
			if (PlayFieldManager) {
				PlayFieldManager.SetColorPanel (numPoint, val);
			}
		}
	}

	public void SetPanelPointRC_Color(int numRow, int numCol, Color val) {
		if (numRow >= 0) {
			if (numCol >= 0) {
				// find global row, col possition
				int numPoint = ConvertRowColInNumberPoint (numRow, numCol);

				SetPanelPointNP_Color (numPoint, val);
			}
		}
	}

	private void SetPanelPointNP_ColorDef(int numPoint) {
		if (numPoint >= 0) {
			if (PlayFieldManager) {
				PlayFieldManager.SetColorPanelDef (numPoint);
			}
		}
	}

	public void SetPanelPointRC_ColorDef(int numRow, int numCol) {
		if (numRow >= 0) {
			if (numCol >= 0) {
				// find global row, col possition
				int numPoint = ConvertRowColInNumberPoint (numRow, numCol);

				SetPanelPointNP_ColorDef (numPoint);
			}
		}
	}

	public bool IsPointEmpty(string stPoint) {
		if (stPoint == "") {
			return true;
		} else {
			return false;
		}
	}

	private List<int> CreateListFreePointNP() {
		int countPoint = countPointInRow * countPointInCol;

		List<int> res = new List<int> ();

		for (int i = 0; i < countPoint; i++) {
			if (IsPointEmpty (GetTextPointNP (i))) {
				res.Add (i);
			}
		}

		return res;
	}

	private List<RowCol> CreateListFreePointRC() {
		int countRow = countPointInRow;
		int countCol = countPointInCol;

		List<RowCol> res = new List<RowCol> ();

		for (int i = 0; i < countRow; i++) {
			for (int j = 0; j < countCol; j++) {
				if (IsPointEmpty (GetTextPointRC (i, j))) {
					res.Add (new RowCol(i, j));
				}
			}
		}

		return res;
	}

	private List<RowCol> CreateListAllPoint() {
		int countRow = countPointInRow;
		int countCol = countPointInCol;

		List<RowCol> res = new List<RowCol> ();

		for (int i = 0; i < countRow; i++) {
			for (int j = 0; j < countCol; j++) {
				res.Add (new RowCol(i, j));
			}
		}

		return res;
	}

	private List<string> CreateListAllPointNumber() {
		int countPoint = countPointInRow * countPointInCol;

		List<string> res = new List<string> ();

		for (int i = 1; i <= countPoint; i++) {
			res.Add (i.ToString());
		}

		return res;
	}

	private List<RowCol> RemoveFromList(List<RowCol> startList, List<RowCol> removeList) {
		List<RowCol> res = new List<RowCol> (startList);

		for (int i = 0; i < removeList.Count; i++) {
			RowCol resEl = removeList [i];

			res.Remove (resEl);
		}

		return res;
	}

	private int AIPanelSelect() {
		if (difficultyState == DifficultyState.easy) {
			List<int> freePointList = CreateListFreePointNP ();

			if (freePointList.Count > 0) {
				int numGet = GetRandomInt (0, freePointList.Count - 1);

				return freePointList [numGet];
			} else {
				return -1;
			}
		} else if (difficultyState == DifficultyState.medium) {
			RowCol oneStepForWin = new RowCol ();
			if (ChackGamePoolOneStepForWin (signPlayer, out oneStepForWin)) {
				return ConvertRowColInNumberPoint (oneStepForWin);
			} else {
				List<int> freePointList = CreateListFreePointNP ();

				if (freePointList.Count > 0) {
					int numGet = GetRandomInt (0, freePointList.Count - 1);

					return freePointList [numGet];
				} else {
					return -1;
				}
			}
		} else {
			RowCol oneStepForWin = new RowCol ();
			string signAI = (signPlayer == "X" ? "0" : "X");
			if (ChackGamePoolOneStepForWin (signAI, out oneStepForWin)) {
				return ConvertRowColInNumberPoint (oneStepForWin);
			} else {
				if (ChackGamePoolOneStepForWin (signPlayer, out oneStepForWin)) {
					return ConvertRowColInNumberPoint (oneStepForWin);
				} else {
					if (IsPointEmpty (GetTextPointRC (1, 1))) {
						return ConvertRowColInNumberPoint (1, 1);
					} else {
						List<int> freePointList = CreateListFreePointNP ();

						if (freePointList.Count > 0) {
							int numGet = GetRandomInt (0, freePointList.Count - 1);

							return freePointList [numGet];
						} else {
							return -1;
						}
					}
				}
			}
		}
	}

	private void AIStepWithDelay(float valDel) {
		int choosePanel = AIPanelSelect ();

		if (choosePanel >= 0) {
			PanelGameSelectPanel (choosePanel);

			Invoke ("PanelGameActiveSetSign", valDel);
		} else {
			errorString = "- AI not select point";
			Debug.LogError (errorString);
		}
	}

	private void PanelGameActiveSetSign() {
		PanelGameSetSign (selNumPoint);

		//play sound
		PlaySoundByIndex (1);
	}

	private void PanelGameSetSign(int numPanel) {
		if (
			(!developState && !lockNumber) //play in game
			|| (developState) //develop level
		) {
			string signForSet = signPlayer;
			if (!playerStep) {
				if (signPlayer == "X") {
					signForSet = "0";
				} else {
					signForSet = "X";
				}
			}

			SetTextPointNP (selNumPoint, signForSet);
			SetLockPointNP (selNumPoint);

			lockNumber = true;

			playerStep = !playerStep;
			HeadMenuPlayerPanelUpdateColor ();

			//chack level Complete
			if (!developState) { //not develop state
				if (IsGameCompleted ()) {
					completeLevel = true;

					// calculate score
					scoreLevel = BonusForPlayer ();

					//stop Level
					StopActiveLevel ();

					//open window inform
					Invoke("OpenMenuInform", 0.5f);
					MenuInformSetText (true);
				} else {
					if (!playerStep) {
						AIStepWithDelay (1f);
					}
				}
			}
		}
	}

	private void PanelGameSelectPanel(int numPoint) {
		//select point
		if (selNumPoint != numPoint) {
			//find global row, col possition
			List<int> convertList = ConvertNumberPointInRowCol (numPoint);
			int numGlobalRow = convertList [0];
			int numGlobalCol = convertList [1];

			//find number
			string activeNumber = GetTextPointNP (numPoint);
			activeNumber = (activeNumber == "" ? "-1" : activeNumber);

			//change select lock number
			bool activeLockNumber = GetLockPointNP (numPoint);

			//select number panel
			if (selNumber != activeNumber) {
				selNumber = activeNumber;
			}

			// update possition
			selNumPoint = numPoint;

			selNumGlobalRow = numGlobalRow;
			selNumGlobalCol = numGlobalCol;

			lockNumber = activeLockNumber;
		}
	}

	private void FillGamePool() {
		HeadMenuPlayerPanelUpdateColor ();

		if (!playerStep) {
			AIStepWithDelay (1f);
		}

		fillingLevel = false;

		Debug.Log (" - Complete fill level");
	}

	private IEnumerator FillGamePool_Coroutine() {
		//wait clear
		yield return new WaitUntil (() => clearingLevel == false);

		HeadMenuPlayerPanelUpdateColor ();

		if (!playerStep) {
			AIStepWithDelay (1f);
		}

		fillingLevel = false;

		Debug.Log (" - Complete fill level");
	}

	private void ClearGamePool() {
		int countRow = countPointInRow;
		int countCol = countPointInCol;

		//clear game pool
		for (int i = 0; i < countRow; i++) {
			for (int j = 0; j < countCol; j++) {
				//clear points
				SetTextPointRC (i, j, "");
				SetPanelPointRC_ColorDef (i, j);

				SetLockPointRC (i, j, false);
			}
		}

		clearingLevel = false;

		Debug.Log (" - Complete clear level");
	}

	private IEnumerator ClearGamePool_Coroutine() {
		int countRow = countPointInRow;
		int countCol = countPointInCol;

		yield return new WaitUntil (() => fillingLevel == false);

		//clear game pool
		for (int i = 0; i < countRow; i++) {
			for (int j = 0; j < countCol; j++) {
				//clear points
				SetTextPointRC (i, j, "");
				SetPanelPointRC_ColorDef (i, j);

				SetLockPointRC (i, j, false);

				yield return null;
			}
				
		}

		clearingLevel = false;

		Debug.Log (" - Complete clear level");
	}

	// rule func

	private List<string> GetRowList(int numRow) {
		List<string> res = new List<string> ();

		int sizeRow = countPointInCol;

		for (int i = 0; i < sizeRow; i++) {
			int selCol = i;

			string numPoint = GetTextPointRC (numRow, selCol);

			if (!IsPointEmpty (numPoint)) {
				res.Add (numPoint);
			}
		}

		return res;
	}

	private List<string> GetColList(int numCol) {
		List<string> res = new List<string> ();

		int sizeCol = countPointInCol;

		for (int i = 0; i < sizeCol; i++) {
			int selRow = i;

			string numPoint = GetTextPointRC (selRow, numCol);

			if (!IsPointEmpty (numPoint)) {
				res.Add (numPoint);
			}
		}

		return res;
	}

	private List<string> GetDiagonalList(int numDiag) {
		List<string> res = new List<string> ();

		int sizeCol = countPointInCol;

		for (int i = 0; i < sizeCol; i++) {
			if (numDiag == 0) {
				int selRow = i;
				int selCol = i;

				string numPoint = GetTextPointRC (selRow, selCol);

				if (!IsPointEmpty (numPoint)) {
					res.Add (numPoint);
				}
			} else {
				int selRow = sizeCol - 1 - i;
				int selCol = i;

				string numPoint = GetTextPointRC (selRow, selCol);

				if (!IsPointEmpty (numPoint)) {
					res.Add (numPoint);
				}
			}
		}

		return res;
	}

	private List<string> GetRowListAll(int numRow) {
		List<string> res = new List<string> ();

		int sizeRow = countPointInCol;

		for (int i = 0; i < sizeRow; i++) {
			int selCol = i;

			string numPoint = GetTextPointRC (numRow, selCol);

			res.Add (numPoint);
		}

		return res;
	}

	private List<string> GetColListAll(int numCol) {
		List<string> res = new List<string> ();

		int sizeCol = countPointInCol;

		for (int i = 0; i < sizeCol; i++) {
			int selRow = i;

			string numPoint = GetTextPointRC (selRow, numCol);

			res.Add (numPoint);
		}

		return res;
	}

	private List<string> GetDiagonalListAll(int numDiag) {
		List<string> res = new List<string> ();

		int sizeCol = countPointInCol;

		for (int i = 0; i < sizeCol; i++) {
			if (numDiag == 0) {
				int selRow = i;
				int selCol = i;

				string numPoint = GetTextPointRC (selRow, selCol);

				res.Add (numPoint);
			} else {
				int selRow = sizeCol - 1 - i;
				int selCol = i;

				string numPoint = GetTextPointRC (selRow, selCol);

				res.Add (numPoint);
			}
		}

		return res;
	}

	private List<int> GetRowListNP(int numRow) {
		List<int> res = new List<int> ();

		int sizeRow = countPointInCol;

		for (int i = 0; i < sizeRow; i++) {
			int selCol = i;

			int numPoint = ConvertRowColInNumberPoint (numRow, selCol);

			res.Add (numPoint);
		}

		return res;
	}

	private List<int> GetColListNP(int numCol) {
		List<int> res = new List<int> ();

		int sizeCol = countPointInCol;

		for (int i = 0; i < sizeCol; i++) {
			int selRow = i;

			int numPoint = ConvertRowColInNumberPoint (selRow, numCol);

			res.Add (numPoint);
		}

		return res;
	}

	private List<int> GetDiagonelListNP(int numDiag) {
		List<int> res = new List<int> ();

		int sizeCol = countPointInCol;

		for (int i = 0; i < sizeCol; i++) {
			if (numDiag == 0) {
				int selRow = i;
				int selCol = i;

				int numPoint = ConvertRowColInNumberPoint (selRow, selCol);

				res.Add (numPoint);
			} else {
				int selRow = sizeCol - 1 - i;
				int selCol = i;

				int numPoint = ConvertRowColInNumberPoint (selRow, selCol);

				res.Add (numPoint);
			}
		}

		return res;
	}

	private bool ChackDiagonelFill(int numDiag, out int res) {
		res = -1;
		List<string> digList = GetDiagonalListAll (numDiag);
		List<string> resList = digList.FindAll (r => IsPointEmpty(r));

		if (resList.Count > 0) {
			int ind = digList.FindIndex (r => IsPointEmpty(r));
			res = ind;

			return false;
		}

		return true;
	}

	private bool ChackRowFill(int numRow, out int res) {
		res = -1;
		List<string> rowList = GetRowListAll (numRow);
		List<string> resList = rowList.FindAll (r => IsPointEmpty(r));

		if (resList.Count > 0) {
			int ind = rowList.FindIndex (r => IsPointEmpty(r));
			res = ind;

			return false;
		}

		return true;
	}

	private bool ChackColFill(int numCol, out int res) {
		res = -1;
		List<string> colList = GetColListAll (numCol);
		List<string> resList = colList.FindAll (r => IsPointEmpty(r));

		if (resList.Count > 0) {
			int ind = colList.FindIndex (r => IsPointEmpty (r));
			res = ind;

			return false;
		}

		return true;
	}

	private bool ChackDiagonelAllWithSign(int numDiagonel, string signChack) {
		int countRow = countPointInCol;

		List<string> digListSign = GetDiagonalListAll (numDiagonel);

		if (digListSign.Count == countRow) {

			List<string> digListSignFind = digListSign.FindAll (s => s == signChack);

			if (digListSign.Count == digListSignFind.Count) {
				return true;
			} else {
				return false;
			}
		}

		return false;
	}

	private bool ChackRowAllWithSign(int numRow, string signChack) {
		int countRow = countPointInCol;

		List<string> rowListSign = GetRowListAll (numRow);

		if (rowListSign.Count == countRow) {

			List<string> rowListSignFind = rowListSign.FindAll (s => s == signChack);

			if (rowListSign.Count == rowListSignFind.Count) {
				return true;
			} else {
				return false;
			}
		}

		return false;
	}

	private bool ChackColAllWithSign(int numCol, string signChack) {
		int countCol = countPointInCol;

		List<string> colListSign = GetColListAll (numCol);

		if (colListSign.Count == countCol) {

			List<string> colListSignFind = colListSign.FindAll (s => s == signChack);

			if (colListSign.Count == colListSignFind.Count) {
				return true;
			} else {
				return false;
			}
		}

		return false;
	}

	private bool ChackDiagonelForWin(int numDiagonel, string signChack) {
		List<string> digListSign = GetDiagonalListAll (numDiagonel);
		List<string> digListSignFind = digListSign.FindAll (s => s == signChack);

		if (digListSign.Count == digListSignFind.Count) {
			return true;
		} else {
			return false;
		}
	}

	private bool ChackRowForWin(int numRow, string signChack) {
		List<string> rowListSign = GetRowListAll (numRow);
		List<string> rowListSignFind = rowListSign.FindAll (s => s == signChack);

		if (rowListSign.Count == rowListSignFind.Count) {
			return true;
		} else {
			return false;
		}
	}

	private bool ChackColForWin(int numCol, string signChack) {
		List<string> colListSign = GetColListAll (numCol);
		List<string> colListSignFind = colListSign.FindAll (s => s == signChack);

		if (colListSign.Count == colListSignFind.Count) {
			return true;
		} else {
			return false;
		}
	}

	private bool ChackDiagonelOneStepForWin(int numDiagonel, string signChack) {
		int countRow = countPointInCol;

		List<string> digListSign = GetDiagonalList (numDiagonel);

		if (digListSign.Count == countRow - 1) {
			
			List<string> digListSignFind = digListSign.FindAll (s => s == signChack);

			if (digListSign.Count == digListSignFind.Count) {
				return true;
			} else {
				return false;
			}
		}

		return false;
	}

	private bool ChackRowOneStepForWin(int numRow, string signChack) {
		int countRow = countPointInCol;

		List<string> rowListSign = GetRowList (numRow);

		if (rowListSign.Count == countRow - 1) {
			List<string> rowListSignFind = rowListSign.FindAll (s => s == signChack);

			if (rowListSign.Count == rowListSignFind.Count) {
				return true;
			} else {
				return false;
			}
		}

		return false;
	}

	private bool ChackColOneStepForWin(int numCol, string signChack) {
		int countCol = countPointInCol;

		List<string> colListSign = GetColList (numCol);

		if (colListSign.Count == countCol - 1) {
			List<string> colListSignFind = colListSign.FindAll (s => s == signChack);

			if (colListSign.Count == colListSignFind.Count) {
				return true;
			} else {
				return false;
			}
		}

		return false;
	}

	private bool ChackGamePoolForWin(string signChack, out List<int> winLine) {
		winLine = new List<int>();
		bool res = false;
		int countRow = countPointInCol;
		int countCol = countPointInRow;

		// chanck diagonels
		if (!res) {
			for (int i = 0; i < countRow - 1; i++) {
				if (ChackDiagonelForWin (i, signChack)) {
					winLine = GetDiagonelListNP (i);

					return true;
				}
			}
		}

		// chanck rows
		if (!res) {
			for (int i = 0; i < countRow; i++) {
				if (ChackRowForWin (i, signChack)) {
					winLine = GetRowListNP (i);

					return true;
				}
			}
		}

		// chanck col
		if (!res) {
			for (int i = 0; i < countCol; i++) {
				if (ChackColForWin (i, signChack)) {
					winLine = GetColListNP (i);

					return true;
				}
			}
		}

		return res;
	}

	private bool ChackGamePoolOneStepForWin(string signChack, out RowCol res) {
		res = new RowCol ();
		bool resBool = false;
		int countRow = countPointInCol;
		int countCol = countPointInRow;

		// chanck diagonels
		if (!resBool) {
			for (int i = 0; i < countRow - 1; i++) {
				if (ChackDiagonelOneStepForWin (i, signChack)) {
					int numPoint = 0;
					ChackDiagonelFill (i, out numPoint);

					if (i == 0) {
						res.row = numPoint;
						res.col = numPoint;
					} else {
						res.row = countRow - 1 - numPoint;
						res.col = numPoint;
					}

					return true;
				}
			}
		}

		// chanck rows
		if (!resBool) {
			for (int i = 0; i < countRow; i++) {
				if (ChackRowOneStepForWin (i, signChack)) {
					int numPoint = 0;
					ChackRowFill (i, out numPoint);

					res.row = i;
					res.col = numPoint;

					return true;
				}
			}
		}

		// chanck col
		if (!resBool) {
			for (int i = 0; i < countCol; i++) {
				if (ChackColOneStepForWin (i, signChack)) {
					int numPoint = 0;
					ChackColFill (i, out numPoint);

					res.row = numPoint;
					res.col = i;

					return true;
				}
			}
		}

		return resBool;
	}

	private bool ChackGamePoolForFill(out RowCol res) {
		res = new RowCol ();
		int countRow = countPointInCol;

		for (int i = 0; i < countRow; i++) {
			int colNumb = -1;

			bool resFill = ChackRowFill (i, out colNumb);

			if (!resFill) {
				res.row = i;
				res.col = colNumb;

				return false;
			}
		}

		return true;
	}

	private bool ChackGamePoolForFill() {
		RowCol res = new RowCol ();
		bool resBool = ChackGamePoolForFill(out res);

		return resBool;
	}

	private void HighightPointLine(List<int> val) {
		foreach (int item in val) {
			SetPanelPointNP_Color (item, colorHighlight);
		}
	}

	private bool IsGameCompleted() {
		bool res = false;

		if (!res) {
			List<int> winLine = new List<int> ();
			if (ChackGamePoolForWin ("X", out winLine)) {
				HighightPointLine (winLine);

				if (signPlayer == "X") {
					isPlayerWin = true;

					playedLevel++;
					winLevel++;
				} else {
					isPlayerLose = true;

					playedLevel++;
					loseLevel++;
				}

				res = true;
			}
		}

		if (!res) {
			List<int> winLine = new List<int> ();
			if (ChackGamePoolForWin ("0", out winLine)) {
				HighightPointLine (winLine);

				if (signPlayer == "0") {
					isPlayerWin = true;

					playedLevel++;
					winLevel++;
				} else {
					isPlayerLose = true;

					playedLevel++;
					loseLevel++;
				}

				res = true;
			}
		}

		if (!res) {
			if (ChackGamePoolForFill ()) {
				playedLevel++;

				res = true;
			}
		}

		return res;
	}

	private bool IsPlayFieldButtonBlocked() {
		if ( clearingLevel || fillingLevel || pauseLevel || completeLevel) {
			return true;
		} else {
			return false;
		}
	}

	public void StartLevel() {
		if (!fillingLevel) {
			if (startLevel) {
				ClearLevel (useCoroutineMain);
			}

			//show desccribe level
			WindowAdwiceShowLevelText ();

			FillLevel (useCoroutineMain);

			// hide window
			CloseAllUI ();
		}
	}

	private void FillLevel(bool useCoroutine = false) {
		// setVariable
		startLevel = true;
		fillingLevel = true;

		//filling game pool
		if (useCoroutine) {
			StartCoroutine ("FillGamePool_Coroutine");
		} else {
			FillGamePool ();
		}

		scoreLevel = 0;

		if (useCoroutine) {
			
		} else {
			fillingLevel = false;
		}
	}

	private void ClearLevel(bool useCoroutine = false) {
		//complete level
		startLevel = false;
		completeLevel = false;
		clearingLevel = true;

		// who step first and whot sign
		if (isPlayerWin) {
			playerStep = true;
			signPlayer = "X";
		} else if (isPlayerLose) {
			playerStep = false;
			signPlayer = "0";
		} else {
			if (firstPlayerStep_save) {
				playerStep = false;
			} else {
				playerStep = true;
			}
		}
		firstPlayerStep_save = playerStep;

		isPlayerWin = false;
		isPlayerLose = false;

		//clear game pool
		if (useCoroutine) {
			StartCoroutine ("ClearGamePool_Coroutine");
		} else {
			ClearGamePool();
		}

		//clear game manager
		selNumPoint = -1;
		selNumGlobalRow = -1;
		selNumGlobalCol = -1;
		selNumber = "-1";
		lockNumber = false;

		if (useCoroutine) {

		} else {
			clearingLevel = false;
		}
	}

	// bonus logic
	private int ProcentRandom(int pr1, int pr2, int pr3 = 0) {
		int res = Random.Range (1, pr1 + pr2 + pr3 + 1);

		if (res <= pr1) {
			return 0;
		} else if (res <= pr1 + pr2) {
			return 1;
		} else {
			return 2;
		}
	}

	// UI uttons

	public void ClickOnPanelGamePlayer_Button(int numPanel) {
		if (!IsPlayFieldButtonBlocked ()) {
			if (playerStep) {
				PanelGameSelectPanel (numPanel);

				PanelGameSetSign (selNumPoint);
			}
		}

		//play sound
		PlaySoundByIndex (1);
	}

	public void GoInGame_Button() {
		GoInGame ();

		//play sound
		PlaySoundByIndex (2);
	}

	public void GoInMenu_Button() {
		GoInMenu ();

		//play sound
		PlaySoundByIndex (2);
	}

	public void StartLevel_Button() {
		StartLevel ();

		//play sound
		PlayButtonSound ();
	}

	public void LevelDoTestWork_Button() {
		//ClearLevel (useCoroutineMain);

		//play sound
		PlayButtonSound ();
	}

	public void ClearProgreesPlayer_Button() {
		ClearDataProgress ();

		//play sound
		PlayButtonSound ();
	}

	//repeat update
	private void UpdateSystemTime() {
		if (PlayFieldManager) {
			PlayFieldManager.SetSystemTime (GetSystemTime ());
		}
	}

	//standart 

	void Awake () {
		Init ();
	}

	void Start () {
		StartGame ();
	}

	void Update () {
		
	}
}

[System.Serializable]
public class CountOpenPanel {
	public int countInEasy = 0;
	public int countInMedium = 0;
	public int countInHard = 0;
}
  