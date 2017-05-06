using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItem : Item
{
    public int charges;
    public int duration;

    public IEnumerator useItem()
    {
        charges--;
        yield return new WaitForSeconds(duration);
        if (charges <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
