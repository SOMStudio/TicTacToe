using UnityEngine;
using UnityEngine.UI;

public class GameMenuController_TicTacToe : BaseMenuController {
	[Header("Settings TicTacToe")]
	[SerializeField]
	private bool overMenuUI = false;

	[SerializeField]
	private string namePlayerVal = "Anonim";
	[SerializeField]
	private InputField namePlayer;

	[SerializeField]
	private int languagePlayerVal = 0;
	[SerializeField]
	private Dropdown languagePlayer;

	[SerializeField]
	private int difficaltyPlayerVal = 0;
	[SerializeField]
	private Toggle[] difficultyToggleList;

	[SerializeField]
	private int multiThread = 1;
	[SerializeField]
	private Toggle multiThreadToggle;

	[SerializeField]
	private Text versionText;

	// window Arcade
	[SerializeField]
	private Text scorePlayer;

	//reference for GameControl
	[SerializeField]
	private GameController_TicTacToe gameController;

	[System.NonSerialized]
	public static GameMenuController_TicTacToe Instance;

	// main event
	void Awake () {
		// activate instance
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	// main logic
	protected override void RestoreOptionsPref() {
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

	protected override void SaveOptionsPrefs ()
	{
		base.SaveOptionsPrefs ();

		// addition Prefub
		PlayerPrefs.SetInt(gamePrefsName + "_Difficalty", difficaltyPlayerVal);

		PlayerPrefs.SetInt(gamePrefsName + "_MultiThread", multiThread);

		gameController.UpdateSoundVolume ();
		gameController.UpdateMusicVolume ();
	}

	protected override void ExitGame ()
	{
		SaveOptionsPrefs ();

		gameController.SaveDataBeforeExit ();

		gameController.PlayButtonSound ();

		base.ExitGame ();
	}

	#region event
	protected override void ActivateWindowEvent() {
		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	protected override void DisActivateWindowEvent() {
		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	protected override void ChancheWindowEvent(int number) {
		//play sound button-click
		gameController.PlaySoundByIndex (3);
	}

	protected override void ActivateConsoleWEvent ()
	{
		gameController.PauseGameWithDelay (0.2f);

		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	protected override void DisActivateConsoleWEvent ()
	{
		// if out from menu in game
		gameController.ContinueGame ();

		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	protected override void ChancheConsoleWEvent(int number) {

	}
	#endregion

	// main part
	public bool OverMenuUI {
		get { return overMenuUI; }
	}

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
				gameController.DevelopState = true;
			} else {
				gameController.DevelopState = false;
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

		gameController.UseCoroutineMain = val;

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
		stInfo = stInfo + gameController.GetPlayedLevelCountInfo ();
		stInfo = stInfo + gameController.GetWinLevelCountInfo ();

		// picture
		string resPict = "";

		ConsoleWinInformationSmol_SetInf (stHead, stInfo, resPict);
	}
}
