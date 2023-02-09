using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    protected Stat _stat;
    protected string[] _tags = { "Sheep", "Tower" };
    protected GameObject[] _tagged;
    protected NavMeshAgent _navMesh;

    protected float _targetingTime = 1.0f;
    protected float _lastTargetingTime = 0.0f;
    
    public Define.MonsterId MonsterId { get; protected set; } = Define.MonsterId.Unknown;
    
    protected override void Init()
    {
        base.Init();
        _navMesh = GetComponent<NavMeshAgent>();
        _navMesh.enabled = false;
        WorldObjectType = Define.WorldObject.Monster;
        _destPos = Managers.Game.GetRandomPointOnNavMesh(GameData.center, 5.0f);
        _navMesh.enabled = true;

        _stat = gameObject.GetComponent<Stat>();
        _stat.Hp = 0;
        _stat.MaxHp = 0;
        _stat.Mp = 0;
        _stat.maxMp = 0;
        _stat.Attack = 0;
        _stat.Defense = 0;
        _stat.MoveSpeed = 0;
        _stat.AttackRange = 0;
    }

    protected override void UpdateIdle()
    {
        if (_lockTarget == null || !Spawner._bounds.Contains(transform.position))
        {
            string[] tags = { "Sheep", "Fence", "Tower" };
            SetTarget(tags);
            State = Define.State.Moving;
        }

        if (Spawner._bounds.Contains(transform.position))
        {
            State = Define.State.Moving;
        }
    }

    protected override void UpdateMoving()
    {
        // Targeting
        if (Time.time > _lastTargetingTime + _targetingTime)
        {
            _lastTargetingTime = Time.time;
        
            NavMeshPath navMeshPath = new NavMeshPath();

            if (Spawner._bounds.Contains(transform.position))
            {
                string[] tags = { "Sheep", "Tower" };
                SetTarget(tags);
            }
            // Fence 안으로 들어갈 수 있는지?
            else if (_navMesh.CalculatePath(GameData.center, navMeshPath) 
                     && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                string[] tags = { "Sheep", "Tower" };
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
                _navMesh.SetDestination(transform.position);
                State = Define.State.Attack;
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

    protected override void UpdateAttack()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quaternion = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, 20 * Time.deltaTime);
        }
    }

    protected override void UpdateDie()
    {
        if (_navMesh != null)
        {
            _navMesh.enabled = false;
            _navMesh.enabled = true;
        }
        
        StartCoroutine(Despawn(gameObject, 2.0f));
    }
    
    protected void SetTarget(string[] tags)
    {
        if (Condition == Define.Condition.Aggro) return;
        float closestDist = 5000.0f;
        foreach (string tag in tags)
        {
            _tagged = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject tagged in _tagged)
            {
                Vector3 targetPos = tagged.transform.position;
                bool targetable = tagged.GetComponent<Stat>().Targetable;
                float dist = (targetPos - transform.position).sqrMagnitude;
                if (dist < closestDist && targetable)
                {
                    closestDist = dist;
                    _lockTarget = tagged;
                }
            }
        }
    }

    // Animation Event
    protected virtual void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttakced(_stat);
        }
    }

    protected virtual void OnEndEvent()
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
