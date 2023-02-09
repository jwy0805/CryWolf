using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerController : BaseController
{
    public Define.TowerId TowerId { get; protected set; } = Define.TowerId.Unknown;
    protected Stat _stat;
    protected string[] tags = { "Monster" };
    protected GameObject[] _tagged;
    private Drag _drag;
    protected bool _active;
    private GameObject _canvas;

    protected float _targetingTime = 1.0f;
    protected float _lastTargetingTime = 0.0f;
    
    public virtual bool Active
    {
        get => _active;
        set
        {
            _active = value;
            if (!_active)
            {
                State = Define.State.None;
            }            
            else
            {
                _canvas.SetActive(false);
                Destroy(_drag);
                State = Define.State.Idle;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();
        WorldObjectType = Define.WorldObject.Tower;
        Vector3 size = gameObject.GetComponent<Collider>().bounds.size;
        _canvas = transform.Find("CanSpawn").gameObject;
        _drag = gameObject.GetComponent<Drag>();
        _stat = gameObject.GetComponent<Stat>();

        Active = false;
    }
    
    protected override void UpdateIdle()
    {
        if (_lockTarget != null)
        {
            float distance = (_lockTarget.transform.position - transform.position).magnitude;
            if (distance <= _stat.AttackRange)
            {
                    State = Define.State.Attack;
            }
        }
        
        if (Time.time > _lastTargetingTime + _targetingTime)
        {
            _lastTargetingTime = Time.time;
            SetTarget(tags);
        }
    }

    protected override void UpdateAttack()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quaternion = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, 20 * Time.deltaTime);
        }
    }

    protected override void UpdateSkill()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quaternion = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, 20 * Time.deltaTime);
        }
    }

    protected override void UpdateDie()
    {
        StartCoroutine(Despawn(gameObject, 2.0f));
    }
    
    protected virtual void SetTarget(string[] tags)
    {
        float closestDist = 5000.0f;
        foreach (var tag in tags)
        {
            _tagged = GameObject.FindGameObjectsWithTag(tag);
            foreach (var tagged in _tagged)
            {
                Vector3 targetPos = tagged.transform.position;
                Stat stat = tagged.gameObject.GetComponent<Stat>();
                stat.enabled = true;
                bool targetable = stat.Targetable;
                float dist = (targetPos - transform.position).sqrMagnitude;
                if (dist < closestDist && targetable)
                {
                    closestDist = dist;
                    _lockTarget = tagged;
                }
            }   
        }
    }
    
    protected Vector3 SetDest()
    {
        Vector3 destination = Vector3.zero;
        float closestDist = 5000f;
        foreach (var pos in GameData.SpawnerPos)
        {
            float dist = (pos - transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                destination = pos;
            }
        }
    
        return destination;
    }
    
    // Animation Event
    protected virtual void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttakced(_stat);
        }
    }
    
    protected virtual void OnEndEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();

            if (targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _stat.AttackRange)
                {
                    State = Define.State.Attack;
                }
                else
                {
                    _lockTarget = null;
                    State = Define.State.Idle;
                }
            }
            else
            {
                _lockTarget = null;
                State = Define.State.Idle;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}
