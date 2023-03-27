using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoisonAttackController : ProjectileController
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
                // targetStat.OnAttakced(_stat);
                if (transform.parent.gameObject.name == "Horror")
                {
                    HorrorController horrorController = transform.parent.GetComponent<HorrorController>();
                    targetStat.ApplyingBuff(horrorController.PoisonStack ? 5f : 10f, 0.03f,
                        horrorController.PoisonStack ? Define.BuffList.DeadlyAddicted : Define.BuffList.Addicted);
                    // _stat.ApplyingBuff(5f, 1f, Define.BuffList.AttackIncrease);
                    // Debug.Log(_stat.Attack);
                }
                else
                {
                    targetStat.ApplyingBuff(10f, 0.03f, Define.BuffList.Addicted);
                }
                GetMp();
            }
            HitEffect();
        }
    }
}
