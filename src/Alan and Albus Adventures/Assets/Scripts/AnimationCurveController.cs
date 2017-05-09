using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveController : MonoBehaviour
{
    public AnimationCurve hitAnim;

    private VitalityController vc;
    private IEnumerator co;
    private Vector3 originalScale;

    public void Hit()
    {
        co = HitAnim();
        StopCoroutine(co);
        transform.localScale = originalScale;
        StartCoroutine(co);
    }

    private void Start()
    {
        vc = GetComponent<VitalityController>();
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
                transform.localScale = new Vector3((hitAnim.Evaluate(elapsed) * -0.4f) + originalScale.x, (hitAnim.Evaluate(elapsed) * 0.4f) + originalScale.y, originalScale.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;
        }
    }
}
