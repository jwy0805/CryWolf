using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Transform _parentTransform;
    private Stat _parentStat;
    
    void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _parentTransform = transform.parent.transform;
        _parentStat = _parentTransform.GetComponent<Stat>();
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
        Stat targetStat = other.GetComponent<Stat>();
        if (targetStat != null)
        {
            targetStat.OnSkilled(_parentStat);
        }
    }

    private void SkillHit(int mask)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f, mask);
        int length = colliders.Length;
        for (int i = 0; i < length; i++)
        {
            GameObject go = colliders[i].gameObject;
            Debug.Log(go.name);
            Stat targetStat = go.GetComponent<Stat>();
            if (targetStat != null)
            {
                targetStat.OnSkilled(_parentStat);
            }
        }
    }
}
