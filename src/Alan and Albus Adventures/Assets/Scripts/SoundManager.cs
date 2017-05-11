using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
	public List<AudioClip> ProjectileHittingWallSounds;
	public List<AudioClip> BlobShootingProjectilesSounds;
	private const int AudioSourceAmount = 20;
	private int counterCurrAudioSource = 0;
	private List<AudioSource> AudioSources;
	private AudioSource ProjectileHittingWallAudioSource;
	/*private List<AudioSource> BlobShootingProjectilesAudioSource;
	 
	private AudioSource slashAudioSource;
	private AudioSource hackNSlashAudioSource;
	private AudioSource critHackNSlashAudioSource;
	private AudioSource shootAudioSource;
	private AudioSource critShootAudioSource;

	//private List<AudioSource> hackNSlashAudioSource;
	//private List<AudioSource> critHackNSlashAudioSource;
	private List<AudioSource> deadBlobAudioSource;
	private List<AudioSource> damageBlobAudioSource;
	private AudioSource damagePlayerAudioSource;
	private AudioSource deadPlayerAudioSource;
*/

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
		for(int i = 0; i < AudioSourceAmount; i++)
		{
			AudioSources.Add(gameObject.AddComponent<AudioSource>());
		}
		ProjectileHittingWallAudioSource = gameObject.AddComponent<AudioSource>();
		//slashAudioSource = new List<AudioSource>();
		//slashAudioSource = gameObject.AddComponent<AudioSource>();
		//hackNSlashAudioSource = new List<AudioSource>();
		//hackNSlashAudioSource = gameObject.AddComponent<AudioSource>();
		//critHackNSlashAudioSource = new List<AudioSource>();
		/*critHackNSlashAudioSource = gameObject.AddComponent<AudioSource>();
		critShootAudioSource = gameObject.AddComponent<AudioSource>();
		shootAudioSource = gameObject.AddComponent<AudioSource>();
		
		deadBlobAudioSource = new List<AudioSource>();
		damageBlobAudioSource = new List<AudioSource>();
		damagePlayerAudioSource = gameObject.AddComponent<AudioSource>();
		deadPlayerAudioSource = gameObject.AddComponent<AudioSource>();
		BlobShootingProjectilesAudioSource = new List<AudioSource>();*/

		/*foreach (AudioClip clip in BlobShootingProjectilesSounds)
		{
			BlobShootingProjectilesAudioSource.Add(gameObject.AddComponent<AudioSource>());
			BlobShootingProjectilesAudioSource.Last().clip = clip;
		}*/
	}
	public void PlaySounds(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		AudioSources.ElementAt(counterCurrAudioSource).PlayOneShot(soundList.ElementAt(index), 0.2f);
		counterCurrAudioSource++;
		if(counterCurrAudioSource >= 20)
		{
			counterCurrAudioSource = 0;
		}
	}

	public void PlayWallSound()
	{
		int index = Random.Range(0, ProjectileHittingWallSounds.Count);
		ProjectileHittingWallAudioSource.PlayOneShot(ProjectileHittingWallSounds.ElementAt(index), 0.1f);
	}

	/*public void PlayProjectileBlobFireSound()
	{
		BlobShootingProjectilesAudioSource
		//(BlobShootingProjectilesAudioSource.ElementAt(index)).Play();
	} */

/*	public void PlaySlash(List<AudioClip> soundList)
	{
		int index = Random.Range(0, soundList.Count);
		AudioSources.PlayOneShot(soundList.ElementAt(index), 0.2f);
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

	public void PlayBlogDamage(List<AudioClip> soundList)
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

	public void PlayPlayerDamage(AudioClip sound)
	{
		damagePlayerAudioSource.PlayOneShot(sound, 0.1f);
	}
	public void PlayPlayerDeath(AudioClip sound)
	{
		deadPlayerAudioSource.PlayOneShot(sound, 0.2f);
	}

	public void PlayProjectileBlobFireSound(AudioSource source)
	{
		BlobShootingProjectilesAudioSource
		//(BlobShootingProjectilesAudioSource.ElementAt(index)).Play();
	}
*/

}
