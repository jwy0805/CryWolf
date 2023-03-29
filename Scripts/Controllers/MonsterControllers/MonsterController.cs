using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    protected Stat _stat;
    protected GameObject[] _tagged;
    protected NavMeshAgent _navMesh;
    protected PlayerController _playerController;

    protected float _targetingTime = 1.0f;
    protected float _lastTargetingTime = 0.0f;
    
    public Define.MonsterId MonsterId { get; protected set; } = Define.MonsterId.Unknown;
    
    protected override void Init()
    {
        base.Init();
        _navMesh = GetComponent<NavMeshAgent>();
        _navMesh.enabled = false;
        WorldObjectType = Define.WorldObject.Monster;
        _destPos = Managers.Game.GetRandomPointOnNavMesh(GameData.Center, 5.0f);
        _navMesh.enabled = true;

        _stat = gameObject.GetComponent<Stat>();
        _stat.Mp = 0;
        _stat.maxMp = 0;
        _stat.Attack = 0;
        _stat.Defense = 0;
        _stat.MoveSpeed = 0;
        _stat.AttackRange = 0;
        
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var go in gameObjects)
        {
            if (Enum.IsDefined(typeof(Define.WolfCharacter), go.name)) 
                _playerController = go.GetComponent<PlayerController>();
        }    
    }

    protected override void UpdateIdle()
    {
        if (GameData.FenceBounds.Contains(transform.position) || IsReachable(GameData.Center))
            Tags = new[] { "Sheep", "Tower" };
        else Tags = new []{ "Tower", "Fence" };
        SetTarget(Tags);

        if (_lockTarget == null) State = Define.State.Idle;
        State = Define.State.Moving;
    }

    protected override void UpdateMoving()
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
        if (_lockTarget == null) return;
        Vector3 dir = _lockTarget.transform.position - transform.position;
        Quaternion quaternion = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, 20 * Time.deltaTime);
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
    
    protected virtual void SetTarget(string[] tags)
    {
        if (GetComponent<Stat>().Aggro) return;
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

    protected virtual bool IsReachable(Vector3 pos)
    {
        NavMeshPath path = new NavMeshPath();
        _navMesh.CalculatePath(pos, path);
        bool contains = true;
        Bounds bounds = new Bounds();
        switch (Way)
        {
            case Define.Way.West:
                bounds = GameData.WestBounds;
                break;
            case Define.Way.North:
                bounds = GameData.NorthBounds;
                break;
            case Define.Way.East:
                bounds = GameData.EastBounds;
                break;
        }
        for (int i = 0; i < path.corners.Length && contains; i++)
        {
            if (!bounds.Contains(path.corners[i])) contains = false;
        }
        return path.status == NavMeshPathStatus.PathComplete && contains;
    }
    
    // Animation Event
    protected virtual void OnHitEvent()
    {
        if (_lockTarget == null) return;
        Stat targetStat = _lockTarget.GetComponent<Stat>();
        targetStat.OnAttakced(_stat);
        _stat.Mp += 4;

        if (_playerController != null) _playerController.Resource += _stat.Resource;
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
                State = distance <= _stat.AttackRange ? Define.State.Attack : Define.State.Moving;
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
