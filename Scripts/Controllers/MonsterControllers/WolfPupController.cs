using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPupController : MonsterController
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
                case Define.Skill.WolfPupSpeed:
                    _stat.MoveSpeed = 5.0f;
                    break;
                case Define.Skill.WolfPupHealth:
                    _stat.Hp = 70;
                    _stat.MaxHp = 70;
                    break;
                case Define.Skill.WolfPupAttackSpeed:
                    _stat.AttackSpeed = 0.85f;
                    break;
                case Define.Skill.WolfPupAttack:
                    _stat.Attack = 15;
                    break;
            }
        }
    }


    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 50;
        _stat.MaxHp = 50;
        _stat.Attack = 10;
        _stat.Defense = 0;
        _stat.AttackSpeed = 0.75f;
        _stat.MoveSpeed = 4.0f;
        _stat.AttackRange = 1.5f;
        SkillInit();
    }
}
