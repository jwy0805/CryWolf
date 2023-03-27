using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MothCelestialPoisonController : ProjectileController
{
    protected override void OnTriggerEnter(Collider collider)
    {
        GameObject go = collider.gameObject;

        if (go == null || _lockTarget == null)
        {
            Managers.Resource.Destroy(gameObject);
            return; 
        }
        
        if (!_baseController.Tags.Contains(go.tag))
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
                if (!targetStat.Targetable) HitEffect();
                targetStat.OnFaint();
                targetStat.OnAttakced(_stat);
                targetStat.ApplyingBuff(10, 0.05f, Define.BuffList.Addicted);
                GetMp();
            }
            HitEffect();
        }
    }
}
