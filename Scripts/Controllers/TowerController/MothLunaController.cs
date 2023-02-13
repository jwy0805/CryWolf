using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MothLunaController : TowerController
{
    private float _height = 8f;
    private bool _faint = false;
    private Bounds _fenceBounds = new Bounds(GameData.center, GameData.FenceSize[1]);
    
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.MothLunaAttack:
                    _stat.Attack += 3;
                    break;
                case Define.Skill.MothLunaAccuracy:
                    _stat.Accuracy += 5;
                    break;
                case Define.Skill.MothLunaFaint:
                    _faint = true;
                    break;
                case Define.Skill.MothLunaSpeed:
                    _stat.MoveSpeed += 2;
                    break;
            }
        }
    }

    public override bool Active
    {
        get => _active;
        set => _active = value;
    }
    
    protected override void Init()
    {
        _skillSubject = GameObject.Find("SkillSubject").GetComponent<SkillSubject>();
        _skillSubject.AddObserver(this);
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
        WorldObjectType = Define.WorldObject.Tower;
        Vector3 size = gameObject.GetComponent<Collider>().bounds.size;
        _stat = gameObject.GetComponent<Stat>();
        
        transform.position = GameData.center + Vector3.up * 2;
        State = Define.State.Idle;
        
        _stat.Hp = 100;
        _stat.MaxHp = 100;
        _stat.MoveSpeed = 1;
        _stat.Attack = 6;
        _stat.Defense = 0;
        _stat.AttackRange = 2;
        _stat.AttackSpeed = 0.7f;

        tags = new[] { "MonsterAir" };
        
        SkillInit(); 
    }

    protected override void UpdateIdle()
    {
        if (_lockTarget != null)
        {
            State = Define.State.Moving;
        }
        
        if (Time.time > _lastTargetingTime + _targetingTime)
        {
            _lastTargetingTime = Time.time;
            SetTarget(tags);
        }
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
        else
        {
            _destPos = GameData.center + Vector3.up * 2;
        }

        // Move
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, _height, pos.z);
        Vector3 dir= _destPos - pos;
        
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

    protected override void SetTarget(string[] tags)
    {
        float closestDist = 5000.0f;
        foreach (var tag in tags)
        {
            _tagged = GameObject.FindGameObjectsWithTag(tag);
            foreach (var tagged in _tagged)
            {
                Vector3 targetPos = tagged.transform.position;
                Stat stat = tagged.gameObject.GetComponent<Stat>();
                stat.enabled = true;
                bool targetable = stat.Targetable;
                float dist = (targetPos - transform.position).sqrMagnitude;
                if (dist < closestDist && targetable && _fenceBounds.Contains(targetPos))
                {
                    closestDist = dist;
                    _lockTarget = tagged;
                }
            }   
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
    
    protected override void OnEndEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();

            if (targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _stat.AttackRange)
                {
                    State = Define.State.Attack;
                }
                else
                {
                    _lockTarget = null;
                    State = Define.State.Moving;
                }
            }
            else
            {
                _lockTarget = null;
                State = Define.State.Moving;
            }
        }
        else
        {
            State = Define.State.Moving;
        }
    }
}
