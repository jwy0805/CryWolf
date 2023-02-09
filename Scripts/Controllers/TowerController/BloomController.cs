using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BloomController : TowerController
{
    private bool _skill3Combo = false;

    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);
            
            switch (skill)
            {
                case Define.Skill.BloomAttack:
                    _stat.Attack = 40;
                    break;
                case Define.Skill.BloomRange:
                    _stat.AttackRange = 8f;
                    break;
                case Define.Skill.BloomAttackSpeed:
                    _stat.AttackSpeed = 1f;
                    _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);
                    break;
                case Define.Skill.BloomAttackSpeed2:
                    _stat.AttackSpeed = 1.25f;
                    _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);
                    break;
                case Define.Skill.Bloom3Combo:
                    _stat.Attack = 24;
                    _skill3Combo = true;
                    break;
                case Define.Skill.BloomAirAttack:
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();
        _stat.Hp = 250;
        _stat.MaxHp = 250;
        _stat.Mp = 20;
        _stat.maxMp = 20;
        _stat.Attack = 30;
        _stat.Defense = 0;
        _stat.AttackRange = 6.0f;
        _stat.AttackSpeed = 0.85f;
        _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);
        SkillInit();
    }
    
    protected override void UpdateAttack()
    {
        if (_skill3Combo)
        {
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
}
