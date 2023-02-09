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
            UI_GameSheep uiGameSheep = GameObject.Find("UI_GameSheep").GetComponent<UI_GameSheep>();
            uiGameSheep.SelectedObj = _selected;
        }
    }

    // UI_GameSheep -> Spawner
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
        _spawner.SpawnTower = Spawn;
        _spawner.SpawnWay = Way;
    }
}