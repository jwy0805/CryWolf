using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BudSkillPanel : UI_Scene
{
    private Dictionary<string, GameObject> _dictBtn = new ();
    private Dictionary<string, GameObject> _dictImg = new ();
    
    enum Buttons
    {
        BudAttackButton,
        BudAttackSpeedButton,
        BudRangeButton,
        BudSeedButton,
        BudDoubleButton,
    }

    enum Images
    {
        BudAttackPanel,
        BudAttackSpeedPanel,
        BudRangePanel,
        BudSeedPanel,
        BudDoublePanel,
    }

    private void Awake()
    {
        
    }

    protected override void Init()
    {
        // BindObjects();
        // SetButtonEvents();
        // SetUI();
    }

    protected override void BindObjects()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        
        // for (int i = 0; i < _objects[typeof(Button)].Length; i++)
        // {
        //     GameObject btn = GetButton(i).gameObject;
        //     _dictBtn.Add(btn.name, btn);
        // }
        //
        // for (int i = 0; i < _objects[typeof(Image)].Length; i++)
        // {
        //     GameObject img = GetImage(i).gameObject;
        //     _dictImg.Add(img.name, img);
        // }
    }

    protected override void SetButtonEvents()
    {
        
    }

    protected override void SetUI()
    {
        foreach (var panel in _dictImg.Values)
        {
            SetObjectSize(panel, 0.22f);
        }

        foreach (var btn in _dictBtn.Values)
        {
            Image img = _dictBtn[btn.name].gameObject.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.6f);
        }
    }
}
