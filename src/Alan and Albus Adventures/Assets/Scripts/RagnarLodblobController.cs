using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagnarLodblobController : Boss
{
    public float enrageDuration;
    public float enrageSpeedIncrease;
    public Color enrageColor;

    private const float firstWait = 5f;
    private bool startRoutine;
    private VitalityController vc;

    protected override void Start()
    {
        base.Start();
        startRoutine = true;
        vc = GetComponent<VitalityController>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (startRoutine && Attacking)
        {
            startRoutine = false;
            StartCoroutine(Enrage());
        }
    }

    protected override void Move()
    {
        targetVector.x = target.transform.position.x - transform.position.x;
        targetVector.y = target.transform.position.y - transform.position.y;

        rb2d.AddForce(targetVector.normalized * speed, ForceMode2D.Impulse);
    }

    private IEnumerator Enrage()
    {
        var originalColor = spriteRenderer.color;

        var hitParticleMain = vc.hitParticle.GetComponent<ParticleSystem>().main;
        var critParticleMain = vc.critParticle.GetComponent<ParticleSystem>().main;
        var deathParticleMain = vc.deathParticle.GetComponent<ParticleSystem>().main;

        var originalParticleColor = hitParticleMain.startColor;

        var psEnrageColor = new ParticleSystem.MinMaxGradient(enrageColor);

        yield return new WaitForSeconds(firstWait);

        while (true)
        {
            yield return new WaitForSeconds(enrageDuration);

            spriteRenderer.color = enrageColor;
            hitParticleMain.startColor = psEnrageColor;
            critParticleMain.startColor = psEnrageColor;
            deathParticleMain.startColor = psEnrageColor;
            speed += enrageSpeedIncrease;

            yield return new WaitForSeconds(enrageDuration);

            spriteRenderer.color = originalColor;
            hitParticleMain.startColor = originalParticleColor;
            critParticleMain.startColor = originalParticleColor;
            deathParticleMain.startColor = originalParticleColor;
            speed -= enrageSpeedIncrease;
        }
    }
}
