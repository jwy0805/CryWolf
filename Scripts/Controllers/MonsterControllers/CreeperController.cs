using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class CreeperController : MonsterController
{
    private bool _rush;
    private bool _knockBack;
    private bool _poisonAttack = false;
    private bool _roll = true;
    private float _moveSpeed = 3.5f;
    private float _rollingSpeed = 8.0f;
    private Vector3 _dir;

    private bool KnockBack
    {
        get => _knockBack;
        set
        {
            _knockBack = value;
            if (_knockBack)
            {
                _destPos = -(_dir.normalized * 8.0f);
                _destPos.y = 6.0f;
                State = Define.State.KnockBack;
            }
        }
    }
    
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.CreeperAttack:
                    _stat.AttackSpeed += 0.15f;
                    break;
                case Define.Skill.CreeperPoison:
                    _poisonAttack = true;
                    break;
                case Define.Skill.CreeperSpeed:
                    _stat.MoveSpeed += 2.5f;
                    break;
                case Define.Skill.CreeperAttackSpeed:
                    _stat.Accuracy += 10;
                    break;
                case Define.Skill.CreeperRoll:
                    _roll = true;
                    break;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        MonsterId = Define.MonsterId.Creeper;

        _stat.Hp = 120;
        _stat.MaxHp = 120;
        _stat.Attack = 15;
        _stat.Skill = 32;
        _stat.AttackSpeed = 0.7f;
        _stat.Defense = 5;
        _stat.MoveSpeed = _moveSpeed;
        _stat.AttackRange = 3.0f;

        _rush = false;
        _knockBack = false;
    }

    protected override void UpdateMoving()
    {
        // if: 스킬 업그레이드 && 생성된 후 최초 1번
        if (_roll && _rush == false) State = Define.State.Rush;
        else base.UpdateMoving();
    }

    protected override void UpdateRush()
    {
        _stat.MoveSpeed = _rollingSpeed;
        _rush = true;
        // Targeting
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
        _dir = _destPos - transform.position;
        _navMesh.SetDestination(_destPos);
        _navMesh.speed = _stat.MoveSpeed;
    }

    protected override void UpdateKnockBack()
    {
        _navMesh.SetDestination(_destPos);
        _dir = _destPos - transform.position;
        if (_dir.magnitude < 0.1f) State = Define.State.Idle;
    }

    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            // 스킬 업그레이드
            Managers.Resource.Instanciate(_poisonAttack ? "Effects/PoisonAttack" : "Effects/BasicAttack",
                gameObject.transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Tags.Contains(collision.gameObject.tag) && State == Define.State.Rush)
        {
            if (collision.gameObject.TryGetComponent(out Stat targetStat))
            {
                targetStat.OnSkilled(_stat);
                KnockBack = true;
                // 충돌음 재생
            }
        }
        
        if (KnockBack && State == Define.State.Rush)
        {
            State = Define.State.Idle;
        }
    }
}
