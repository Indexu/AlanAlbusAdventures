using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveController : MonoBehaviour
{
    public VitalityController vc;
    public float reviveTime = 2.5f;

    public void Revive()
    {
        if (vc.isDead)
        {
            vc.Revive();
        }
    }

    private void Start()
    {
        vc = transform.parent.GetComponent<VitalityController>();
    }
}
