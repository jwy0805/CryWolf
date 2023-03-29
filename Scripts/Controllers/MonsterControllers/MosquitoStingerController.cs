using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosquitoStingerController : MonsterController
{
    private float _height = 8.0f;
    private bool _longAttack = false;
    private bool _poison = false;
    private bool _sheepDeath = false;
    private bool _infection = false;
    private int _deathRate = 0;
    public bool SheepDeath => _sheepDeath;
    public bool Infection => _infection;
    public float DeathRate => _deathRate;

    protected override string NewSkill 
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.MosquitoStingerAvoid:
                    _stat.Evasion += 15;
                    break;
                case Define.Skill.MosquitoStingerHealth:
                    _stat.MaxHp += 60;
                    _stat.Hp += 60;
                    break;
                case Define.Skill.MosquitoStingerLongAttack:
                    _longAttack = true;
                    _stat.AttackRange += 3f;
                    break;
                case Define.Skill.MosquitoStingerPoison:
                    _poison = true;
                    break;
                case Define.Skill.MosquitoStingerPoisonResist:
                    _stat.PoisonResist = 20;
                    break;
                case Define.Skill.MosquitoStingerSheepDeath:
                    _sheepDeath = true;
                    _deathRate = 30;
                    break;
                case Define.Skill.MosquitoStingerInfection:
                    _infection = true;
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
        _stat.Hp = 200;
        _stat.MaxHp = 200;
        _stat.Mp = 20;
        _stat.maxMp = 20;
        _stat.Attack = 80;
        _stat.AttackSpeed = 1f;
        _stat.Defense = 0;
        _stat.MoveSpeed = 7.0f;
        _stat.AttackRange = 3.0f;
        _stat.Evasion = 15;
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
        if (_longAttack) State = Define.State.Skill;
        else base.UpdateAttack();
    }

    private void OnSkillEvent()
    {
        Managers.Resource.Instanciate(_poison ? "Effects/BasicAttack" : "Effects/PoisonAttack", gameObject.transform);
    }
}
