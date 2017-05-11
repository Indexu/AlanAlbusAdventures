using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
	public AudioClip projectileHittingWallSounds;
	public List<AudioClip> blobShootingProjectilesSounds;
	private const int audioSourceAmount = 20;
	private int counterCurrAudioSource = 0;
	private int counterCurrDamageAudioSource = 0;
	private List<AudioSource> audioSources;
	private List<AudioSource> damageAudioSources;
	private AudioSource projectileHittingWallAudioSource;

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
	
	private void Start()
	{
		audioSources = new List<AudioSource>();
		damageAudioSources = new List<AudioSource>();
		for(int i = 0; i < audioSourceAmount; i++)
		{
			audioSources.Add(gameObject.AddComponent<AudioSource>());
			damageAudioSources.Add(gameObject.AddComponent<AudioSource>());
		}
		projectileHittingWallAudioSource = gameObject.AddComponent<AudioSource>();
	}
	public void PlaySounds(AudioClip sound)
	{
		if(counterCurrAudioSource >= 20)
		{
			counterCurrAudioSource = 0;
		}
		audioSources.ElementAt(counterCurrAudioSource).PlayOneShot(sound, 0.2f);
		counterCurrAudioSource++;

	}
	public void PlayDamageSounds(AudioClip sound, bool critHit)
	{
		if(counterCurrDamageAudioSource >= 20)
		{
			counterCurrDamageAudioSource = 0;
		}
		float freq = (critHit ? 0.4f : 0.2f);
		audioSources.ElementAt(counterCurrDamageAudioSource).PlayOneShot(sound, freq);
		counterCurrDamageAudioSource++;
	}

	public void PlayWallSound()
	{
		projectileHittingWallAudioSource.PlayOneShot(projectileHittingWallSounds, 0.1f);
	}
}
