using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonsterController
{
    private bool _fire = false;
    
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SnakeAttackSpeed:
                    _stat.AttackSpeed += 0.15f;
                    break;
                case Define.Skill.SnakeAttack:
                    _stat.Attack += 10;
                    break;
                case Define.Skill.SnakeRange:
                    _stat.AttackRange += 2;
                    break;
                case Define.Skill.SnakeAccuracy:
                    _stat.Accuracy += 10;
                    break;
                case Define.Skill.SnakeFire:
                    _fire = true;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 100;
        _stat.MaxHp = 100;
        _stat.Attack = 25;
        _stat.AttackSpeed = 0.85f;
        _stat.Defense = 0;
        _stat.MoveSpeed = 6.0f;
        _stat.AttackRange = 8.0f;
    }
    
    protected override void OnHitEvent()
    {
        // 스킬 업그레이드
        Managers.Resource.Instanciate(_fire ? "Effects/SmallFire" : "Effects/BasicAttack", gameObject.transform);
    }
}
