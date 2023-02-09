using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CanSpawn : MonoBehaviour
{
    [SerializeField] private Image _background;
    private RectTransform _rectTransform;
    private Collider _collider;
    private bool _canSpawn;

    public bool CanSpawn
    {
        get => _canSpawn;
        set
        {
            _canSpawn = value;
            if (!_canSpawn)
            {
                _background.color = new Color(1f, 0f, 0f, 0.75f);
            }
            else
            {
                _background.color = new Color(0f, 1f, 0f, 0.75f);
            }
        }
    }

    void Start()
    {
        _rectTransform = _background.GetComponent<RectTransform>();
        _collider = gameObject.GetComponent<Collider>();
        float multiplier = _collider.bounds.size.x;
        _rectTransform.localScale = new Vector3(multiplier, multiplier, 1);
        CanSpawn = false;
    }

    private void Update()
    {
        
    }
}
