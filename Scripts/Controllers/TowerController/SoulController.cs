using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoulController : TowerController
{
    private int _mask = (1 << (int)Define.Layer.Ground);
    private bool _drain = false;

    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SoulAttack:
                    _stat.Attack = 28;
                    break;
                case Define.Skill.SoulDefence:
                    _stat.Defense = 5;
                    break;
                case Define.Skill.SoulHealth:
                    _stat.MaxHp = 80;
                    break;
                case Define.Skill.SoulDrain:
                    _drain = true;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        
        _destPos = SetDest();
        
        _stat.Hp = 60;
        _stat.MaxHp = 60;
        _stat.Mp = 20;
        _stat.maxMp = 20;
        _stat.Attack = 20;
        _stat.Defense = 0;
        _stat.MoveSpeed = 4.0f;
        _stat.AttackRange = 1.5f;
        _stat.AttackSpeed = 0.75f;
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
            RaycastHit hit;
            float moveDist = Mathf.Clamp(5.0f * Time.deltaTime, 0, dir.magnitude);
            Vector3 rayStart = transform.position + Vector3.up;
            Vector3 rayDir = (transform.position + dir.normalized * moveDist) - rayStart; 
            bool raycastHit = Physics.Raycast(rayStart, rayDir, out hit, 70f, _mask);

            if (raycastHit)
            {
                transform.position = hit.point;
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
        }
    }

    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttakced(_stat);
            if (_drain)
            {
                int recoverHp = ((_stat.Attack - targetStat.Defense)/4);
                if (_stat.Hp + recoverHp <= 60)
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
