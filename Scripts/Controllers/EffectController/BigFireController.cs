using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BigFireController : ProjectileController
{
    private SnakeNagaController _snakeNagaController;
    private readonly float _drainParam = 0.25f;
    protected override void Init()
    {
        base.Init();
        _snakeNagaController = transform.parent.GetComponent<SnakeNagaController>();
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
        // hit판정
        else
        {            
            if (go.TryGetComponent(out Stat targetStat))
            {
                if (!targetStat.Targetable) HitEffect();
                targetStat.OnAttakced(_stat);
                if (_snakeNagaController.Drain)
                {
                    int recoverHp = (int)((_stat.Attack - targetStat.Defense) * _drainParam);
                    if (_stat.Hp + recoverHp <= _stat.MaxHp)
                    {
                        _stat.Hp += recoverHp;
                    }
                    else
                    {
                        _stat.Hp = _stat.MaxHp;
                    }
                }
                GetMp();
            }
            HitEffect();
        }
    }
}
