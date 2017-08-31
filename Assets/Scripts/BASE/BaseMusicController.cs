using UnityEngine;
using System.Collections.Generic;

public class BaseMusicController : MonoBehaviour {

	public List<MusicController> musicList;

	[System.NonSerialized]
	public static BaseMusicController Instance;

	public void Awake()
	{
		// activate instance
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	public void UpdateValume() {
		foreach (MusicController item in musicList) {
			item.UpdateVolume ();
		}
	}

	public void StopMusic(int num) {
		MusicController temp = musicList [num];

		if (temp) {
			temp.loopMusic = false;
			temp.FadeOut (15f);
		}
	}

	public void StopMusicButPlayToEnd(int num) {
		MusicController temp = musicList [num];

		if (temp) {
			temp.loopMusic = false;
		}
	}

	public void PlayMusic(int num) {
		MusicController temp = musicList [num];

		if (temp) {
			temp.loopMusic = true;
			temp.FadeIn (15f);
		}
	}

	//predefine

	public void PlayRememberMusic() {
		MusicController temp1 = musicList [0];

		if (temp1.IsPlaing () == true) {
			StopMusic (0);
		}

		PlayMusic (1);
	}

	public void PlayLevelMusic() {
		MusicController temp1 = musicList [1];

		if (temp1.IsPlaing () == true) {
			StopMusic (1);
		}

		PlayMusic (0);
	}
}
