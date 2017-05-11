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
	public List<AudioClip> ProjectileHittingWallSounds;
	private AudioSource ProjectileHittingWallAudioSource; 
	private AudioSource slashAudioSource;
	private AudioSource hackNSlashAudioSource;
	private AudioSource critHackNSlashAudioSource;
	private AudioSource shootAudioSource;
	private AudioSource critShootAudioSource;

	//private List<AudioSource> hackNSlashAudioSource;
	//private List<AudioSource> critHackNSlashAudioSource;
	private List<AudioSource> deadBlobAudioSource;
	private List<AudioSource> damageBlobAudioSource;


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
		critShootAudioSource = gameObject.AddComponent<AudioSource>();
		shootAudioSource = gameObject.AddComponent<AudioSource>();
		ProjectileHittingWallAudioSource = gameObject.AddComponent<AudioSource>();
		deadBlobAudioSource = new List<AudioSource>();
		damageBlobAudioSource = new List<AudioSource>();
	}

	public void PlaySlash(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		slashAudioSource.PlayOneShot(soundList.ElementAt(index), 0.2f);
	}

	public void PlayHackNSlash(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		hackNSlashAudioSource.PlayOneShot(soundList.ElementAt(index), 0.2f);
	}

	public void PlayCritHackNSlash(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		critHackNSlashAudioSource.PlayOneShot(soundList.ElementAt(index), 0.2f);
	}

	public void PlayShoot(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		shootAudioSource.PlayOneShot(soundList.ElementAt(index), 0.2f);
	}

	public void PlayCritShoot(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		critShootAudioSource.PlayOneShot(soundList.ElementAt(index), 0.2f);
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

	public void PlayBlobDamage(List<AudioClip> soundList)
	{
		if(!damageBlobAudioSource.Any())
		{
			foreach (AudioClip clip in soundList)
			{
				damageBlobAudioSource.Add(gameObject.AddComponent<AudioSource>());
				damageBlobAudioSource.Last().clip = clip;
			}
		}
		int index = Random.Range(0, soundList.Count);
		(damageBlobAudioSource.ElementAt(index)).Play();
	}
	public void PlayWallSound()
	{
		int index = Random.Range(0, ProjectileHittingWallSounds.Count);
		critShootAudioSource.PlayOneShot(ProjectileHittingWallSounds.ElementAt(index), 0.1f);
	}
}
