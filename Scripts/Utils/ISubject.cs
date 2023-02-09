using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubject
{
    void AddObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void Notify();
}
