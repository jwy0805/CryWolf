using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    private List<Define.Buff> _activeBuffs = new List<Define.Buff>();
    private GameObject _targetGameObject;

    public BuffManager(GameObject targetGameObject)
    {
        _targetGameObject = targetGameObject;
    }

    public void AddBuff(Define.Buff buff)
    {
        _activeBuffs.Add(buff);
    }

    public void RemoveBuff(Define.Buff buff)
    {
        _activeBuffs.Remove(buff);
    }
    
    
}
