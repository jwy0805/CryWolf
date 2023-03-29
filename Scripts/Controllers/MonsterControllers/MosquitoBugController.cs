using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosquitoBugController : MonsterController
{
    private float _height = 8.0f;
    private bool _woolDown = true;

    protected override string NewSkill 
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.MosquitoBugAvoid:
                    _stat.Evasion += 10;
                    break;
                case Define.Skill.MosquitoBugDefence:
                    _stat.Defense += 2;
                    break;
                case Define.Skill.MosquitoBugSpeed:
                    _stat.MoveSpeed += 1f;
                    break;
                case Define.Skill.MosquitoBugWoolDown:
                    _woolDown = true;
                    break;
            }
        } 
    }

    protected override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;
        
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        _stat = gameObject.GetComponent<Stat>();
        _stat.Hp = 20;
        _stat.MaxHp = 20;
        _stat.Attack = 3;
        _stat.AttackSpeed = 0.75f;
        _stat.Defense = 0;
        _stat.MoveSpeed = 5.0f;
        _stat.AttackRange = 2.0f;
    }

    protected override void UpdateIdle()
    {
        if (_lockTarget == null || !GameData.FenceBounds.Contains(transform.position))
        {
            string[] tags = { "Sheep" };
            SetTarget(tags);
            State = Define.State.Moving;
        }
    }

    protected override void UpdateMoving()
    {
        // Targeting
        if (Time.time > _lastTargetingTime + _targetingTime)
        {
            _lastTargetingTime = Time.time;
            
            if (GameData.FenceBounds.Contains(transform.position))
            {
                string[] tags = { "Sheep" };
                SetTarget(tags);
            }
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
        transform.position = new Vector3(transform.position.x, _height, transform.position.z);
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
    
    protected override void OnHitEvent()
    {
        if (_lockTarget == null) return;
        Stat targetStat = _lockTarget.GetComponent<Stat>();
        targetStat.OnAttakced(_stat);
        if (_lockTarget.CompareTag("Sheep") && _woolDown)
        {
            if (_lockTarget.TryGetComponent(out SheepController sheepController))
            {
                sheepController.DecreaseParam = 0.1f;
            }
        }

        if (_playerController != null) _playerController.Resource += _stat.Resource;
    }
}
