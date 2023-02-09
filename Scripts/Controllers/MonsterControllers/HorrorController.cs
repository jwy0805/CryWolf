using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorController : MonsterController
{
    private bool _rush;
    private bool _knockBack;
    private float _rollingSpeed = 7.0f;
    private Vector3 _dir;
    
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
        _stat.Defense = 0;
        _stat.MoveSpeed = 4.0f;
        _stat.AttackRange = 3.0f;
    }
    
    protected override void UpdateMoving()
    {
        // if: 스킬 업그레이드 && 생성된 후 최초 1번
        if (true && _rush == false)
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

        if (_dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
    }
    
    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Managers.Resource.Instanciate("Effects/PoisonAttack", gameObject.transform);
        }
    }
}
