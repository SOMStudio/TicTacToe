using UnityEngine;

public class AnimationOpenClose : MonoBehaviour
{
	[SerializeField] private bool hasHideAnimation = false;

	private static readonly int OpenAnim = Animator.StringToHash("open");
	private static readonly int HideAnim = Animator.StringToHash("hide");

	private Animator animator;
	
	private void Awake()
	{
		animator = GetComponent<Animator>();

		if (animator.parameters.Length > 1)
		{
			hasHideAnimation = true;
		}
	}
	
	public void Click()
	{
		if (IsOpen())
		{
			Close();
		}
		else
		{
			Open();
		}
	}

	public void Open()
	{
		if (!IsOpen())
		{
			animator.SetBool(OpenAnim, true);
		}
	}

	public void Close()
	{
		if (IsOpen())
		{
			animator.SetBool(OpenAnim, false);
		}
	}

	public bool IsOpen()
	{
		return animator.GetBool(OpenAnim);
	}

	public void Hide()
	{
		if (hasHideAnimation)
		{
			animator.SetBool(HideAnim, true);
		}
	}

	public void Show()
	{
		if (hasHideAnimation)
		{
			animator.SetBool(HideAnim, false);
		}
	}

	public bool IsShow()
	{
		if (hasHideAnimation)
		{
			return animator.GetBool(HideAnim);
		}
		else
		{
			return true;
		}
	}
}
