using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFireController : ProjectileControllerMonster
{
    protected override void Init()
    {
        Transform parent = transform.parent.GetComponent<Transform>();
        base.Init();
    }
    
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer is not ((int)Define.Layer.Sheep or (int)Define.Layer.Fence
            or (int)Define.Layer.Tower))
        {
            return;
        }
        
        Stat targetStat = collision.gameObject.GetComponent<Stat>();
        if (targetStat != null)
        {
            targetStat.OnAttakced(_stat);
        }
        _stat.Mp += 4;

        #region Effect

        //Lock all axes movement and rotation
        rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        //Spawn hit effect on collision
        hit = Managers.Resource.Instanciate($"Effects/Hits/{gameObject.name}Hit");
        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            //Destroy hit effects depending on particle Duration time
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }

        //Removing trail from the projectile on cillision enter or smooth removing. Detached elements must have "AutoDestroying script"
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }
        //Destroy projectile on collision
        Destroy(gameObject);

        #endregion
    }
}
