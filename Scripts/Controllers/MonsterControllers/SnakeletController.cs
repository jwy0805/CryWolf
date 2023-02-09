using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeletController : MonsterController
{
    protected override void Init()
    {
        base.Init();

        _stat.Hp = 36;
        _stat.MaxHp = 36;
        _stat.Attack = 15;
        _stat.Defense = 0;
        _stat.MoveSpeed = 3.0f;
        _stat.AttackRange = 6.0f;
    }

    protected override void OnHitEvent()
    {
        Managers.Resource.Instanciate("Effects/BasicAttack", gameObject.transform);
    }
}
