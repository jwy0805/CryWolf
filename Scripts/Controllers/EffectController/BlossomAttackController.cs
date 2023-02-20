using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlossomAttackController : ProjectileController
{
    private BlossomController _blossomController;
    protected override void Init()
    {
        base.Init();
        _blossomController = GetComponentInParent<BlossomController>();
    }

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
                if (_blossomController._blossomDeath)
                {
                    var random = new System.Random();
                    int randVal = random.Next(100);
                    if (randVal < _blossomController._deadParameter)
                    {
                        targetStat.Hp = 0;
                        targetStat.OnDead();
                        HitEffect();
                    }
                    else
                    {
                        targetStat.OnAttakced(_stat);
                        HitEffect();
                    }
                }
                else
                {
                    targetStat.OnAttakced(_stat);
                    HitEffect();
                }
            }
        }
    }
}
