using UnityEngine;
using System.Collections;

public class AnimationOpenClose : MonoBehaviour {

	public bool hasHideAnimation = false;

	private Animator animator;

	void Awake () {
		
	}

	void Init () {
		if (!animator) {
			animator = GetComponent<Animator> ();

			if (animator.parameters.Length > 1) {
				hasHideAnimation = true;
			}
		}
	}

	public void Click () {
		if (IsOpen()) {
			Close ();
		}
		else {
			Open ();
		}
	}

	public void Open () {
		if (!IsOpen ()) {
			animator.SetBool ("open", true);
		}
	}

	public void Close () {
		if (IsOpen ()) {
			animator.SetBool ("open", false);
		}
	}

	public bool IsOpen() {
		if (!animator)
			Init ();
		
		return animator.GetBool ("open");
	}

	public void Hide() {
		if (hasHideAnimation) {
			animator.SetBool ("hide", true);
		}
	}

	public void Show() {
		if (hasHideAnimation) {
			animator.SetBool ("hide", false);
		}
	}

	public bool IsShow() {
		if (hasHideAnimation) {
			return animator.GetBool ("hide");
		} else {
			return true;
		}
	}
}
