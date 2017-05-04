using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public AudioSource soundEffect;
	public AudioSource theme;
	private static SoundManager instance = null;

	void Awake() {
		if(instance = null)
		{
			instance = this;
		}

		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	public void PlaySingle(AudioClip sound)
	{
		soundEffect.clip = sound;
		soundEffect.Play();
	}
}
