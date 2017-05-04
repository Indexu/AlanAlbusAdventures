using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public AudioSource soundEffect;
	public AudioSource theme;
	public static SoundManager instance = null;

	private void Awake() {
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
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
