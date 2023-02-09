using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class UI_Skill : MonoBehaviour
{
    private bool _isActive;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            SkillFunction(_isActive);
        }
    }

    protected void SkillFunction(bool isActive)
    {
        
    }
}
