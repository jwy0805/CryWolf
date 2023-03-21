using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeDummyController : TowerController
{
    private bool _aggro = false;
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
                case Define.Skill.PracticeDummyDefence:
                    _stat.Defense += 3;
                    break;
                case Define.Skill.PracticeDummyDefence2:
                    _stat.Defense += 4;
                    break;
                case Define.Skill.PracticeDummyHealth:
                    _stat.Hp += 40;
                    _stat.MaxHp += 40;
                    break;
                case Define.Skill.PracticeDummyHealth2:
                    _stat.Hp += 60;
                    _stat.MaxHp += 60;
                    break;
                case Define.Skill.PracticeDummyAggro:
                    _aggro = true;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        _stat.Hp = 200;
        _stat.MaxHp = 200;
        _stat.Mp = 0;
        _stat.maxMp = 30;
        _stat.Defense = 3;
        Dead = false;
        
        SkillInit();
    }

    protected override void UpdateIdle()
    {
        if (_aggro && _stat.Mp >= _stat.maxMp)
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
            stat.SetDebuffParams(5, 0, Define.Debuff.Aggro);
        }

        _stat.Mp = 0;
    }
    
    protected override void OnEndEvent()
    {
        State = Define.State.Idle;
    }
}
