using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public AudioSource soundEffect;
	public static SoundManager instance { get; private set; }

	void Awake() {
		instance = this;
	}
	
	// Update is called once per frame
	public void PlaySingle(AudioClip sound)
	{
		soundEffect.clip = sound;
		soundEffect.Play();
	}
}
