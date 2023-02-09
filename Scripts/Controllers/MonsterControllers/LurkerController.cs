using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LurkerController : MonsterController
{
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 75;
        _stat.MaxHp = 75;
        _stat.Attack = 8;
        _stat.Defense = 0;
        _stat.MoveSpeed = 3.0f;
        _stat.AttackRange = 3.0f;
    }

    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Managers.Resource.Instanciate("Effects/BasicAttack", gameObject.transform);
        }
    }
}
