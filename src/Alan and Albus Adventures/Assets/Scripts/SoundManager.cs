using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
	public AudioClip ProjectileHittingWallSounds;
	public List<AudioClip> BlobShootingProjectilesSounds;
	private const int AudioSourceAmount = 20;
	private int counterCurrAudioSource = 0;
	private List<AudioSource> AudioSources;
	private AudioSource ProjectileHittingWallAudioSource;

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
		AudioSources = new List<AudioSource>();
		for(int i = 0; i < AudioSourceAmount; i++)
		{
			AudioSources.Add(gameObject.AddComponent<AudioSource>());
		}
		ProjectileHittingWallAudioSource = gameObject.AddComponent<AudioSource>();
	}
	public void PlaySounds(AudioClip sound)
	{
		if(counterCurrAudioSource >= 20)
		{
			counterCurrAudioSource = 0;
		}
		AudioSources.ElementAt(counterCurrAudioSource).PlayOneShot(sound, 0.2f);
		counterCurrAudioSource++;

	}

	public void PlayWallSound()
	{
		//int index = Random.Range(0, ProjectileHittingWallSounds.Count);
		//ProjectileHittingWallAudioSource.PlayOneShot(ProjectileHittingWallSounds.ElementAt(index), 0.1f);
		ProjectileHittingWallAudioSource.PlayOneShot(ProjectileHittingWallSounds, 0.1f);
	}
}
