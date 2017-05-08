using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{

    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (ps && !ps.IsAlive())
        {
            GameObject.Destroy(gameObject);
        }
    }
}
