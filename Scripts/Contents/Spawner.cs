using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private int _wolfCount = 0;
    [SerializeField] private int _snakeCount = 0;
    [SerializeField] private int[] _spawnMonsterCntArr = GameData.SpawnMonsterCnt;

    [SerializeField] private Vector3[] _spawnPosArr = GameData.SpawnerPos;
    private Define.Way _spawnWay;

    public void AddWolfCount(int value) { _wolfCount += value; }
    public void AddSnakeCount(int value) { _snakeCount += value; }
    
    private int _spawnersCount = GameData.SpawnersCnt;
    
    #region tmp
    private float _roundTime = 10f;
    private float _lastSpawnTime = 0.0f;
    

    private Define.MonsterId[] _monsterList =
    {
        Define.MonsterId.WolfPup,
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
    private int _storageLevel = 0;
    private bool _fenceRepair = false;
    private int _sheepIncrease = 0;
    public int Summon
    {
        get => _summon;
        set
        {
            _summon = value;
            if (_summon < _monsterList.Length)
            {
                StartCoroutine(ReserveSpawnMonsterEx(_monsterList[_summon]));
            }
        }
    }
    // 스킬 업그레이드 알림을 받아서 실행하기 위한 변수
    public bool FenceRepair
    {
        // Round시작할 때 FenceRepair = true면 파괴된 울타리 고쳐짐
        get => _fenceRepair;
        set
        {
            _fenceRepair = value;
            GameObject[] fences = GameObject.FindGameObjectsWithTag("Fence");
            foreach (var fence in fences)
            {
                Stat fenceStat = fence.GetComponent<Stat>();
                fenceStat.Hp = fenceStat.MaxHp;
            }
            // ReserveDespawnFence();
            // ReserveSpawnFence(GameData.FenceName[_storageLevel], _storageLevel);
        }
    }
    
    public int StorageLevel
    {
        get => _storageLevel;
        set
        {
            _storageLevel = value;
            if (_storageLevel > 3) _storageLevel = 3;
            GameData.StorageLevel = _storageLevel;
            // 울타리 새로 생성
            if (_storageLevel != 1) ReserveDespawnFence();
            GameData.FenceBounds = new Bounds(GameData.FenceCenter[_storageLevel], GameData.FenceSize[_storageLevel]);
            ReserveSpawnFence(GameData.FenceName[_storageLevel], _storageLevel);
            // Way Bounds 설정
            GameData.NorthBounds = new Bounds(new Vector3(0f, 5f, 24f),
                new Vector3(GameData.FenceSize[_storageLevel].x, 6f, 50f));
            GameData.WestBounds = new Bounds(new Vector3(-24f, 5f, GameData.FenceCenter[_storageLevel].z),
                new Vector3(50f, 6f, GameData.FenceSize[_storageLevel].z));
            GameData.EastBounds = new Bounds(new Vector3(24f, 5f, GameData.FenceCenter[_storageLevel].z),
                new Vector3(50f, 6f, GameData.FenceSize[_storageLevel].z));
            // 몬스터 밖으로 밀어냄
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
            foreach (var monster in monsters)
            {
                // 현재 위치에서 가장 가까운 울타리 밖으로 나감
                if (GameData.FenceBounds.Contains(monster.transform.position))
                {
                    Debug.Log(GameData.FenceBounds.ClosestPoint(monster.transform.position));
                    // Vector3 tmpPos = GameData.FenceBounds.ClosestPoint(monster.transform.position);
                    monster.transform.position = new Vector3(10, 6, 10);
                }
            }
        }
    }

    public int SheepIncrease
    {
        get => _sheepIncrease;
        set
        {
            _sheepIncrease = value;
            ReserveSpawnSheep();
            GameData.SheepCapacity += 1;
        }
    }
    #endregion
    
    public GameObject SpawnUnit { get; set; }
    public GameObject UIType { get; set; }
    
    public Define.Way SpawnWay
    {
        get => _spawnWay;
        set
        {
            _spawnWay = value;
            if (UIType.name == "UI_GameSheep")
            {
                string towerName = SpawnUnit.name.Replace("Button", "");
                Define.TowerId towerId = (Define.TowerId)Enum.Parse(typeof(Define.TowerId), towerName);
                StartCoroutine(ReserveSpawnTower(towerId, _spawnWay, 1));
            }
            else
            {
                string monsterName = SpawnUnit.name.Replace("Button", "");
                Define.MonsterId monsterId = (Define.MonsterId)Enum.Parse(typeof(Define.MonsterId), monsterName);
                StartCoroutine(ReserveSpawnMonster(monsterId, _spawnWay));
            }
        }
    }
    
    void Start()
    {
        StorageLevel = 1;
        // 시작하면 양 3마리 주어짐
        for (int i = 0; i < GameData.SheepCapacity; i++) ReserveSpawnSheep();
        StartCoroutine(ReserveSpawnMonster(Define.MonsterId.Wolf, Define.Way.North));
    }

    void Update()
    {
        // Tower Test 용도
        // SummonMonster();
        //
        // // Wolf Test 용도
        // SummonTower();
    }

    IEnumerator ReserveSpawnMonster(Define.MonsterId id, Define.Way way)
    {
        string monsterName = Convert.ToString(id);
        Vector3 monsterSpawnPos;
        switch (way)
        {
            case Define.Way.West:
                monsterSpawnPos = Managers.Game.GetRandomPointOnNavMesh(_spawnPosArr[0]);
                break;
            case Define.Way.North:
                monsterSpawnPos = Managers.Game.GetRandomPointOnNavMesh(_spawnPosArr[1]);
                break;
            case Define.Way.East:
                monsterSpawnPos = Managers.Game.GetRandomPointOnNavMesh(_spawnPosArr[2]);
                break;
            default:
                monsterSpawnPos = Vector3.zero;
                break;
        }

        GameObject obj = Managers.Game.Spawn(id, Define.WorldObject.Monster, $"Monsters/{monsterName}");
        obj.transform.position = monsterSpawnPos;
        BaseController baseController = obj.GetComponent<BaseController>();
        baseController.Way = way;
        yield break;
    }
    

    IEnumerator ReserveSpawnTower(Define.TowerId id, Define.Way way, int fenceLevel)
    {
        string towerName = Convert.ToString(id);
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

        GameObject obj = Managers.Game.Spawn(id, Define.WorldObject.Tower, $"Towers/{towerName}");
        obj.transform.position = towerSpawnPos;
        BaseController baseController = obj.GetComponent<BaseController>();
        baseController.Way = way;
        yield break;
    }
    
    
    IEnumerator ReserveSpawnMonsterEx(Define.MonsterId id)
    {
        float tmp = 1;
        yield return new WaitForSeconds(tmp);
        // yield return new WaitForSeconds(GameData.RoundTime);
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

    IEnumerator ReserveSpawnTowerEx(Define.TowerId id)
    {
        yield break;
    }
    
    private void ReserveSpawnSheep()
    {
        Vector3 spawnPos = GameData.Center;
        Vector3 monsterSpawnPos = Managers.Game.GetRandomPointOnNavMesh(spawnPos);
        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Sheep, "Sheep");
        obj.transform.position = monsterSpawnPos;
    }

    private void ReserveSpawnFence(string fencePrefab, int lv = 1)
    {
        Vector3[] fencePos =
            GameData.GetPos(GameData.FenceCnt[lv], GameData.FenceRow[lv], GameData.FenceStartPos[lv]);
        float[] fenceRotation = GameData.GetRotation(GameData.FenceCnt[lv], GameData.FenceRow[lv]);
        
        for (int i = 0; i < GameData.FenceCnt[lv]; i++)
        {
            GameObject gameobject = Managers.Game.Spawn(Define.WorldObject.Fence, fencePrefab);
            gameobject.transform.position = fencePos[i];
            gameobject.transform.rotation = Quaternion.Euler(0, fenceRotation[i], 0);
        }
    }

    private void ReserveDespawnFence()
    {
        GameObject[] fences = GameObject.FindGameObjectsWithTag("Fence");
        for (int i = 0; i < fences.Length; i++)
        {
            GameData.CurrentFenceCnt -= 1;
            Managers.Resource.Destroy(fences[i]);
        }
        // foreach (var fence in fences) Managers.Resource.Destroy(fence);
    }

    private void SummonMonster()
    {
        if (Time.time > _lastSpawnTime + _roundTime)
        {
            _lastSpawnTime = Time.time;
            Summon += 1;
        }
    }

    private void SummonTower()
    {
        
    }
}
