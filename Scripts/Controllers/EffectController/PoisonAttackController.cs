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
                targetStat.OnAttakced(_stat);
                if (gameObject.name == "Horror")
                {
                    HorrorController horrorController = GetComponent<HorrorController>();
                    targetStat.SetDebuffParams(horrorController.PoisonStack ? 5 : 10, 0.03f,
                        horrorController.PoisonStack ? Define.Debuff.DeadlyAddicted : Define.Debuff.Addicted);
                }
                else
                {
                    targetStat.SetDebuffParams(10, 0.03f, Define.Debuff.Addicted);
                }
                GetMp();
            }
            HitEffect();
        }
    }
}
