using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    private static readonly GameData _gameData = new ();
    [SerializeField] private int _wolfCount = 0;
    [SerializeField] private int _snakeCount = 0;
    [SerializeField] private int[] _spawnMonsterCntArr = _gameData.SpawnMonsterCnt;

    [SerializeField] private Vector3[] _spawnPosArr = GameData.SpawnerPos;
    private GameObject _spawnTower;
    private Define.Way _spawnWay;

    public void AddWolfCount(int value) { _wolfCount += value; }
    public void AddSnakeCount(int value) { _snakeCount += value; }
    
    private int _spawnersCount = GameData.SpawnersCount;

    #region tmp

    private float _sheepSpawnTime = 3.0f;
    private float _roundTime = 10f;
    private float _lastSpawnTime = 0.0f;
    private int _cnt = 0;
    private int _sheepCnt = _gameData.SpawnSheepCnt;
    public static Bounds _bounds;

    private Define.MonsterId[] _monsterList =
    {
        Define.MonsterId.WolfPup,
        Define.MonsterId.Lurker,
        Define.MonsterId.Snakelet,
        Define.MonsterId.Wolf,
        Define.MonsterId.Creeper,
        Define.MonsterId.Snake,
        Define.MonsterId.WereWolf,
        Define.MonsterId.SnakeNaga
    };

    private int _summon = 0;

    public int Summon
    {
        get => _summon;
        set
        {
            _summon = value;
            if (_summon < _monsterList.Length)
            {
                StartCoroutine(ReserveSpawnMonster(_monsterList[_summon]));
            }
        }
    }
    #endregion

    public GameObject SpawnTower
    {
        get => _spawnTower;
        set
        {
            _spawnTower = value;
        }
    }

    public Define.Way SpawnWay
    {
        get => _spawnWay;
        set
        {
            _spawnWay = value;
            string towerName = SpawnTower.name.Replace("Button", "");
            Define.TowerId towerId = (Define.TowerId)Enum.Parse(typeof(Define.TowerId), towerName);
            StartCoroutine(ReserveSpawnTower(towerId, _spawnWay, 1));
        }
    }
    
    void Start()
    {
        _bounds = new Bounds(GameData.FenceCenter[1], GameData.FenceSize[1]);
        ReserveSpawnFence(GameData.FenceCnt[1], GameData.FenceName[1]);
        StartCoroutine(ReserveSpawnMonster(Define.MonsterId.MosquitoBug));
        
        for (int i = 0; i < _sheepCnt; i++)
        {
            ReserveSpawnSheep();
        }
    }

    void Update()
    {
        // if (Time.time > _lastSpawnTime + _sheepSpawnTime && cnt < _sheepCnt)
        // {
        //     _lastSpawnTime = Time.time;
        //     ReserveSpawnSheep();
        //     cnt++;
        // }
        // if (Time.time > _lastSpawnTime + _roundTime)
        // {
        //     _lastSpawnTime = Time.time;
        //     Summon += 1;
        // }
    }

    IEnumerator ReserveSpawnMonster(Define.MonsterId id)
    {
        yield return new WaitForSeconds(GameData.RoundTime);
        Define.MonsterId monsterId = id;
        // ReSharper disable once HeapView.BoxingAllocation
        string monsterName = monsterId.ToString();

        for (int i = 0; i < _spawnersCount; i++)
        {
            int cnt = _spawnMonsterCntArr[i];
            if (cnt == 0)
                continue;
            for (int j = 0; j < cnt; j++)
            {
                Vector3 monsterSpawnPos = Managers.Game.GetRandomPointOnNavMesh(_spawnPosArr[i]);
                GameObject obj = Managers.Game.Spawn(monsterId, Define.WorldObject.Monster, $"Monsters/{monsterName}");
                obj.transform.position = monsterSpawnPos;
            }
        }
    }

    IEnumerator ReserveSpawnTower(Define.TowerId id, Define.Way way, int fenceLevel)
    {
        Define.TowerId towerId = id;
        string towerName = towerId.ToString();

        Vector3 towerSpawnPos;
        switch (way)
        {
            case Define.Way.West:
                towerSpawnPos = new Vector3(-GameData.FenceSize[fenceLevel].x / 2, 6, 0);
                break;
            
            case Define.Way.North:
                towerSpawnPos = new Vector3(0, 6, GameData.FenceSize[fenceLevel].z / 2);
                break;
            
            case Define.Way.East:
                towerSpawnPos = new Vector3(GameData.FenceSize[fenceLevel].x / 2, 6, 0);
                break;
            default:
                towerSpawnPos = Vector3.zero;
                break;
        }

        GameObject obj = Managers.Game.Spawn(towerId, Define.WorldObject.Tower, $"Towers/{towerName}");
        obj.transform.position = towerSpawnPos;
        
        yield break;
    }
    
    private void ReserveSpawnSheep()
    {
        Vector3 spawnPos = GameData.center;
        Vector3 monsterSpawnPos = Managers.Game.GetRandomPointOnNavMesh(spawnPos);
        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Sheep, "Sheep");
        obj.transform.position = monsterSpawnPos;
    }

    private void ReserveSpawnFence(int cnt, string fencePrefab, int lv = 1)
    {
        Vector3[] fencePos =
            GameData.GetPos(GameData.FenceCnt[lv], GameData.FenceRow[lv], GameData.FenceStartPos[lv]);
        float[] fenceRotation = GameData.GetRotation(GameData.FenceCnt[lv], GameData.FenceRow[lv]);
        
        for (int i = 0; i < cnt; i++)
        {
            GameObject gameobject = Managers.Game.Spawn(Define.WorldObject.Fence, fencePrefab);
            gameobject.transform.position = fencePos[i];
            gameobject.transform.rotation = Quaternion.Euler(0, fenceRotation[i], 0);
        }
    }
}
