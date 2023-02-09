using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private Define.State _state = Define.State.Idle;
    private Animator _anim;
    private Vector3 _destPos;

    public Define.State State
    {
        get => _state;
        set
        {
            _state = value;

            _anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Idle:
                    break;
            }
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        
    }

    private void Init()
    {
        
    }

    private void UpdateIdle()
    {
        
    }
}
