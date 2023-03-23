using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                if (!targetStat.Targetable) return;
                if (_blossomController._blossomDeath)
                {
                    var random = new System.Random();
                    int randVal = random.Next(100);
                    if (_blossomController._blossomPoison)
                    {
                        targetStat.SetDebuffParams(10, 0.03f, Define.Debuff.Addicted);
                    }
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
