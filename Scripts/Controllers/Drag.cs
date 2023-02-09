using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private RaycastHit _hitRay, _hitLayerMask;
    private GameObject _objectHitPosition;
    private UI_CanSpawn _canSpawn;
    private TowerController _towerController;

    private void Start()
    {
        _canSpawn = gameObject.GetComponent<UI_CanSpawn>();
        _towerController = gameObject.GetComponent<TowerController>();
    }

    private void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);
    
        int layerMask = 1 << LayerMask.NameToLayer("Ground"); /* 특정 layer 검출 */
        if (Physics.Raycast(ray, out _hitLayerMask, Mathf.Infinity, layerMask))
        {
            float y = transform.position.y; /* 높이 저장 */
            transform.position = new Vector3 (_hitLayerMask.point.x, y, _hitLayerMask.point.z);            
        }
    }

    private void OnMouseUp()
    {
        if (_canSpawn.CanSpawn)
        {
            _towerController.Active = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.layer.ToString() != "Terrain")
        {
            _canSpawn.CanSpawn = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        // Bound내에 있으면
        if (true)
        {
            _canSpawn.CanSpawn = true;
        }
    }

    // private void OnMouseDown()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     if (Physics.Raycast(ray, out _hitRay))
    //     {
    //         _objectHitPosition = new GameObject("HitPosition");
    //         _objectHitPosition.transform.position = _hitRay.point;
    //         Debug.Log($"{_hitRay.point} ss");
    //         transform.SetParent(_objectHitPosition.transform);
    //     }
    // }
    //
    // private void OnMouseUp()
    // {
    //     transform.parent = null;
    //     Destroy(_objectHitPosition);
    // }
    //
    // private void OnMouseDrag()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);
    //     
    //     int layerMask = 1 << LayerMask.NameToLayer("Ground");
    //     if (Physics.Raycast(ray, out _hitLayerMask, Mathf.Infinity, layerMask))
    //     {
    //         float H = Camera.main.transform.position.y;
    //         float h = _objectHitPosition.transform.position.y;
    //         
    //         Debug.Log($"{H}, {h}");
    //         Vector3 newPos = (_hitLayerMask.point * (H - h) + Camera.main.transform.position * h) / H;
    //         // _objectHitPosition.transform.position = newPos;
    //     }
    // }
    
    
}
