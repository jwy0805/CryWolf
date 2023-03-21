using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider; 
    [SerializeField] private Image _sliderFill;
    private Stat _stat;
    private Vector3 _colliderSize;
    
    void Start()
    {
        _stat = gameObject.GetComponent<Stat>();
        _colliderSize = gameObject.GetComponent<Collider>().bounds.size;
    }

    void Update()
    {
        Transform sliderTransform = transform.GetChild(0);
        sliderTransform.position = gameObject.transform.position + 
                                   Vector3.up * (_colliderSize.y) + 
                                   Vector3.left * (_colliderSize.x);
        sliderTransform.rotation = Camera.main.transform.rotation;
        
        float ratio = (_stat.Hp / (float)_stat.MaxHp) * 100;
        _slider.value = ratio;
        
        if (ratio >= 99.8f)
            sliderTransform.gameObject.SetActive(false);
        else 
            sliderTransform.gameObject.SetActive(true);
        
        switch (ratio)
        {
            case > 70.0f:
                _sliderFill.color = Color.green;
                break;
            case < 30.0f:
                _sliderFill.color = Color.red;
                break;
            default:
                _sliderFill.color = Color.yellow;
                break;
        }
    }
}
