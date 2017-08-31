using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayFieldManager_TicTacToe : BaseMenuController {
	public bool overMenuUI = false;

	public PointManager_TicTacToe[] pointList;

	public GameObject[] developList;

	// window Game
	public Text systemTime;
	public Text bonusPlayer;

	public Text countPlayedGame;
	public Text countWinGame;
	public Text countLoseGame;

	public Image panelPlayerImage;
	public Image panelAIImage;
	public Color defColorPanel;

	//reference for GameControl
	public GameController_TicTacToe gameController;

	[System.NonSerialized]
	public static PlayFieldManager_TicTacToe Instance;

	void Awake () {
		// activate instance
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	public override void RestoreOptionsPref() {
		didInit = false;

		if (panelPlayerImage) {
			defColorPanel = panelPlayerImage.color;
		}

		gameController = GameController_TicTacToe.Instance;
		gameController.SetPlayFieldManager (Instance);

		gameController.InitGamePool ();
		gameController.StartLevel ();

		// addition Prefub

		didInit = true;

		//hide develop objects
		if (!gameController.developState) {
			HideDevelopList ();
		}
	}

	// events

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

	public override void ActivateInformWEvent ()
	{
		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	public override void DisActivateInformWEvent ()
	{
		//play sound button-click
		gameController.PlaySoundByIndex (2);
	}

	// main part
	public void GoInMenu() {
		gameController.GoInMenu_Button ();
	}

	public void GoInNextLevel() {
		gameController.StartLevel_Button ();
	}

	public void SetPanelPlayerColor(Color val) {
		if (panelPlayerImage) {
			panelPlayerImage.color = val;
		}
	}

	public void SetPanelPlayerDefColor() {
		if (panelPlayerImage) {
			panelPlayerImage.color = defColorPanel;
		}
	}

	public void SetPanelAIColor(Color val) {
		if (panelAIImage) {
			panelAIImage.color = val;
		}
	}

	public void SetPanelAIDefColor() {
		if (panelAIImage) {
			panelAIImage.color = defColorPanel;
		}
	}

	public bool GetLockPanel(int numPanel) {
		return pointList [numPanel].GetLockState ();
	}

	public void SetLockPanel(int numPanel, bool val = true) {
		pointList [numPanel].SetLockState (val);
	}

	public string GetTextPanel(int numPanel) {
		return pointList [numPanel].GetTextPanel ();
	}

	public void SetTextPanel(int numPanel, string val) {
		pointList [numPanel].SetTextPanel (val);
	}

	public FontStyle GetTextStylePanel(int numPanel) {
		return pointList [numPanel].GetTextStylePanel ();
	}

	public void SetTextStylePanel(int numPanel, FontStyle val) {
		pointList [numPanel].SetTextStylePanel (val);
	}

	public Color GetTextColorPanel(int numPanel) {
		return pointList [numPanel].GetTextColorPanel ();
	}

	public void SetTextColorPanel(int numPanel, Color val) {
		pointList [numPanel].SetTextColorPanel (val);
	}

	public Color GetColorPanel(int numPanel) {
		return pointList [numPanel].GetColorPanel ();
	}

	public void SetColorPanel(int numPanel, Color val) {
		pointList [numPanel].SetColorPanel (val);
	}

	public void SetColorPanelDef(int numPanel) {
		pointList [numPanel].SetColorPanelDef ();
	}

	public void OverUI() {
		overMenuUI = true;
	}

	public void OutUI() {
		overMenuUI = false;
	}

	public void DoTestAction() {
		if (gameController) {
			gameController.LevelDoTestWork_Button ();
		}
	}

	private void HideDevelopList() {
		foreach (GameObject item in developList) {
			if (item) {
				if (item.activeSelf) {
					item.SetActive (false);
				}
			}
		}
	}

	private void ShowDevelopList() {
		foreach (GameObject item in developList) {
			if (item) {
				if (!item.activeSelf) {
					item.SetActive (true);
				}
			}
		}
	}

	public void SetSystemTime(string val) {
		if (systemTime != null) {
			systemTime.text = val;
		}
	}

	public void SetBonusPlayer(string val) {
		if (bonusPlayer != null) {
			bonusPlayer.text = val;
		}
	}

	public  void SetPlayedGame(string val) {
		if (countPlayedGame != null) {
			countPlayedGame.text = val;
		}
	}

	public  void SetWinGame(string val) {
		if (countWinGame != null) {
			countWinGame.text = val;
		}
	}

	public  void SetLoseGame(string val) {
		if (countLoseGame != null) {
			countLoseGame.text = val;
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
		ConsoleWinYesNo_SetTxt ("Confirm crear progress ?");
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
