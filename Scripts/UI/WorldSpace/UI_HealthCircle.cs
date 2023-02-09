using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthCircle : MonoBehaviour
{
    [SerializeField] public Slider _slider; 
    [SerializeField] private Image _sliderFill;
    private Stat _stat;
    private RectTransform _rectTransform;
    private Collider _collider;
    
    void Start()
    {
        _stat = gameObject.GetComponent<Stat>();
        _rectTransform = _slider.GetComponent<RectTransform>();
        _collider = gameObject.GetComponent<Collider>();
        float multiplier = _collider.bounds.size.x;
        _rectTransform.localScale = new Vector3(multiplier, multiplier, 1);
    }

    void Update()
    {
        float ratio = (_stat.Hp / (float)_stat.MaxHp) * 100;
        _slider.value = ratio;
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
