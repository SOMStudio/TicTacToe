using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMenuController_TicTacToe : BaseMenuController {
	public bool overMenuUI = false;

	public string namePlayerVal = "Anonim";
	public InputField namePlayer;

	public int languagePlayerVal = 0;
	public Dropdown languagePlayer;

	public int difficaltyPlayerVal = 0;
	public Toggle[] difficultyToggleList;

	public int multiThread = 1;
	public Toggle multiThreadToggle;

	public Text versionText;

	// window Arcade
	public Text scorePlayer;

	//reference for GameControl
	public GameController_TicTacToe gameController;

	[System.NonSerialized]
	public static GameMenuController_TicTacToe Instance;

	void Awake () {
		// activate instance
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	public override void RestoreOptionsPref() {
		base.RestoreOptionsPref ();

		didInit = false;

		gameController = GameController_TicTacToe.Instance;
		gameController.SetMenuManager (Instance);

		// addition Prefub
		if (PlayerPrefs.HasKey (gamePrefsName + "_Difficalty")) {
			difficaltyPlayerVal = PlayerPrefs.GetInt (gamePrefsName + "_Difficalty");
		}
		if (difficultyToggleList [difficaltyPlayerVal]) {
			difficultyToggleList [difficaltyPlayerVal].isOn = true;
		}

		if (PlayerPrefs.HasKey (gamePrefsName + "_MultiThread")) {
			multiThread = PlayerPrefs.GetInt (gamePrefsName + "_MultiThread");
		}
		if (multiThreadToggle != null) {
			multiThreadToggle.isOn = (multiThread == 1 ? true : false);
		}

		if (versionText != null) {
			versionText.text = Application.productName + " " + Application.version + " " + Application.platform.ToString();
		}

		didInit = true;

		//set params for GameMenu
		gameController.ActivateMenuParametrs ();
	}

	public override void SaveOptionsPrefs ()
	{
		base.SaveOptionsPrefs ();

		// addition Prefub
		PlayerPrefs.SetInt(gamePrefsName + "_Difficalty", difficaltyPlayerVal);

		PlayerPrefs.SetInt(gamePrefsName + "_MultiThread", multiThread);

		gameController.UpdateSoundVolume ();
		gameController.UpdateMusicVolume ();
	}

	public override void ExitGame ()
	{
		SaveOptionsPrefs ();

		gameController.SaveDataBeforeExit ();

		gameController.PlayButtonSound ();

		base.ExitGame ();
	}

	// event

	public override void ActivateWindowEvent() {
		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	public override void DisActivateWindowEvent() {
		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	public override void ChancheWindowEvent(int number) {
		//play sound button-click
		gameController.PlaySoundByIndex (3);
	}

	public override void ActivateConsoleWEvent ()
	{
		gameController.PauseGameWithDelay (0.2f);

		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	public override void DisActivateConsoleWEvent ()
	{
		// if out from menu in game
		gameController.ContinueGame ();

		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	public override void ChancheConsoleWEvent(int number) {

	}

	// main part

	public void OverUI() {
		overMenuUI = true;
	}

	public void OutUI() {
		overMenuUI = false;
	}

	public void GoInGame() {
		gameController.GoInGame_Button ();
	}

	public void ChangeDifficaltyPlayer(int val) {
		difficaltyPlayerVal = val;

		gameController.SetDifficaltyPlayer (val);

		if (didInit) {
			//button sound
			gameController.PlayButtonSound();

			SaveOptionsPrefs ();
		}
	}

	public void ChangeNamePlayer(string val) {
		namePlayerVal = val;

		gameController.SetNamePlayer (val);

		if (didInit) {
			if (val == "123456") {
				gameController.developState = true;
			} else {
				gameController.developState = false;
			}

			SaveOptionsPrefs ();
		}
	}

	public void SetNamePlayer(string val) {
		namePlayerVal = val;

		if (namePlayer) {
			namePlayer.text = val;
		}
	}

	public void ChangeMultiThreadVal(bool val) {
		multiThread = (val ? 1 : 0);

		gameController.useCoroutineMain = val;

		if (didInit) {
			//button sound
			gameController.PlayButtonSound();

			SaveOptionsPrefs ();
		}
	}

	public void ChangeLanguagePlayer(int val) {
		languagePlayerVal = val;

		string stLang = gameController.GetStLocalization (languagePlayerVal);

		gameController.SetLanguagePlayer (stLang);

		if (didInit) {
			SaveOptionsPrefs ();
		}
	}

	public void SetLanguagePlayer(int val) {
		languagePlayerVal = val;

		if (languagePlayer) {
			languagePlayer.value = val;
		}
	}

	public  void SetScorePlayer(string val) {
		if (scorePlayer != null) {
			scorePlayer.text = val;
		}
	}

	public void ProgressDelete() {
		gameController.ClearProgreesPlayer_Button ();
	}

	//console result upgrade
	public void ConsoleWinYesNo_ActionCloseGame() {
		ConsoleWinYesNo_SetTxt ("Exit game ?");
		ConsoleWinYesNo_SetYesAction (ExitGame);
	}

	public void ConsoleWinYesNo_ActionClearProgress() {
		ConsoleWinYesNo_SetTxt ("Confirm clear progress ?");
		ConsoleWinYesNo_SetYesAction (ProgressDelete);
	}

	//console
	public void ConsolWSmol_SetTecticInfo() {
		string stHead = "Tecnic information";

		// information
		string stInfo = "[t]Information Game";

		stInfo = stInfo + gameController.GetPlayerInfo ();
		stInfo = stInfo + gameController.GetCompleteLevelCountInfo ();

		// picture
		string resPict = "";

		ConsoleWinInformationSmol_SetInf (stHead, stInfo, resPict);
	}
}
