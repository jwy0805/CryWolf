using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeletController : MonsterController
{
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SnakeletAttack:
                    _stat.Attack = 21;
                    break;
                case Define.Skill.SnakeletAttackSpeed:
                    _stat.AttackSpeed = 0.85f;
                    break;
                case Define.Skill.SnakeletRange:
                    _stat.AttackRange += 2f;
                    break;
                case Define.Skill.SnakeletSpeed:
                    _stat.MoveSpeed += 2f;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();

        _stat.Hp = 36;
        _stat.MaxHp = 36;
        _stat.Attack = 15;
        _stat.AttackSpeed = 0.75f;
        _stat.Defense = 0;
        _stat.MoveSpeed = 3.0f;
        _stat.AttackRange = 6.0f;
    }

    protected override void OnHitEvent()
    {
        Managers.Resource.Instanciate("Effects/BasicAttack", gameObject.transform);
    }
}
