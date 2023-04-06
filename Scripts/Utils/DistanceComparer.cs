using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceComparer : IComparer<Collider>
{
    private Vector3 _origin;
    
    public DistanceComparer(Vector3 origin)
    {
        _origin = origin;
    }

    public int Compare(Collider a, Collider b)
    {
        float distanceToA = Vector3.Distance(_origin, a.transform.position);
        float distanceToB = Vector3.Distance(_origin, b.transform.position);
        return distanceToA.CompareTo(distanceToB);
    }
}
