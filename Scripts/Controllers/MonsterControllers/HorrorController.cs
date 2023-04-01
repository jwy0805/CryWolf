using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorrorController : MonsterController
{
    private bool _rush;
    private bool _crash = false;
    private bool _knockBack;
    private bool _rollPoison = false;
    private bool _poisonStack = true;
    private bool _poisonBelt = false;
    private readonly float _rollingSpeed = 8.0f;
    private Vector3 _dir;
    
    private bool KnockBack
    {
        get => _knockBack;
        set
        {
            _knockBack = value;
            if (_knockBack)
            {
                _destPos = -(_dir.normalized * 5.0f);
                _destPos.y = 6.0f;
                State = Define.State.KnockBack;
            }
        }
    }

    public bool PoisonStack
    {
        get => _poisonStack;
        set => _poisonStack = value;
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
                case Define.Skill.HorrorHealth:
                    _stat.MaxHp += 200;
                    _stat.Hp += 200;
                    break;
                case Define.Skill.HorrorDefence:
                    _stat.Defense += 5;
                    break;
                case Define.Skill.HorrorPoisonResist:
                    _stat.PoisonResist += 15;
                    break;
                case Define.Skill.HorrorPoisonStack:
                    PoisonStack = true;
                    break;
                case Define.Skill.HorrorRollPoison:
                    _rollPoison = true;
                    break;
                case Define.Skill.HorrorPoisonBelt:
                    _poisonBelt = true;
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 600;
        _stat.MaxHp = 600;
        _stat.maxMp = 10;
        _stat.Mp = 0;
        _stat.Attack = 6;
        _stat.Skill = 0;
        _stat.AttackSpeed = 0.75f;
        _stat.Defense = 7;
        _stat.MoveSpeed = 5.0f;
        _stat.AttackRange = 3.0f;
    }
    
    protected override void UpdateMoving()
    {
        if (_rush == false)
        {
            State = Define.State.Rush;
        }
        else
        {
            base.UpdateMoving();
        }
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
        _dir= _destPos - transform.position;
        _navMesh.SetDestination(_destPos);
        _navMesh.speed = _stat.MoveSpeed;
    }

    protected override void UpdateKnockBack()
    {
        _navMesh.SetDestination(_destPos);
        _dir = _destPos - transform.position;
        if (_dir.magnitude < 0.5f) State = Define.State.Idle;
    }

    protected override void UpdateAttack()
    {
        if (_stat.Mp >= _stat.maxMp)
        { 
            _stat.Mp = 0;
            if (_poisonBelt) Managers.Resource.Instanciate("Effects/PoisonExplosion", gameObject.transform);
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
            Managers.Resource.Instanciate("Effects/PoisonAttack", gameObject.transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Tags.Contains(collision.gameObject.tag) || _crash) return;
        // KnockBack = true;
        if (collision.gameObject.TryGetComponent(out Stat targetStat))
        {
            // 충돌음 재생
            /* todo */
            targetStat.OnSkilled(_stat);
            if (_rollPoison)
            {
                targetStat.ApplyingBuff(_poisonStack? 5 : 10, 0.03f,
                    _poisonStack ? Define.BuffList.DeadlyAddicted : Define.BuffList.Addicted);
            }
        }

        _crash = true;
        State = Define.State.Idle;
    }
}
