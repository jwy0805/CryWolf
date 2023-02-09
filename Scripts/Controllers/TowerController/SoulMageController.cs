using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulMageController : TowerController
{
    private bool _skill = false;
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SoulMageAvoid:
                    _stat.Evasion = 20;
                    break;
                case Define.Skill.SoulMageDefenceAll:
                    break;
                case Define.Skill.SoulMageFireDamage:
                    break;
                case Define.Skill.SoulMageTornado:
                    _skill = true;
                    break;
                case Define.Skill.SoulMageShareDamage:
                    break;
                case Define.Skill.SoulMageNatureAttack:
                    break;
                case Define.Skill.SoulMageDebuffResist:
                    break;
                case Define.Skill.SoulMageCritical:
                    _stat.CriticalMultiplier = 1.75f;
                    _stat.CriticalChance = 20f;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        
        _destPos = SetDest();
        
        _stat.Hp = 700;
        _stat.MaxHp = 700;
        _stat.Mp = 0;
        _stat.maxMp = 20;
        _stat.Attack = 120;
        _stat.Skill = 200;
        _stat.Defense = 0;
        _stat.MoveSpeed = 6.0f;
        _stat.AttackRange = 5.0f;
        _stat.AttackSpeed = 1.0f;
        _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);
        
        SkillInit();
    }

    protected override void UpdateIdle()
    {
        State = Define.State.Moving;
    }
    
    protected override void UpdateMoving()
    {
        // Targeting
        if (Time.time > _lastTargetingTime + _targetingTime && _lockTarget == null)
        {
            SetTarget(tags);
        }
        
        // Attack
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Collider targetCollider = _lockTarget.GetComponent<Collider>();
            Vector3 position = transform.position;
            
            if (targetStat.Targetable == false)
                return;
        
            _destPos = targetCollider.ClosestPoint(position);
            float distance = (_destPos - position).magnitude;
            if (distance < _stat.AttackRange)
            {
                State = Define.State.Attack;
                return;
            }
        }
        
        // Move
        Vector3 dir= _destPos - transform.position;

        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    protected override void UpdateAttack()
    {
        if (_skill)
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
                Managers.Resource.Instanciate("Effects/HauntAttack", gameObject.transform);
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
                Managers.Resource.Instanciate("Effects/SoulMagePunch", gameObject.transform);
            }
        }
    }

    protected override void OnEndEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Collider targetCollider = _lockTarget.GetComponent<Collider>();
            Vector3 position = transform.position;
            
            if (targetStat.Hp > 0)
            {
                float distance = (targetCollider.ClosestPoint(position) - position).magnitude;
                if (distance <= _stat.AttackRange)
                {
                    State = Define.State.Attack;
                }
                else
                {
                    State = Define.State.Moving;
                }
            }
            else
            {
                _lockTarget = null;
                State = Define.State.Idle;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}
