using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSubject : MonoBehaviour, ISubject
{
    private List<IObserver> _observers = new List<IObserver>();
    private string _skillName;
    
    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        if (_observers.IndexOf(observer) > 0)
        {
            _observers.Remove(observer);
        }
    }

    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.OnSkillUpgraded(_skillName);
        }
    }

    public void SkillUpgraded(string skillName)
    {
        _skillName = skillName;
        Notify();
    }
}
