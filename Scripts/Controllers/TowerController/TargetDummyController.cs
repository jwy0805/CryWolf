using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TargetDummyController : TowerController
{
    private bool _heal = false;
    private bool _dead = false;
    private Collider[] _colliders;
    
    public bool Dead
    {
        get => _dead;
        set
        {
            _dead = value;
            if (!_dead) return;
            int length = _colliders.Length;
            for (int i = 0; i < length; i++)
            {
                GameObject go = _colliders[i].gameObject;
                Stat stat = go.GetComponent<Stat>();
                stat.RemoveDebuff(Define.Debuff.Aggro);
            }
        }
    }

    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.TargetDummyHeal:
                    _heal = true;
                    break;
                case Define.Skill.TargetDummyHealth:
                    _stat.Hp += 100;
                    _stat.MaxHp += 100;
                    break;
                case Define.Skill.TargetDummyFireResist:
                    _stat.FireResist += 10;
                    break;
                case Define.Skill.TargetDummyPoisonResist:
                    _stat.PoisonResist += 10;
                    break;
                case Define.Skill.TargetDummyReflection:
                    _stat.Reflection = true;
                    _stat.ReflectionRate = 0.1f;
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
        _stat.maxMp = 30;
        _stat.Defense = 12;
        
        SkillInit();
    }
    
    protected override void UpdateIdle()
    {
        if (_stat.Mp >= _stat.maxMp)
        {
            State = Define.State.Skill;
        }
    }

    protected override void UpdateDie()
    {
        Dead = true;
    }
    
    private void OnSkillEvent()
    {
        _colliders  = Physics.OverlapSphere(transform.position, 4.0f, (int)Define.Layer.Monster);
        int length = _colliders.Length;
        for (int i = 0; i < length; i++)
        {
            GameObject go = _colliders[i].gameObject;
            BaseController baseController = go.GetComponent<BaseController>();
            baseController._lockTarget = gameObject;
            Stat stat = go.GetComponent<Stat>();
            stat.ApplyingBuff(5, 0, Define.BuffList.Aggro);
        }

        if (_heal)
        {
            _stat.Heal((int)(_stat.MaxHp * 0.1));
        }
        
        _stat.Mp = 0;
    }
    
    protected override void OnEndEvent()
    {
        State = Define.State.Idle;
    }
}
