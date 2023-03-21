using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceController : BaseController
{
    private Stat _stat;
    
    protected override void Init()
    {
        WorldObjectType = Define.WorldObject.Fence;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
        GameData.CurrentFenceCnt += 1;
        _stat = gameObject.GetComponent<Stat>();
        _stat.Hp = 200;
        _stat.MaxHp = 200;
    }
}
