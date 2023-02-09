using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPupController : MonsterController
{
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 50;
        _stat.MaxHp = 50;
        _stat.Attack = 10;
        _stat.Defense = 0;
        _stat.MoveSpeed = 4.0f;
        _stat.AttackRange = 1.5f;
    }
}
