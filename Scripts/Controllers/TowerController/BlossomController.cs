using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class BlossomController : TowerController
{
    public bool _blossomPoison = false;
    public bool _blossomDeath = false;
    public int _deadParameter = 3;
    
    protected override string NewSkill 
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.BlossomAttack:
                    _stat.Attack += 30;
                    break;
                case Define.Skill.BlossomAccuracy:
                    _stat.Accuracy = 130;
                    break;
                case Define.Skill.BlossomRange:
                    _stat.AttackRange = 12.0f;
                    break;
                case Define.Skill.BlossomAttackSpeed:
                    _stat.AttackSpeed = 1.25f;
                    break;
                case Define.Skill.BlossomPoison:
                    _blossomPoison = true;
                    break;
                case Define.Skill.BlossomDeath:
                    _blossomDeath = true;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        _stat.Hp = 400;
        _stat.MaxHp = 400;
        _stat.Mp = 20;
        _stat.maxMp = 20;
        _stat.Attack = 70;
        _stat.Defense = 0;
        _stat.AttackRange = 9.0f;
        _stat.AttackSpeed = 0.9f;
        SkillInit();
    }

    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();

            if (targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _stat.AttackRange)
                {
                    if (_blossomPoison)
                    {
                        Managers.Resource.Instanciate("Effects/BlossomAttack", gameObject.transform);
                    }
                    else
                    {
                        Managers.Resource.Instanciate("Effects/BlossomSeed", gameObject.transform);
                    }
                }
            }
        }
    }
}
