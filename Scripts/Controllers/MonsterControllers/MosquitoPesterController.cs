using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosquitoPesterController : MonsterController
{
    private float _height = 8.0f;
    private bool _woolDown = false;
    private bool _woolRate = false;
    private bool _woolStop = false;
    
    protected override string NewSkill 
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.MosquitoPesterAttack:
                    _stat.Attack += 4;
                    break;
                case Define.Skill.MosquitoPesterHealth:
                    _stat.MaxHp += 25;
                    _stat.Hp += 25;
                    break;
                case Define.Skill.MosquitoPesterWoolDown2:
                    _woolDown = true;
                    break;
                case Define.Skill.MosquitoPesterWoolRate:
                    _woolRate = true;
                    break;
                case Define.Skill.MosquitoPesterWoolStop:
                    _woolStop = true;
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
        _stat.Hp = 75;
        _stat.MaxHp = 75;
        _stat.Mp = 20;
        _stat.maxMp = 20;
        _stat.Attack = 20;
        _stat.AttackSpeed = 0.85f;
        _stat.Defense = 0;
        _stat.MoveSpeed = 7.0f;
        _stat.AttackRange = 4.0f;
        _stat.Evasion = 10;
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

    protected override void UpdateAttack()
    {
        if (_stat.Mp >= 20)
        {
            _stat.Mp = 0;
            if (_woolDown || _woolRate) State = Define.State.Skill;
        }
        base.UpdateAttack();
    }

    private void OnSkillEvent()
    {
        if (_lockTarget.CompareTag("Sheep"))
        {
            if (_lockTarget.TryGetComponent(out SheepController sheepController))
            {
                if (_woolDown) sheepController.DecreaseParam = 0.3f;
                if (_woolRate) sheepController.InterruptParam = 30;
                else if (_woolStop) sheepController.InterruptParam = 100;
            }
        }
        else
        {
            OnHitEvent();
        }
    }
}
