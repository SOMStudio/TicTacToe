using UnityEngine;
using System.Collections;

public class SceneManager_TicTacToe : SceneManager {

	[System.NonSerialized]
	public static SceneManager_TicTacToe Instance;

	void Awake() {
		// activate instance
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}
}
