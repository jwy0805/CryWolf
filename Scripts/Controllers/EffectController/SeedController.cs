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
    private GameObject _lockTarget;
    private float _speed;

    private void Start()
    {
        Init();
    }
    
    private void FixedUpdate()
    {
        Vector3 dir = _destPos - transform.position;
        float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
        float distParent = (transform.position - transform.parent.transform.position).magnitude;
        transform.position += dir.normalized * moveDist;
        if (distParent > _stat.AttackRange) Managers.Resource.Destroy(gameObject);
    }

    private void Init()
    {
        Transform parent = transform.parent.GetComponent<Transform>();
        _stat = parent.GetComponent<Stat>();
        _baseController = parent.GetComponent<BaseController>();
        transform.position = parent.position + Vector3.up;
        _lockTarget = _baseController._lockTarget;

        Collider targetCollider = _baseController._lockTarget.GetComponent<Collider>();
        _destPos = targetCollider.ClosestPoint(transform.position);
        _speed = 15.0f;
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        GameObject go = collider.gameObject;

        if (!go.CompareTag(_lockTarget.tag))
        {
            if (go.CompareTag("Terrain"))
            {
                Managers.Resource.Destroy(gameObject);
            }
        }
        else
        {
            if (go.TryGetComponent(out Stat targetStat))
            {
                if (!targetStat.Targetable) return;
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
        }
    }
}
