using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
	public AudioClip projectileHittingWallSounds;
	public List<AudioClip> themes;
	private const int audioSourceAmount = 20;
	private int counterCurrAudioSource = 0;
	private int counterCurrDamageAudioSource = 0;
	private int counterWallAudioSource = 0;
	private List<AudioSource> audioSources;
	private List<AudioSource> damageAudioSources;
	private AudioSource theme;
	private AudioSource projectileHittingWallAudioSource;
	private int previousPlayed;

	private void Awake() {
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}
	
	private void Start()
	{
		theme = gameObject.AddComponent<AudioSource>();
		previousPlayed = Random.Range(0,themes.Count);
		theme.clip = themes[previousPlayed];
		theme.volume = 0.1f;
		theme.Play();
		audioSources = new List<AudioSource>();
		damageAudioSources = new List<AudioSource>();
		for(int i = 0; i < audioSourceAmount; i++)
		{
			audioSources.Add(gameObject.AddComponent<AudioSource>());
			damageAudioSources.Add(gameObject.AddComponent<AudioSource>());
		}
		projectileHittingWallAudioSource = gameObject.AddComponent<AudioSource>();
	}
	void Update ()
     {
         if (!theme.isPlaying && !GameManager.instance.won)
         {
			int curr = Random.Range(0, themes.Count);
			if(previousPlayed != curr)
			{
				theme.clip = themes[curr];
			}
			else 
			{
				curr = (curr == 3 ? 0 : curr++);
				theme.clip = themes[curr];
			}
			theme.Play();
         }
     }
	public void PlaySounds(AudioClip sound)
	{
		if(counterCurrAudioSource >= 20)
		{
			counterCurrAudioSource = 0;
		}
		audioSources.ElementAt(counterCurrAudioSource).PlayOneShot(sound, 0.3f);

		counterCurrAudioSource++;
	}
	public void PlayDamageSounds(AudioClip sound, bool critHit)
	{
		if(counterCurrDamageAudioSource >= 20)
		{
			counterCurrDamageAudioSource = 0;
		}
		float freq = (critHit ? 0.5f : 0.3f);
		audioSources.ElementAt(counterCurrDamageAudioSource).PlayOneShot(sound, freq);
		counterCurrDamageAudioSource++;
	}

	public void PlayWallSound()
	{
		projectileHittingWallAudioSource.PlayOneShot(projectileHittingWallSounds, 0.1f);
	}
}
