using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveController : MonoBehaviour
{
    public AnimationCurve hitAnim;

    private VitalityController vc;
    private IEnumerator co;

    public void Hit()
    {
        co = HitAnim();
        StopCoroutine(co);
        StartCoroutine(co);
    }

    private void Start()
    {
        vc = GetComponent<VitalityController>();
    }

    private IEnumerator HitAnim()
    {
        if (hitAnim.length != 0)
        {
            var elapsed = 0f;
            var old = transform.localScale;

            var time = hitAnim.keys[hitAnim.length - 1].time;

            while (elapsed < time)
            {
                transform.localScale = new Vector3((hitAnim.Evaluate(elapsed) * -0.4f) + old.x, (hitAnim.Evaluate(elapsed) * 0.4f) + old.y, transform.localScale.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = old;
        }
    }
}
