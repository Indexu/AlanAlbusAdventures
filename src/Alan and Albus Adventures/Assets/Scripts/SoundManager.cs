using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    //public AudioSource soundEffect;
	//public AudioSource theme;
	//private List<AudioSource> slashAudioSource;
	private AudioSource slashAudioSource;
	private AudioSource hackNSlashAudioSource;
	private AudioSource critHackNSlashAudioSource;
	//private List<AudioSource> hackNSlashAudioSource;
	//private List<AudioSource> critHackNSlashAudioSource;
	private List<AudioSource> deadBlobAudioSource;

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
		//slashAudioSource = new List<AudioSource>();
		slashAudioSource = gameObject.AddComponent<AudioSource>();
		//hackNSlashAudioSource = new List<AudioSource>();
		hackNSlashAudioSource = gameObject.AddComponent<AudioSource>();
		//critHackNSlashAudioSource = new List<AudioSource>();
		critHackNSlashAudioSource = gameObject.AddComponent<AudioSource>();
		deadBlobAudioSource = new List<AudioSource>();
	}

	public void PlaySlash(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		slashAudioSource.PlayOneShot(soundList.ElementAt(index), 0.2f);
		/*if(!slashAudioSource.Any())
		{
			foreach (AudioClip clip in soundList)
			{
				Debug.Log("Here");
				slashAudioSource.Add(gameObject.AddComponent<AudioSource>());
				slashAudioSource.Last().clip = clip;
				slashAudioSource.Last().volume = 0.2f;
			}
		}
		int index = Random.Range(0, soundList.Count);
		(slashAudioSource.ElementAt(index)).Play();*/
	}

	public void PlayHackNSlash(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		hackNSlashAudioSource.PlayOneShot(soundList.ElementAt(index), 0.2f);
		/*if(!hackNSlashAudioSource.Any())
		{
			foreach (AudioClip clip in soundList)
			{
				hackNSlashAudioSource.Add(gameObject.AddComponent<AudioSource>());
				hackNSlashAudioSource.Last().clip = clip;
				hackNSlashAudioSource.Last().volume = 0.2f;
			}
		}
		int index = Random.Range(0, soundList.Count);
		(hackNSlashAudioSource.ElementAt(index)).Play();*/
	}

	public void PlayCritHackNSlash(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		critHackNSlashAudioSource.PlayOneShot(soundList.ElementAt(index), 0.4f);
		/*if(!critHackNSlashAudioSource.Any())
		{
			foreach (AudioClip clip in soundList)
			{
				critHackNSlashAudioSource.Add(gameObject.AddComponent<AudioSource>());
				critHackNSlashAudioSource.Last().clip = clip;
				critHackNSlashAudioSource.Last().volume = 0.2f;
			}
		}
		int index = Random.Range(0, soundList.Count);
		(critHackNSlashAudioSource.ElementAt(index)).Play();*/
	}

	public void PlayBlobDeath(List<AudioClip> soundList)
	{
		if(!deadBlobAudioSource.Any())
		{
			foreach (AudioClip clip in soundList)
			{
				deadBlobAudioSource.Add(gameObject.AddComponent<AudioSource>());
				deadBlobAudioSource.Last().clip = clip;
			}
		}
		int index = Random.Range(0, soundList.Count);
		(deadBlobAudioSource.ElementAt(index)).Play();
	}
}
