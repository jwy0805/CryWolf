using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_UpgradePopup : UI_Popup
{
    public UnityEvent upgradeSkillEvent;
    private string _skillName;
    private SkillSubject _skillSubject;

    enum Buttons
    {
        AcceptButton,
        DenyButton,
    }

    enum Texts
    {
        SkillInfoText,
    }
    
    protected override void Init()
    {
        base.Init();
        
        BindObjects();
        SetButtonEvents();
        upgradeSkillEvent = new UnityEvent();
        upgradeSkillEvent.AddListener(DeliverSkillUpgraded);
        
        _skillSubject = GameObject.Find("SkillSubject").GetComponent<SkillSubject>();
    }

    protected override void BindObjects()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    protected override void SetButtonEvents()
    {
        GetButton((int)Buttons.AcceptButton).gameObject.BindEvent(OnAcceptClicked);
        GetButton((int)Buttons.DenyButton).gameObject.BindEvent(OnDenyClicked);
    }

    private void OnAcceptClicked(PointerEventData data)
    {
        // gold가 충분하면
        if (true)
        {
            UI_GameSheep ui = GameObject.Find("UI_GameSheep").GetComponent<UI_GameSheep>();
            GameObject currentSkillButton = ui.OnSelectedSkill;
            _skillName = currentSkillButton.name.Replace("Button", "");

            List<string> precedeSkills = GameData.SkillTreeSheep[_skillName].ToList();
            List<string> result = (from s in GameData.SkillUpgradedList select s).Intersect(precedeSkills).ToList();
            bool isEqual = precedeSkills.OrderBy(a => a).SequenceEqual(result.OrderBy(a => a));
            
            if (precedeSkills[0] == "free" || isEqual)
            {
                ui.SetAlpha(ui.DictSkillBtn[currentSkillButton.name], 1.0f);
                GameData.SkillUpgradedList.Add(_skillName);
                upgradeSkillEvent.Invoke();
            }
            // 튤립버튼 설정
        }
        
        Managers.UI.ClosePopupUI();
    }
    
    private void OnDenyClicked(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }

    private void DeliverSkillUpgraded()
    {
        _skillSubject.SkillUpgraded(_skillName);
    }
}

