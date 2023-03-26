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

                    // targetStat.Test();

                    targetStat.ApplyingBuff(3, 0.03f, Define.BuffList.Addicted);

                    // targetStat.SetDebuffParams(horrorController.PoisonStack ? 5f : 10f, 0.03f,
                    //     horrorController.PoisonStack ? Define.Debuff.DeadlyAddicted : Define.Debuff.MoveSpeedDecrease);
                }
                else
                {
                    targetStat.SetDebuffParams(10f, 0.03f, Define.Debuff.Addicted);
                }
                GetMp();
            }
            HitEffect();
        }
    }
}
