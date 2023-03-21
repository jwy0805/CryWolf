using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LurkerController : MonsterController
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
                case Define.Skill.LurkerDefence:
                    _stat.Defense += 4;
                    break;
                case Define.Skill.LurkerHealth:
                    _stat.MaxHp += 25;
                    _stat.Hp += 25;
                    break;
                case Define.Skill.LurkerHealth2:
                    _stat.MaxHp += 20;
                    _stat.Hp += 20;
                    break;
                case Define.Skill.LurkerSpeed:
                    _stat.MoveSpeed += 2f;
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 75;
        _stat.MaxHp = 75;
        _stat.Attack = 8;
        _stat.AttackSpeed = 0.6f;
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
