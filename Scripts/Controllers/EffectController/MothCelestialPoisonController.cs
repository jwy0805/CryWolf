using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothCelestialPoisonController : ProjectileController
{
    protected override void OnTriggerEnter(Collider collider)
    {
        GameObject go = collider.gameObject;

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
                GetMp();
                targetStat.OnFaint();
                targetStat.OnAttakced(_stat);
                targetStat.SetDebuffParams(10, 0.05f, Define.Debuff.Addicted);
                HitEffect();
            }
        }
    }
}
