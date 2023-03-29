using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShellController : MonsterController
{
    private bool _roll = false;
    private float _mpTime;
    private float _crashTime;
    private float _rollCoolTime = 3f;
    private float _rollingSpeed;
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
        _stat.maxMp = 20;
        _stat.Mp = 0;
        _stat.Attack = 0;
        _stat.Skill = 20;
        _stat.Defense = 0;
        _stat.MoveSpeed = 5.0f;
        _stat.AttackRange = 2.5f;
        
        _mpTime = Time.time;
    }

    protected override void Update()
    {
        base.Update();
        
    }
    
    protected override void UpdateMoving()
    {
        if (_roll) State = Define.State.Rush;
        else base.UpdateMoving();
    }

    protected override void UpdateRush()
    {
        _stat.MoveSpeed = _rollingSpeed;
        if (Time.time > _lastTargetingTime + _targetingTime)
        {
            // Fence 안으로 들어갈 수 있는지?
            if (IsReachable(GameData.Center))
            {
                Tags = new[] { "Sheep", "Tower" };
                SetTarget(Tags);
            }
            // Fence 안으로 들어갈 수 없으면
            else
            {
                Tags = new[] { "Fence", "Tower" };
                SetTarget(Tags);
            }
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
        _navMesh.speed = _stat.MoveSpeed;
    }

    // 편의상 3초간 쿨타임을 Attack상태라고 치자.
    protected override void UpdateAttack()
    {
        if (Time.time < _crashTime + _rollCoolTime) return;
        State = Define.State.Idle;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!Tags.Contains(collision.gameObject.tag)) return;
        if (collision.gameObject.TryGetComponent(out Stat targetStat))
        {
            targetStat.OnSkilled(_stat);
            _crashTime = Time.time;
            // 충돌음 재생
        }
    }
}
