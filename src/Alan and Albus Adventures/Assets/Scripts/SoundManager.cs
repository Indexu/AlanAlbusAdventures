using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public AudioSource soundEffect;

	void Awake() {
		//here we will start the theme
	}
	
	// Update is called once per frame
	public void PlaySingle(AudioClip sound)
	{
		soundEffect.clip = sound;
		soundEffect.Play();
	}
}
