using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonsterController
{
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 100;
        _stat.MaxHp = 100;
        _stat.Attack = 25;
        _stat.Defense = 0;
        _stat.MoveSpeed = 5.0f;
        _stat.AttackRange = 1.5f;
    }
}
