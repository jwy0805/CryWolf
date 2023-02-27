using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class PlayerController : BaseController
{
    private int _mask = (1 << (int)Define.Layer.Ground);
    // private int _sheepMask = (1 << (int)Define.Layer.Sheep);
    private int _resource = 0;
    public UnityEvent onGoldChanged;
    
    public int Resource
    {
        get => _resource;
        set
        {
            _resource = value;
            ResourceChanged(_resource);
        }
    }

    protected override void Init()
    {
        base.Init();
        WorldObjectType = Util.SheepOrWolf == "Sheep"
            ? Define.WorldObject.PlayerSheep
            : Define.WorldObject.PlayerWolf;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
    }

    protected override void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;

        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            // if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir,
            //         1.0f, LayerMask.GetMask("Block")))
            // {
            //     if (Input.GetMouseButton(0) == false)
            //         State = Define.State.Idle;
            //     return;
            // }

            // if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir,
            //         3.0f, LayerMask.GetMask("Fence")))
            // {
            //     IsJump = true;
            // }
            
            RaycastHit hit;
            float moveDist = Mathf.Clamp(5.0f * Time.deltaTime, 0, dir.magnitude);
            Vector3 rayStart = transform.position + Vector3.up;
            Vector3 rayDir = (transform.position + dir.normalized * moveDist) - rayStart; 
            bool raycastHit = Physics.Raycast(rayStart, rayDir, out hit, 70f, _mask);
            Debug.DrawRay(rayStart, rayDir * 5, Color.blue, 1.0f);

            if (raycastHit)
            {
                transform.position = hit.point;
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
        }
    }

    private void OnMouseEvent(Define.MouseEvent evt)
    {
        OnMouseEvent_IdleRun(evt);
    }

    private void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 70.0f);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 70f, Color.red, 1.0f);

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                if (raycastHit)
                {
                    int layer = hit.transform.gameObject.layer;

                    if (layer == (int)Define.Layer.Ground)
                    {
                        _destPos = hit.point;
                        State = Define.State.Moving;
                    }
                    else
                    {
                        DeliverGameObject dgo = gameObject.GetComponent<DeliverGameObject>();
                        dgo.Selected = hit.transform.gameObject;
                    }
                }
                
                break;
            
            case Define.MouseEvent.Press:
                if (raycastHit)
                    _destPos = hit.point;
                break;
        }
    }

    private void ResourceChanged(int gold)
    {
        if (Enum.IsDefined(typeof(Define.SheepCharacter), gameObject.name))
        {
            var ui = GameObject.FindWithTag("UI").GetComponent<UI_GameSheep>();
            var goldText = ui.DictTxt["GoldText"].GetComponent<TextMeshProUGUI>();
            goldText.text = gold.ToString();
        }
        else
        {
            var ui = GameObject.FindWithTag("UI").GetComponent<UI_GameWolf>();
            var resourceText = ui.DictTxt["ResourceText"].GetComponent<TextMeshProUGUI>();
            resourceText.text = gold.ToString();
        }
    }
}
