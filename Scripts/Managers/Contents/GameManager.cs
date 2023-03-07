using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameManager
{
    private GameObject _player;
    private GameObject _sheep;
    private GameData _gameData = new ();
    private Dictionary<Define.MonsterId, GameObject> _monsters = new Dictionary<Define.MonsterId, GameObject>();
    private Dictionary<Define.TowerId, GameObject> _towers = new Dictionary<Define.TowerId, GameObject>();

    public Action<int> OnSpawnEvent;

    public GameObject GetPlayer() { return _player; }

    public GameObject GetSheep() { return _sheep; }

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instanciate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Sheep:
                _sheep = go;
                break;
            case Define.WorldObject.PlayerSheep:
                _player = go;
                break;
            case Define.WorldObject.PlayerWolf:
                _player = go;
                break;
        }

        return go;
    }
    
    public GameObject Spawn(Define.MonsterId id, Define.WorldObject type, string path, Transform parent = null)
    {
        Define.MonsterId monsterId = id;
        GameObject go = Managers.Resource.Instanciate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                //_monsters.Add(monsterId, go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
        }

        return go;
    }
    
    public GameObject Spawn(Define.TowerId id, Define.WorldObject type, string path, Transform parent = null)
    {
        Define.TowerId towerId = id;
        GameObject go = Managers.Resource.Instanciate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Tower:
                if (OnSpawnEvent != null) 
                {
                    OnSpawnEvent.Invoke(1);
                }                
                break;
        }

        return go;
    }

    public void Despawn(GameObject go, float time = 0.0f)
    {
        Define.WorldObject type = GetWorldObjectType(go);
        
        switch (type)
        {
            case Define.WorldObject.Monster:
                Define.MonsterId monsterId = GetMonsterId(go);
                if (_monsters.ContainsKey(monsterId))
                {
                    _monsters.Remove(monsterId);
                    if (OnSpawnEvent != null)
                    {
                        OnSpawnEvent.Invoke(-1);
                    }
                }
                break;
            
            case Define.WorldObject.Tower:
                Define.TowerId towerId = GetTowerId(go);
                if (_towers.Remove(towerId))
                {
                    _towers.Remove(towerId);
                    if (OnSpawnEvent != null)
                    {
                        OnSpawnEvent.Invoke(-1);
                    }
                }
                break;
            
            case Define.WorldObject.Sheep:
                break;
            case Define.WorldObject.PlayerSheep:
                if (_player == go)
                    _player = null;
                break;
            case Define.WorldObject.PlayerWolf:
                if (_player == go)
                    _player = null;
                break;
        }
        
        Managers.Resource.Destroy(go, time);
    }
    
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController baseController = go.GetComponent<BaseController>();
        if (baseController == null)
            return Define.WorldObject.Unknown;

        return baseController.WorldObjectType;
    }

    public Define.MonsterId GetMonsterId(GameObject go)
    {
        MonsterController monsterController = go.GetComponent<MonsterController>();
        if (monsterController == null)
            return Define.MonsterId.Unknown;

        return monsterController.MonsterId;
    }
    
    public Define.TowerId GetTowerId(GameObject go)
    {
        TowerController towerController = go.GetComponent<TowerController>();
        if (towerController == null)
            return Define.TowerId.Unknown;

        return towerController.TowerId;
    }

    public Vector3 GetRandomPointOnNavMesh(Vector3 spawnerPos, float distance = 3.0f)
    {
        bool x;
        do
        {
            Vector3 randPosAroundSpawner = Random.insideUnitSphere * distance + spawnerPos;
            NavMeshHit hit;
            x = NavMesh.SamplePosition(randPosAroundSpawner, out hit, distance, NavMesh.AllAreas);
            if (x)
                return hit.position;
        } while (x == false);

        return new Vector3(0, 0, 0);
    }
}
