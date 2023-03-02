using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNagaController : MonsterController
{
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 120;
        _stat.MaxHp = 120;
        _stat.Mp = 0;
        _stat.maxMp = 20;
        _stat.Attack = 100;
        _stat.Defense = 0;
        _stat.Skill = 250;
        _stat.MoveSpeed = 5.0f;
        _stat.AttackRange = 20.0f;
    }

    protected override void UpdateAttack()
    {
        if (_stat.Mp >= 20)
        {
            _stat.Mp = 0;
            State = Define.State.Skill;
        }
        else
        {
            base.UpdateAttack();
        }
    }
    
    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Managers.Resource.Instanciate("Effects/BigFire", gameObject.transform);
        }
    }

    private void OnSkillEvent()
    {
        GameObject meteor = Managers.Resource.Instanciate("Effects/Meteor", gameObject.transform);
        meteor.transform.position = GameData.Center;
    }
}
