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
    private float _lastYieldTime = 0.0f;
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

        if (Time.time > _lastYieldTime + GameData.RoundTime)
        {
            _lastYieldTime = Time.time;
            YieldCoin(GameData.SheepYield);
        }
    }
    
    protected override void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;
        
        if (Time.time > _lastYieldTime + GameData.RoundTime)
        {
            _lastYieldTime = Time.time;
            YieldCoin(GameData.SheepYield);
        }

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

    private void YieldCoin(int yield)
    {
        GameObject coin;
        
        switch (yield)
        {
            case < 30:
                coin = Managers.Resource.Instanciate("Items/CoinStarSilver");
                coin.GetComponent<CoinController>().gold = yield;
                break;
            case < 100:
                coin = Managers.Resource.Instanciate("Items/CoinStarGolden");
                coin.GetComponent<CoinController>().gold = yield;
                break;
            case < 200:
                coin = Managers.Resource.Instanciate("Items/PouchGreen");
                coin.GetComponent<CoinController>().gold = yield;
                break;
            case < 300:
                coin = Managers.Resource.Instanciate("Items/PouchRed");
                coin.GetComponent<CoinController>().gold = yield;
                break;
            default:
                coin = Managers.Resource.Instanciate("Items/ChestGold");
                coin.GetComponent<ChestController>().gold = yield;
                break;
        }
        
        coin.transform.position = gameObject.transform.position + Vector3.up * 0.5f;
    }
}