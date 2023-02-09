using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBarUnit : MonoBehaviour
{
    [SerializeField] private Slider _slider; 
    [SerializeField] private Image _sliderFill;
    private Stat _stat;
    private Transform _sliderTransform;
    
    void Start()
    {
        _stat = gameObject.GetComponent<Stat>();
        _sliderTransform = GameObject.Find("HealthSliderUnit").transform;
    }

    void Update()
    {
        float ratio = (_stat.Hp / (float)_stat.MaxHp) * 100;
        _slider.value = ratio;
        _sliderTransform.rotation = Camera.main.transform.rotation;
        // if (ratio >= 99.8f)
        //     sliderTransform.gameObject.SetActive(false);
        // else 
        //     sliderTransform.gameObject.SetActive(true);
        
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
