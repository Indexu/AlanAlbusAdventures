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
    private ParticleSystem.MinMaxGradient originalParticleColor;
    private ParticleSystem.MinMaxGradient psEnrageColor;
    private ParticleSystem.MainModule hitParticleMain;
    private ParticleSystem.MainModule critParticleMain;
    private ParticleSystem.MainModule deathParticleMain;
    private Color originalColor;

    protected override void Start()
    {
        base.Start();
        startRoutine = true;
        vc = GetComponent<VitalityController>();
        originalParticleColor = vc.hitParticle.GetComponent<ParticleSystem>().main.startColor;
        hitParticleMain = vc.hitParticle.GetComponent<ParticleSystem>().main;
        critParticleMain = vc.critParticle.GetComponent<ParticleSystem>().main;
        deathParticleMain = vc.deathParticle.GetComponent<ParticleSystem>().main;
        psEnrageColor = new ParticleSystem.MinMaxGradient(enrageColor);
        originalColor = spriteRenderer.color;
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

    private void OnDestroy()
    {
        Debug.Log("DESTROY");
        spriteRenderer.color = originalColor;
        hitParticleMain.startColor = originalParticleColor;
        critParticleMain.startColor = originalParticleColor;
        deathParticleMain.startColor = originalParticleColor;
    }

    private IEnumerator Enrage()
    {
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
