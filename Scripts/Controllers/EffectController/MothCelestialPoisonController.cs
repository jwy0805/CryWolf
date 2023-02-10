using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothCelestialPoisonController : ProjectileController
{
    protected override void UpdateAttack()
    {
        _destPos = _lockTarget.transform.position;
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.2f)
        {
            if (_lockTarget.TryGetComponent(out Stat targetStat))
            {
                targetStat.OnFaint();
                targetStat.OnAttakced(_stat);
                HitEffect();
            }

            if (_lockTarget.TryGetComponent(out BaseController baseController))
            {
                baseController.Condition = Define.Condition.Addicted;
            }

            GetMp();
        }
        else
        {
            float moveDist = Mathf.Clamp(speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;   
        }
    }
}
