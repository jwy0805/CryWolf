using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFireController : ProjectileController
{
    protected override void OnTriggerEnter(Collider collider)
    {
        Debug.Log("fe");
        GameObject go = collider.gameObject;

        if (go == null || _lockTarget == null)
        {
            HitEffect();
            return; 
        }
        if (!go.CompareTag(_lockTarget.tag))
        {
            if (go.CompareTag("Terrain"))
            {
                HitEffect();
            }
        }
        else
        {            
            if (go.TryGetComponent(out Stat targetStat))
            {
                if (!targetStat.Targetable) return;
                targetStat.OnAttakced(_stat);
                GetMp();
                HitEffect();
            }
        }
    }
}
