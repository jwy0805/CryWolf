using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour
{
    private Vector3 _destPos;
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        // _rb.constraints = RigidbodyConstraints.FreezeAll;
        _rb.isKinematic = true;
        _destPos = new Vector3(transform.position.x, 6, 0);
    }

    void Update()
    {
        _rb.isKinematic = false;
        // _rb.constraints = RigidbodyConstraints.None;
        Vector3 dir= _destPos - transform.position;
        float moveDist = Mathf.Clamp(3.0f * Time.deltaTime, 0, dir.magnitude);
        transform.position += dir.normalized * moveDist;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(dir), 20 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain")) return;
        // _rb.constraints = RigidbodyConstraints.None;
        Vector3 opposite = collision.contacts[0].normal;
        _rb.AddForce(0, 5, 5, ForceMode.Impulse);
    }
}
