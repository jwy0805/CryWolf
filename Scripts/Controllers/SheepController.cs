using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SheepController : BaseController
{
    private Stat _stat;
    
    private Vector3 _point;
    private float _lastMoveTime = 0.0f;
    private float _moveTime = 0.0f;
    private NavMeshAgent _nma;

    protected override void Init()
    {
        base.Init();
        
        WorldObjectType = Define.WorldObject.Sheep;

        _nma = GetComponent<NavMeshAgent>();
        _nma.enabled = false;
        _nma.enabled = true;
        
        _stat = gameObject.GetComponent<Stat>();
        _stat.Hp = 40;
        _stat.MaxHp = 40;
        _stat.Mp = 0;
        _stat.maxMp = 0;
        _stat.Attack = 5;
        _stat.Defense = 0;
        _stat.MoveSpeed = 0.4f;
        
        // DropWool();
    }

    protected override void UpdateIdle()
    {
        if (Time.time > _lastMoveTime + _moveTime)
        {
            _destPos = Managers.Game.GetRandomPointOnNavMesh(gameObject.transform.position, distance: 3.0f);
            State = Define.State.Moving;
            _lastMoveTime = Time.time;
            _moveTime = Random.Range(6, 9);
        }
    }
    
    protected override void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;

        if (dir.magnitude < 0.1f || 
            Physics.Raycast(transform.position + Vector3.up * 0.5f, dir.normalized, 1.0f, 
                LayerMask.GetMask("Block", "Monsters", "Player", "Sheep")))
        {
            State = Define.State.Idle;
        }
        else
        {
            _nma.SetDestination(_destPos);
            _nma.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }
    
    protected override void UpdateDie()
    {
        if (_nma != null)
        {
            _nma.enabled = false;
            _nma.enabled = true;
        }
        
        StartCoroutine(Despawn(gameObject, 2.0f));
    }

    private void DropWool()
    {
        GameObject wool = Managers.Resource.Instanciate("WorldObjects/Wool");
        wool.transform.position = gameObject.transform.position + Vector3.up;
    }
}