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
    private GameObject _currentSkillButton;
    private int _cost;

    private enum Buttons
    {
        AcceptButton,
        DenyButton,
    }

    private enum Texts
    {
        SkillInfoText,
        CostText,
    }
    
    protected override void Init()
    {
        base.Init();
        
        BindObjects();
        SetCost();
        SetButtonEvents();
        upgradeSkillEvent = new UnityEvent();
        upgradeSkillEvent.AddListener(DeliverSkillUpgraded);
        
        _skillSubject = GameObject.Find("Subject").GetComponent<SkillSubject>();
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

    private void SetCost()
    {
        var ui = GameObject.FindWithTag("UI").GetComponent<UI_GameSheep>();
        var currentSkillButton = ui.OnSelectedSkill;
        var skillName = currentSkillButton.name.Replace("Button", "");

        switch (skillName)
        {
            case "FenceRepair":
                _cost = SetFenceRepairCost();
                break;
            case "StorageLvUp":
                _cost = GameData.StorageLvUpCost[GameData.StorageLevel];
                break;
            default:
                _cost = 100;
                break;
        }
        
        GetText((int)Texts.CostText).gameObject.GetComponent<TextMeshProUGUI>().text = _cost.ToString();

    }

    // 여기에 base skill 구현
    private void OnAcceptClicked(PointerEventData data)
    {
        UI_GameSheep ui = GameObject.FindWithTag("UI").GetComponent<UI_GameSheep>();
        GameObject currentSkillButton = ui.OnSelectedSkill;
        _skillName = currentSkillButton.name.Replace("Button", "");
        var playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        int resourceOwned = playerController.Resource;
        
        // 울타리 수리는 실시간으로 비용이 변함 -> AcceptButton이 클릭될 때 한번 더 cost 수정
        if (_skillName == "FenceRepair") _cost = SetFenceRepairCost();
        
        if (resourceOwned >= _cost)
        {
            GameObject[] fences;
            switch (_skillName)
            {
                // Sheep Base Skill
                case "FenceRepair":
                    fences = GameObject.FindGameObjectsWithTag("Fence");
                    foreach (var fence in fences)
                    {
                        Stat stat = fence.GetComponent<Stat>();
                        stat.Hp = stat.MaxHp;
                    }
                    break;
                case "StorageLvUp":
                    if (GameData.CurrentFenceCnt != GameData.FenceCnt[GameData.StorageLevel])
                    {
                        // 오류 메시지 출력
                        // Debug.Log("울타리가 고장나 업그레이드 할 수 없습니다!");
                    }

                    Spawner spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
                    spawner.StorageLevel += 1;
                    break;
                case "GoldIncrease":
                    break;
                case "SheepHealth":
                    break;
                case "SheepIncrease":
                    break;
                // Wolf Base Skill
                // Unit Skill
                default:
                    // 스킬트리를 참조해서 조건에 맞으면 스킬 업그레이드
                    List<string> precedeSkills = GameData.SkillTree[_skillName].ToList();
                    List<string> result = (from s in GameData.SkillUpgradedList select s).Intersect(precedeSkills).ToList();
                    bool isEqual = precedeSkills.OrderBy(a => a).SequenceEqual(result.OrderBy(a => a));
                    
                    if (precedeSkills[0] == "free" || isEqual)
                    {
                        ui.SetAlpha(ui.DictSkillBtn[currentSkillButton.name], 1.0f);
                        GameData.SkillUpgradedList.Add(_skillName);
                        upgradeSkillEvent.Invoke();
                    }
                    // 튤립버튼 설정 -> 진척도에 따라 색이 차도록
                    break;
            }
            playerController.Resource -= _cost;
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
    
    private int SetFenceRepairCost()
    {
        GameObject[] fences = GameObject.FindGameObjectsWithTag("Fence");
        int lv = GameData.StorageLevel;
        int cost = 0;
        float param = (float)((lv + 5) / (Math.Pow(lv, 2) + 5));
        for (var i = 0; i < fences.Length; i++)
        {
            GameObject fence = fences[i];
            Stat stat = fence.GetComponent<Stat>();
            int diff = stat.MaxHp - stat.Hp;
            cost += diff;
        }
        cost = (int)(cost * param);

        return cost;
    }
}

