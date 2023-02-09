using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonsterController
{
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 100;
        _stat.MaxHp = 100;
        _stat.Attack = 25;
        _stat.Defense = 0;
        _stat.MoveSpeed = 5.0f;
        _stat.AttackRange = 8.0f;
    }
    
    protected override void OnHitEvent()
    {
        // 스킬 업그레이드
        if (true)
        {
            Managers.Resource.Instanciate("Effects/SmallFire", gameObject.transform);
        }
        else
        {
            Managers.Resource.Instanciate("Effects/BasicAttack", gameObject.transform);
        }              
    }
}
