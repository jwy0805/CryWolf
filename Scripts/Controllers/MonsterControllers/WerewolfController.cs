using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfController : MonsterController
{
    private int _count = 0;
    private bool _thunder = false;
    private bool _debuffResist = false;
    private bool _faint = false;
    private bool _enhance = false;
    
    private float _attackSpeedParam;
    private float _drainParam;

    public bool DebuffResist => _debuffResist;

    protected override string NewSkill
    {
        get => NewSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.WerewolfThunder:
                    _thunder = true;
                    break;
                case Define.Skill.WerewolfDebuffResist:
                    _debuffResist = true;
                    break;
                case Define.Skill.WerewolfFaint:
                    _faint = true;
                    break;
                case Define.Skill.WerewolfHealth:
                    _stat.MaxHp += 250;
                    _stat.Hp += 250;
                    break;
                case Define.Skill.WerewolfEnhance:
                    _enhance = true;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 400;
        _stat.MaxHp = 400;
        _stat.Mp = 0;
        _stat.maxMp = 0;
        _stat.Attack = 100;
        _stat.AttackSpeed = 1f;
        _stat.Skill = 3;
        _stat.Defense = 5;
        _stat.MoveSpeed = 7.0f;
        _stat.AttackRange = 2.5f;
    }
    
    protected override void UpdateAttack()
    {
        // 스킬 업그레이드 완료
        if (_thunder)
            State = _count % 2 == 0 ? Define.State.Skill : Define.State.Skill2;
        else base.UpdateAttack();
    }

    private void AddAttackSpeed()
    {
        if (!_enhance) return;
        _attackSpeedParam = (1f - (float)_stat.Hp / _stat.MaxHp) * 0.6f;
        _stat.AttackSpeed += _attackSpeedParam;
    }

    private void RemoveAttackSpeed()
    {
        if (!_enhance) return;
        _stat.AttackSpeed -= _attackSpeedParam;
    }
    
    protected override void UpdateDie()
    {
        _count = 0;
        base.UpdateDie();
    }
    
    protected override void UpdateSkill()
    {
        base.UpdateAttack();
    }

    private void OnSkillStart()
    {
        AddAttackSpeed();    
    }

    protected override void OnHitEvent()
    {
        if (_lockTarget == null) return;
        Stat targetStat = _lockTarget.GetComponent<Stat>();
        targetStat.OnAttakced(_stat);
        
        if (_playerController != null) _playerController.Resource += 6;
        
        // drain 승계
        int recoverHp = 0;
        if (_enhance)
        {
            float drainParam = _drainParam + _drainParam * (1f - (float)_stat.Hp / _stat.MaxHp);
            recoverHp = (int)((_stat.Attack - targetStat.Defense) * drainParam);
        }
        else
        {
            recoverHp = (int)((_stat.Attack - targetStat.Defense) * _drainParam);
        }
        
        if (_stat.Hp + recoverHp <= _stat.MaxHp) _stat.Hp += recoverHp;
        else _stat.Hp = _stat.MaxHp;
    }
    
    private void OnSkillEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            GameObject go = Managers.Resource.Instanciate("Effects/Lightning Strike");
            if (_faint) targetStat.OnFaint();
            targetStat.OnSkilled(_stat);
            go.transform.position = _lockTarget.transform.position;
            _count++;
        }
    }

    protected override void OnEndEvent()
    {
        RemoveAttackSpeed();
        base.OnEndEvent();
    }
}
