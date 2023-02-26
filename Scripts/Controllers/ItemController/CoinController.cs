using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private Define.State _state = Define.State.Idle;
    private Animator _anim;
    private Vector3 _destPos;
    private GameObject _player;
    private readonly float _dist = 4f;
    private readonly float _moveSpeed = 3f;
    public int gold = 0;

    public Define.State State
    {
        get => _state;
        set
        {
            _state = value;

            _anim = GetComponent<Animator>();
            _anim.CrossFade("IDLE", 0.1f);
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        switch (State)
        {
            case Define.State.Moving:
                UpdateMoving();
                break;
            default:
                UpdateIdle();
                break;
        }
    }

    private void Init()
    {
        _player = GameObject.FindWithTag("Player");
    }

    private void UpdateIdle()
    {
        if (_player == null) Managers.Resource.Destroy(gameObject);
        float dist = (_player.transform.position - transform.position).sqrMagnitude;
        if (dist < _dist)
        {
            State = Define.State.Moving;
        }
    }

    private void UpdateMoving()
    {
        _destPos = _player.transform.position;
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.3f)
        {
            // 골드 증가, 코인은 사라짐
            GameObject go = GameObject.FindWithTag("UI").GetComponent<UI_GameSheep>().DictTxt["GoldText"];
            int.TryParse(go.GetComponent<TextMeshProUGUI>().text, out int goldUi);
            goldUi += gold;
            go.GetComponent<TextMeshProUGUI>().text = goldUi.ToString();
            Managers.Resource.Destroy(gameObject);
        }
        else
        {
            float moveDist = Mathf.Clamp(_moveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
        }
    }
}
