using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveController : MonoBehaviour
{
    public AnimationCurve hitAnim;

    private IEnumerator co;
    private Vector3 originalScale;
    private float hitMagnitute = 0.1f;

    public void Hit()
    {
        co = HitAnim();
        StopCoroutine(co);
        transform.localScale = originalScale;
        StartCoroutine(co);
    }

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private IEnumerator HitAnim()
    {
        if (hitAnim.length != 0)
        {
            var elapsed = 0f;

            var time = hitAnim.keys[hitAnim.length - 1].time;

            while (elapsed < time)
            {
                transform.localScale = new Vector3((hitAnim.Evaluate(elapsed) * -hitMagnitute) + originalScale.x, (hitAnim.Evaluate(elapsed) * hitMagnitute) + originalScale.y, originalScale.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;
        }
    }
}
