using UnityEngine;
using UnityEngine.UI;

public class PointManager_TicTacToe : MonoBehaviour
{
	[Header("Settings TicTacToe")]
	[SerializeField] private int numPanel;

	[SerializeField] private Image imagePanel;
	[SerializeField] private Text textPanel;
	[SerializeField] private Button buttonPanel;

	[SerializeField] private bool locked;

	private Color colorPanelDefault;

	[SerializeField] private GameController_TicTacToe gameController;
	
	private void Start()
	{
		if (!gameController)
		{
			gameController = GameController_TicTacToe.Instance;
		}

		colorPanelDefault = imagePanel.color;
	}
	
	public void SetLockState(bool val = true)
	{
		locked = val;
	}

	public bool GetLockState()
	{
		return locked;
	}

	public void SetColorPanel(Color val)
	{
		if (imagePanel != null)
		{
			if (imagePanel.color != val)
			{
				imagePanel.color = val;
			}
		}
		else
		{

		}
	}

	public Color GetColorPanel()
	{
		if (imagePanel != null)
		{
			return imagePanel.color;
		}
		else
		{
			return Color.black;
		}
	}

	public void SetColorPanelDef()
	{
		SetColorPanel(colorPanelDefault);
	}

	public void SetTextPanel(string stVal)
	{
		if (textPanel != null)
		{
			textPanel.text = stVal;
		}
		else
		{

		}
	}

	public string GetTextPanel()
	{
		if (textPanel != null)
		{
			return textPanel.text;
		}
		else
		{
			return "";
		}
	}

	public FontStyle GetTextStylePanel()
	{
		if (textPanel != null)
		{
			return textPanel.fontStyle;
		}
		else
		{
			return FontStyle.Normal;
		}
	}

	public void SetTextStylePanel(FontStyle val)
	{
		if (textPanel != null)
		{
			textPanel.fontStyle = val;
		}
		else
		{

		}
	}

	public Color GetTextColorPanel()
	{
		if (textPanel != null)
		{
			return textPanel.color;
		}
		else
		{
			return Color.black;
		}
	}

	public void SetTextColorPanel(Color val)
	{
		if (textPanel != null)
		{
			textPanel.color = val;
		}
	}

	public void ClickOnPanel()
	{
		gameController.ClickOnPanelGamePlayer_Button(numPanel);
	}
}
