using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoisonBeltController : MonoBehaviour
{
    private BaseController _baseController;
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _baseController = GetComponentInParent<BaseController>();
    }
    
    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Tower") && !other.CompareTag("Sheep")) return;
        if (other.TryGetComponent(out Stat targetStat))
        {
            targetStat.ApplyingBuff(5, 0.1f, Define.BuffList.Addicted);
        }
    }
}
