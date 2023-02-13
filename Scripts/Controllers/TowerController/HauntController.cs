using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntController : TowerController
{
    private int _mask = (1 << (int)Define.Layer.Ground);
    private bool _fire = false;
    private bool _longAttack = false;

    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.HauntLongAttack:
                    _longAttack = true;
                    _stat.AttackRange += 3.5f;
                    break;
                case Define.Skill.HauntAttackSpeed:
                    _stat.AttackSpeed += 0.15f;
                    break;
                case Define.Skill.HauntAttack:
                    _stat.Attack += 10;
                    break;
                case Define.Skill.HauntFireResist:
                    _stat.FireResist += 10;
                    break;
                case Define.Skill.HauntPoisonResist:
                    _stat.PoisonResist += 10;
                    break;
                case Define.Skill.HauntFire:
                    _stat.Attack += 20;
                    _fire = true;
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
        _stat.AttackSpeed = 0.9f;
        
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

    protected override void UpdateAttack()
    {
        // 스킬 업그레이드
        if (_longAttack)
        {
            _stat.AttackRange = 6.0f;
            State = Define.State.Skill;
        }
        else
        {
            base.UpdateAttack();
        }
    }

    protected override void UpdateSkill()
    {
        base.UpdateAttack();
    }

    private void OnSkillEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();

            if (targetStat.Hp > 0)
            {
                if (_fire)
                {
                    Managers.Resource.Instanciate("Effects/HauntFire", gameObject.transform);
                }
                else
                {
                    Managers.Resource.Instanciate("Effects/HauntAttack", gameObject.transform);
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
