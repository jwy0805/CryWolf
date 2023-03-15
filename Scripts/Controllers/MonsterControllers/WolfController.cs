using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonsterController
{
    private bool _drain = false;
    private float _drainParam = 0.25f;
    private bool _dna = false;

    protected override string NewSkill
    {
        get =>  _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.WolfDefence:
                    _stat.Defense += 4;
                    break;
                case Define.Skill.WolfDrain:
                    _drain = true;
                    break;
                case Define.Skill.WolfAvoid:
                    _stat.Evasion = 10;
                    break;
                case Define.Skill.WolfFireResist:
                    _stat.FireResist = 10;
                    break;
                case Define.Skill.WolfPoisonResist:
                    _stat.PoisonResist = 10;
                    break;
                case Define.Skill.WolfDna:
                    _dna = true;
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
        _stat.Defense = 0;
        _stat.MoveSpeed = 5.0f;
        _stat.AttackRange = 1.5f;
    }

    protected override void OnHitEvent()
    {
        if (_lockTarget == null) return;
        Stat targetStat = _lockTarget.GetComponent<Stat>();
        targetStat.OnAttakced(_stat);
        
        if (_playerController != null) _playerController.Resource += 6;
        
        if (_drain)
        {
            int recoverHp = (int)((_stat.Attack - targetStat.Defense) * _drainParam);
            if (_stat.Hp + recoverHp <= _stat.MaxHp)
            {
                _stat.Hp += recoverHp;
            }
            else
            {
                _stat.Hp = _stat.MaxHp;
            }
        }
    }
}
