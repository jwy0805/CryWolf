using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void OnSkillUpgraded(string skillName);
}
