using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : MonoBehaviour
{
    private BaseController _baseController;
    private Stat _stat;
    private RaycastHit _hitLayerMask;
    private Vector3 _destPos;
    private float _speed;
    private float _validTime;
    private float _initTime;

    private void Start()
    {
        Transform parent = transform.parent.GetComponent<Transform>();;
        _stat = parent.GetComponent<Stat>();
        _baseController = parent.GetComponent<BaseController>();
        transform.position = parent.position + Vector3.up;
        _speed = 15.0f;
        _validTime = _stat.AttackRange / _speed;
        _destPos = _baseController._lockTarget.transform.position;

        _initTime = Time.time;
    }
    
    private void Update()
    {
        Vector3 dir = _destPos - transform.position;
        float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
        transform.position += dir.normalized * moveDist;

        if (Time.time - _initTime > _validTime)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        GameObject go = collision.gameObject;
        int layer = go.layer;
        
        if (layer == (int)Define.Layer.Monsters)
        {
            bool get = go.TryGetComponent(out Stat targetStat);
            if (!get || _stat.Targetable == false) return;
            
            if (_stat.gameObject.name == "Bud")
            {
                targetStat.OnSkilled(_stat);
            }
            else
            {
                targetStat.OnAttakced(_stat);
            }
            
            Managers.Resource.Destroy(gameObject);
        }
        else if (layer == (int)Define.Layer.Ground)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
