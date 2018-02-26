using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

	public Slider volume;
	public AudioSource music;

	void Awake() {
		DontDestroyOnLoad (this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		music.volume = volume.value;
	}

}
