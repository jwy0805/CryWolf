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
                switch (transform.parent.gameObject.name)
                {
                    case "Horror":
                        HorrorController horrorController = transform.parent.GetComponent<HorrorController>();
                        targetStat.ApplyingBuff(horrorController.PoisonStack ? 3f : 10f, 0.03f,
                            horrorController.PoisonStack ? Define.BuffList.DeadlyAddicted : Define.BuffList.Addicted);
                        break;
                    case "MosquitoStinger":
                        MosquitoStingerController controller =
                            transform.parent.GetComponent<MosquitoStingerController>();
                        targetStat.ApplyingBuff(10f, 0.03f, Define.BuffList.Addicted);
                        if (targetStat.CompareTag("Sheep"))
                        {
                            if (controller.Infection)
                            {
                                SheepController sheepController = targetStat.GetComponent<SheepController>();
                                sheepController.Infection = true;
                            }
                            if (controller.SheepDeath)
                            {
                                int num = Random.Range(0, 100);
                                if (num >= controller.DeathRate) targetStat.Hp = 1;
                            }
                        }
                        break;
                    default:
                        targetStat.ApplyingBuff(10f, 0.03f, Define.BuffList.Addicted);
                        break;
                }
                GetMp();
            }
            HitEffect();
        }
    }
}
