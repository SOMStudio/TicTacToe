using UnityEngine;
using System.Collections;

public class ContentCorrectionPos : MonoBehaviour {

	private Transform transformActive;

	void Awake () {
		transformActive = GetComponent<Transform> ();
	}

	void Start () {
		if (transformActive.localPosition.y > 1) {
			transformActive.localPosition = new Vector2 (transformActive.localPosition.x, 0);
		}
	}
}
