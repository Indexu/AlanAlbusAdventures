using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlobertController : Boss
{
    public float spawnInterval;
    public GameObject enemyHealthBar;
    public GameObject[] blobs;
    public AudioClip spawnSound;

    private const float firstWait = 3.5f;
    private bool startRoutine;

    protected override void Start()
    {
        base.Start();
        startRoutine = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (startRoutine && Attacking)
        {
            startRoutine = false;
            StartCoroutine(SpawnBlobs());
        }
    }

    protected override void Move()
    {
        targetVector.x = target.transform.position.x - transform.position.x;
        targetVector.y = target.transform.position.y - transform.position.y;

        rb2d.AddForce(targetVector.normalized * speed, ForceMode2D.Impulse);
    }

    private IEnumerator SpawnBlobs()
    {
        yield return new WaitForSeconds(firstWait);

        while (true)
        {
            var blob = blobs[Random.Range(0, blobs.Length)];

            var blobInstance = Instantiate(blob, transform.position, Quaternion.identity, transform.parent);
            var healthBar = Instantiate(enemyHealthBar, Vector3.zero, Quaternion.identity, GameManager.instance.canvas.transform);

            blobInstance.GetComponent<VitalityController>().healthSlider = healthBar.GetComponent<Slider>();
            blobInstance.GetComponent<Enemy>().Attacking = true;
            blobInstance.GetComponent<Enemy>().experienceValue = 0;

            GameManager.instance.EnemySpawned();
            SoundManager.instance.PlaySounds(spawnSound);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
