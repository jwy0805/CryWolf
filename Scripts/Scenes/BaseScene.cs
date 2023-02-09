using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = System.Object;

public abstract class BaseScene : MonoBehaviour
{
    public static Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectsOfType(typeof(EventSystem));
        if (obj == null)
        {
            Managers.Resource.Instanciate("UI/EventSystem").name = "@EventSystem";
        }
    }

    public abstract void Clear();
}
