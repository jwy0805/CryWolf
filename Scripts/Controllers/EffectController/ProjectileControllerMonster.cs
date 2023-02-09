using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileControllerMonster : MonoBehaviour
{
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public GameObject hit;
    public GameObject flash;
    protected Rigidbody rb;
    public GameObject[] Detached;

    protected Stat _stat;
    protected BaseController _baseController;
    protected Vector3 _destPos;
    protected GameObject _lockTarget;
    public float speed = 15f;
    protected float _validTime;
    protected float _initTime;
    private string[] effects;
    
    private void Start()
    {
        Init();
    }

    private void FixedUpdate ()
    {
        Vector3 dir = _destPos - transform.position;
        float moveDist = Mathf.Clamp(speed * Time.deltaTime, 0, dir.magnitude);
        transform.position += dir.normalized * moveDist;
        
        if (Time.time - _initTime > _validTime + 0.2f)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }

    protected virtual void Init()
    {
        Transform parent = transform.parent.GetComponent<Transform>();
        _stat = parent.GetComponent<Stat>();
        _baseController = parent.GetComponent<BaseController>();
        _lockTarget = _baseController._lockTarget;
        if (_lockTarget == null)
        {
            return;
        }
        _validTime = _stat.AttackRange / speed;
        _destPos = _lockTarget.transform.position;

        _initTime = Time.time;

        #region Effect

        rb = GetComponent<Rigidbody>();

        flash = Managers.Resource.Instanciate($"Effects/Flashes/{gameObject.name}Flash");
        if (flash != null)
        {
            //Instantiate flash effect on projectile position
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            
            //Destroy flash effect depending on particle Duration time
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
        Destroy(gameObject,5);

        #endregion
    }
    
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == (int)Define.Layer.Monsters)
        {
            return;
        }
        
        Stat targetStat = collision.gameObject.GetComponent<Stat>();
        if (targetStat != null)
        {
            targetStat.OnAttakced(_stat);
        }
        
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

        //Removing trail from the projectile on collision enter or smooth removing. Detached elements must have "AutoDestroying script"
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
