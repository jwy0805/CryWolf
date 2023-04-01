using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShellController : MonsterController
{
    private bool _roll = false;
    private float _mpTime = 1f;
    private float _lastMpTime = 0f;
    private float _crashTime;
    private float _rollCoolTime = 3f;
    private float _rollingSpeed;
    private bool _start = false;
    private bool _speedBuff = false;
    private bool _attackSpeedBuff = false;
    private Vector3 _dir;
    
    protected override string NewSkill 
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.ShellHealth:
                    _stat.MaxHp += 35;
                    _stat.Hp += 35;
                    break;
                case Define.Skill.ShellSpeed:
                    _speedBuff = true;
                    break;
                case Define.Skill.ShellAttackSpeed:
                    _attackSpeedBuff = true;
                    break;
                case Define.Skill.ShellRoll:
                    _roll = true;
                    break;
            }
        } 
    }

    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 100;
        _stat.MaxHp = 100;
        _stat.maxMp = 40;
        _stat.Mp = 0;
        _stat.Attack = 0;
        _stat.Skill = 20;
        _stat.Defense = 0;
        _stat.MoveSpeed = 4.0f;
        _stat.AttackRange = 2.5f;
        _stat.CriticalChance = 0;
        
        _lastMpTime = Time.time;
        _crashTime = 0f;
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time > _lastMpTime + _mpTime)
        {
            _stat.Mp += 4;
            Debug.Log(_stat.Mp);
            _lastMpTime = Time.time;
        }
    }

    protected override void UpdateIdle()
    {
        if (Time.time < _crashTime + _rollCoolTime && _start) return;
        _start = true;
        if (_stat.Mp > _stat.maxMp) State = Define.State.Skill;
        base.UpdateIdle();
    }
    
    protected override void UpdateMoving()
    {
        if (_stat.Mp > _stat.maxMp) State = Define.State.Skill;
        if (_roll) State = Define.State.Rush;
        else
        {
            // Targeting
            if (Time.time > _lastTargetingTime + _targetingTime)
            {
                _lastTargetingTime = Time.time;
                // 이미 Fence 내부에 있으면
                if (GameData.FenceBounds.Contains(transform.position))
                {
                    Tags = new[] { "Sheep", "Tower" };
                }
                else
                {
                    // Fence 안으로 들어갈 수 있는지?
                    Tags = IsReachable(GameData.Center) ? new[] { "Sheep", "Tower" } :
                        // Fence 안으로 들어갈 수 없으면
                        new[] { "Fence", "Tower" };
                }
                SetTarget(Tags);
            }
        
            // Attack
            if (_lockTarget != null)
            {
                Stat targetStat = _lockTarget.GetComponent<Stat>();
                Collider targetCollider = _lockTarget.GetComponent<Collider>();
                Vector3 position = transform.position;
                if (targetStat.Targetable == false) return;
            
                _destPos = targetCollider.ClosestPoint(position);
                float distance = (_destPos - position).magnitude;
                if (distance < _stat.AttackRange)
                {
                    _navMesh.SetDestination(transform.position);
                    State = Define.State.Idle;
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
                _navMesh.SetDestination(_destPos);
                _navMesh.speed = _stat.MoveSpeed;
            }
        }
    }

    protected override void UpdateRush()
    {
        _rollingSpeed = _stat.MoveSpeed + 1.0f;
        
        if (_stat.Mp > _stat.maxMp) State = Define.State.Skill;
        if (Time.time > _lastTargetingTime + _targetingTime)
        {
            // Fence 안으로 들어갈 수 있는지?
            Tags = IsReachable(GameData.Center) ? new[] { "Sheep", "Tower" } :
                // Fence 안으로 들어갈 수 없으면
                new[] { "Fence", "Tower" };
            SetTarget(Tags);
        }
        
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            if (targetStat.Targetable == false) return;
            _destPos = _lockTarget.transform.position;
        }

        // Move
        _dir= _destPos - transform.position;
        _navMesh.SetDestination(_destPos);
        _navMesh.speed = _rollingSpeed;
    }

    protected override void UpdateKnockBack()
    {
        base.UpdateKnockBack();
    }

    protected override void UpdateSkill()
    {
        _destPos = transform.position;
        _navMesh.SetDestination(_destPos);
    }

    private void OnSkillEvent()
    {
        _stat.Mp = 0;

        float height = 6f;
        Vector3 pos = transform.position;
        Vector3 pos1 = new Vector3(pos.x, pos.y - height, pos.z);
        Vector3 pos2 = new Vector3(pos.x, pos.y + height, pos.z);
        Collider[] colliders = Physics.OverlapCapsule(pos1, pos2, 20, 1 << (int)Define.Layer.Monster);
        Debug.Log(colliders.Length);
        
        GameObject nearestMonster = null;
        float closestDist = 5000.0f;
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 monsterPos = colliders[i].transform.position;
            if (colliders[i].gameObject.name == "Shell") continue;
            float dist = (monsterPos - transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                nearestMonster = colliders[i].gameObject;
            }
        }

        if (nearestMonster == null) return;
        Stat stat = nearestMonster.GetComponent<Stat>();
        if (_speedBuff) stat.ApplyingBuff(10, 2.0f, Define.BuffList.MoveSpeedIncrease);
        if (_attackSpeedBuff) stat.ApplyingBuff(10, 0.1f, Define.BuffList.AttackSpeedIncrease);
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
                State = distance <= _stat.AttackRange ? Define.State.Idle : Define.State.Moving;
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
    
    private void OnCollisionEnter(Collision collision)
    {
        if (Tags.Contains(collision.gameObject.tag) && _roll)
        {
            if (collision.gameObject.TryGetComponent(out Stat targetStat))
            {
                targetStat.OnSkilled(_stat);
                _stat.Mp += 4;
                _crashTime = Time.time;
                State = Define.State.Idle;
                // 충돌음 재생
            }
        }
    }
}
