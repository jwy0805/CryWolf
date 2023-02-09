using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummyController : TowerController
{
    private bool _dead = false;
    private bool _faint = false;
    private bool _debuffRemove = false;
    private float _healParam = 0.1f;
    private float _radius = 4f;
    private Collider[] _colliders;

    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.TrainingDummyAggro:
                    _radius += 2f;
                    break;
                case Define.Skill.TrainingDummyDefence:
                    _stat.Defense += 6;
                    break;
                case Define.Skill.TrainingDummyHeal:
                    _healParam += 0.08f;
                    break;
                case Define.Skill.TrainingDummyHealth:
                    _stat.Hp += 200;
                    _stat.MaxHp += 200;
                    break;
                case Define.Skill.TrainingDummyFireResist:
                    _stat.FireResist += 10;
                    break;
                case Define.Skill.TrainingDummyPoisonResist:
                    _stat.PoisonResist += 10;
                    break;
                case Define.Skill.TrainingDummyFaint:
                    _faint = true;
                    break;
                case Define.Skill.TrainingDummyDebuffRemove:
                    _debuffRemove = true;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 600;
        _stat.MaxHp = 600;
        _stat.Mp = 0;
        _stat.maxMp = 30;
        _stat.Attack = 10;
        _stat.AttackRange = 1.5f;
        _stat.Defense = 16;
        _stat.FireResist = 15;
        _stat.PoisonResist = 15;
        _stat.AttackSpeed = 0.4f;
        _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);
        
        SkillInit();
    }
    
    protected override void UpdateIdle()
    {
        if (_stat.Mp >= _stat.maxMp)
        {
            State = Define.State.Skill;
        }
        
        if (_lockTarget != null)
        {
            float distance = (_lockTarget.transform.position - transform.position).magnitude;
            if (distance <= _stat.AttackRange)
            {
                State = Define.State.Attack;
            }
        }
        else
        {
            if (Time.time > _lastTargetingTime + _targetingTime)
            {
                _lastTargetingTime = Time.time;
                SetTarget(tags);
            }
        }
    }

    protected override void UpdateAttack()
    {
        if (_stat.Mp >= _stat.maxMp)
        {
            State = Define.State.Skill;
        }
        else
        {
            base.UpdateAttack();
        }
    }
    
    protected override void UpdateDie()
    {
        if (_dead) return;
        else
        {
            int length = _colliders.Length;
            for (int i = 0; i < length; i++)
            {
                GameObject go = _colliders[i].gameObject;
                BaseController baseController = go.GetComponent<BaseController>();
                baseController.Condition = Define.Condition.Good;
            }
            
            _dead = true;
        }
    }

    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            if (_faint)
            {
                targetStat.OnFaint();
            }      
            targetStat.OnAttakced(_stat);
        }
    }

    private void OnSkillEvent()
    {
        _colliders  = Physics.OverlapSphere(transform.position, _radius, (int)Define.Layer.Monsters);
        int length = _colliders.Length;
        for (int i = 0; i < length; i++)
        {
            GameObject go = _colliders[i].gameObject;
            BaseController baseController = go.GetComponent<BaseController>();
            baseController.Condition = Define.Condition.Aggro;
            baseController._lockTarget = gameObject;
        }

        if (_debuffRemove)
        {
            Condition = Define.Condition.Good;
        }
        
        _stat.Heal((int)(_stat.MaxHp * _healParam));
        _stat.Mp = 0;
    }
}
