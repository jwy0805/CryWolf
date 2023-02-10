using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    private Define.State _state = Define.State.Idle;
    private Animator _anim;
    private Vector3 _destPos;
    private GameObject _player;
    private readonly float _dist = 4f;
    private readonly float _moveSpeed = 3f;

    public Define.State State
    {
        get => _state;
        set
        {
            _state = value;

            _anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Moving:
                    _anim.CrossFade("RUN", 0.1f, 0);
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
            // 골드 증가, Chest 사라짐
            Managers.Resource.Destroy(gameObject);
        }
        else
        {
            float moveDist = Mathf.Clamp(_moveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
        }
    }
}
