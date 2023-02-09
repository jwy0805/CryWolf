using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BudController : TowerController
{
    private bool _seed = false;
    private bool _double = false;

    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.BudAttack:
                    _stat.Attack = 15;
                    break;
                case Define.Skill.BudAttackSpeed:
                    _stat.AttackSpeed = 1f;
                    _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);
                    break;
                case Define.Skill.BudRange:
                    _stat.AttackRange = 6;
                    break;
                case Define.Skill.BudSeed:
                    _seed = true;
                    break;
                case Define.Skill.BudDouble:
                    _stat.Attack = 12;
                    _stat.AttackSpeed = 0.9f;
                    _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);
                    _double = true;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        _stat.Hp = 120;
        _stat.MaxHp = 120;
        _stat.Mp = 20;
        _stat.maxMp = 20;
        _stat.Attack = 10;
        _stat.Skill = 20;
        _stat.Defense = 0;
        _stat.AttackRange = 4.0f;
        _stat.AttackSpeed = 0.75f;
        _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);
        SkillInit();
    }

    protected override void UpdateAttack()
    {
        // 스킬 업그레이드 
        if (_seed)
        {
            State = Define.State.Skill;
        }
        else
        {
            base.UpdateAttack();
        }  
    }

    private void OnSkillEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();

            if (targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _stat.AttackRange)
                {
                    Managers.Resource.Instanciate("Effects/Seed", gameObject.transform);
                }
            }
        }
    }

    private void OnDoubleEvent()
    {
        if (_double == false) return;
        OnSkillEvent();
    }
}
