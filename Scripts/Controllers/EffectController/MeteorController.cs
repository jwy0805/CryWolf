using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Transform _parentTransform;
    private Stat _parentStat;
    private void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _parentTransform = transform.parent.transform;
        _parentStat = _parentTransform.GetComponent<Stat>();
        transform.position = GameData.center;
    }
    
    private void OnParticleCollision(GameObject other)
    {
        SkillHit(1 << (int)Define.Layer.Fence);
        SkillHit(1 << (int)Define.Layer.Sheep);
        SkillHit(1 << (int)Define.Layer.Tower);
    }

    private void SkillHit(int mask)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f, mask);
        int length = colliders.Length;
        for (int i = 0; i < length; i++)
        {
            GameObject go = colliders[i].gameObject;
            Stat targetStat = go.GetComponent<Stat>();
            if (targetStat != null)
            {
                targetStat.OnSkilled(_parentStat);
            }
        }
    }
}