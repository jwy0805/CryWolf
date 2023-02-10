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

    protected override void UpdateAttack()
    {
        _destPos = _lockTarget.transform.position;
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.2f)
        {
            if (_lockTarget.TryGetComponent(out Stat targetStat))
            {
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
        else
        {
            float moveDist = Mathf.Clamp(speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;   
        }
    }
}
