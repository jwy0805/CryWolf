using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorHitController : MonoBehaviour
{
    private Stat _stat;
    
    private void Start()
    {
        transform.position = GameData.center + Vector3.up * 3f;
        Transform parent = transform.parent.transform;
        _stat = parent.GetComponent<Stat>();
        
        StartCoroutine(DestroyHit());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer is not ((int)Define.Layer.Sheep or (int)Define.Layer.Fence
            or (int)Define.Layer.Tower))
        {
            return;
        }

        Stat targetStat = other.gameObject.GetComponent<Stat>();
        if (targetStat != null)
        {
            targetStat.OnAttakced(_stat);
        }
    }

    IEnumerator DestroyHit()
    {
        yield return new WaitForSeconds(2f);
        Managers.Resource.Destroy(gameObject);
    }
}
