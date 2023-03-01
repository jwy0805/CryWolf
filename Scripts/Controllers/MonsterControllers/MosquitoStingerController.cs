using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosquitoStingerController : MonsterController
{
    private float _height = 8.0f;
    
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
        _stat.Defense = 0;
        _stat.MoveSpeed = 7.0f;
        _stat.AttackRange = 4.0f;
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
        base.UpdateAttack();
    }

    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Collider targetCollider = _lockTarget.GetComponent<Collider>();
            Vector3 position = transform.position;
            
            if (targetStat.Hp > 0)
            {
                float distance = (targetCollider.ClosestPoint(position) - position).magnitude;
                if (distance <= _stat.AttackRange)
                {
                    // 스킬 업그레이드
                    if (true)
                    {
                        Managers.Resource.Instanciate("Effects/PoisonAttack", gameObject.transform);
                    }
                    State = Define.State.Attack;
                }
                else
                {
                    State = Define.State.Moving;
                }
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
}
