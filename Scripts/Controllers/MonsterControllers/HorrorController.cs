using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorrorController : MonsterController
{
    private bool _rush;
    private bool _knockBack;
    private bool _rollPoison = false;
    private bool _poisonBelt = false;
    private readonly float _rollingSpeed = 8.0f;
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
                    _stat.Accuracy += 10;
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
    
    private bool KnockBack
    {
        get => _knockBack;
        set
        {
            _knockBack = value;
            if (_knockBack)
            {
                _destPos = -(_dir.normalized * 10.0f);
                _destPos.y = 6.0f;
                Debug.Log(_destPos);
                State = Define.State.KnockBackCreeper;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 600;
        _stat.MaxHp = 600;
        _stat.Attack = 60;
        _stat.AttackSpeed = 0.75f;
        _stat.Defense = 7;
        _stat.MoveSpeed = 4.0f;
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
        
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            if (targetStat.Targetable == false) return;

            _destPos = _lockTarget.transform.position;
        }
        else
        {
            if (Time.time > _lastTargetingTime + _targetingTime)
            {
                _lastTargetingTime = Time.time;

                string[] tags = { "Fence", "Tower" };
                SetTarget(tags);
            }
        }
        
        _dir= _destPos - transform.position;
        
        if (_dir.magnitude < 0.7f && KnockBack == false)
        {
            KnockBack = true;
        }
        else
        {
            _navMesh.SetDestination(_destPos);
            _navMesh.speed = _stat.MoveSpeed;
        }
    }

    protected override void UpdateKnockBackCreeper()
    {
        _navMesh.SetDestination(_destPos);
        _dir = _destPos - transform.position;
        State = Define.State.Idle;
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
        if (!Tags.Contains(collision.gameObject.tag)) return;
        KnockBack = true;
        if (collision.gameObject.TryGetComponent(out Stat targetStat))
        {
            // 충돌음 재생
            /* todo */
            targetStat.OnSkilled(_stat);
            if (_rollPoison)
            {
                
            }
        }
    }
}
