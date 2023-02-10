using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFireController : ProjectileController
{
    protected override void UpdateAttack()
    {
        base.UpdateAttack();
        GetMp();
    }
}
