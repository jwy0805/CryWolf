using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunBlossomController : TowerController
{
    private float _mpTime = 1f;
    private float _lastMpTime = 0f;
    private bool _heal = false;
    private bool _health = false;
    private bool _slow = false;
    private bool _slowAttack = false;
    
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SunBlossomHeal:
                    _heal = true;
                    break;
                case Define.Skill.SunBlossomHealth:
                    _health = true;
                    break;
                case Define.Skill.SunBlossomSlow:
                    _slow = true;
                    break;
                case Define.Skill.SunBlossomSlowAttack:
                    _slowAttack = true;
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();

        _stat.Hp = 80;
        _stat.MaxHp = 80;
        _stat.Mp = 0;
        _stat.maxMp = 40;
        _stat.Defense = 0;
        _stat.AttackRange = 10;

        SkillInit();
    }

    protected override void UpdateIdle()
    {
        if (Time.deltaTime > _lastMpTime + _mpTime)
        {
            _stat.Mp += 4;
        }

        if (_stat.Mp >= _stat.maxMp)
        {
            State = Define.State.Skill;
        }
    }

    private void OnSkillEvent()
    {
        
    }

    protected override void OnEndEvent()
    {
        State = Define.State.Idle;
    }
}
