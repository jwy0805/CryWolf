using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class DeliverGameObject : MonoBehaviour
{
    private GameObject _selected;
    private GameObject _spawn;
    private Define.Way _way;
    public Spawner _spawner;

    private void Start()
    {
        _spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
    }

    // PlayerController -> UI_GameSheep
    public GameObject Selected
    {
        get => _selected;
        set
        {
            _selected = value;

            var uiGame = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_Game>();
            uiGame.SelectedObj = _selected;
        }
    }

    // UI_Game -> Spawner
    // Spawn, Way 설정 후 SetSpawn() 호출 -> 클릭한 Way에서 Unit 생성됨
    public GameObject Spawn
    {
        get => _spawn;
        set
        {
            _spawn = value;
        }
    }

    public Define.Way Way
    {
        get => _way;
        set
        {
            _way = value;
            SetSpawn();
        }
    }

    public void SetSpawn()
    {
        // _spawner.UIType = gameObject;
        _spawner.SpawnUnit = Spawn;
        _spawner.SpawnWay = Way;
    }
}