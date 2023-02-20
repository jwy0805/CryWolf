using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UI_GameSheep : UI_Scene
{
    private GameObject _selectedObj;
    private GameObject _selectedBefore;
    private GameObject _onSelectedPortrait;
    private GameObject _offSelectedPortrait;
    private GameObject _onSelectedSkill;
    private GameObject _offSelectedSkill;
    private DeliverGameObject _dgo;
    
    private int _gold;
    private UI_Portrait _isActive;

    #region UIDictionary

    private Dictionary<string, GameObject> _dictBtn = new ();
    private Dictionary<string, GameObject> _dictImg = new ();
    private Dictionary<string, GameObject> _dictTxt = new ();
    private Dictionary<string, GameObject> _dictSkillBtn = new ();
    private Dictionary<string, GameObject> _dictSkillBtnPanel = new ();
    private Dictionary<string, GameObject> _dictSkillPanel = new ();
    private Dictionary<string, GameObject> _dictLine = new ();
    private Dictionary<string, GameObject> _dictPortrait = new ();

    public Dictionary<string, GameObject> DictSkillBtn
    {
        get => _dictSkillBtn;
        set => _dictSkillBtn = value;
    }

    #endregion

    public GameObject SelectedObj
    {
        get => _selectedObj;
        set
        {
            if (_selectedObj != null)
            {
                SelectedBefore = _selectedObj;
            }
            
            _selectedObj = value;
            // Debug.Log(_selectedObj.name);
        }
    }

    public GameObject SelectedBefore
    {
        get => _selectedBefore;
        set
        {
            _selectedBefore = value;
        }
    }
    
    private GameObject OnSelectedPortrait
    {
        get => _onSelectedPortrait;
        set
        {
            _onSelectedPortrait = value;
            PortraitOn(_onSelectedPortrait);
            SetTulipButton(_onSelectedPortrait);
        }
    }

    private GameObject OffSelectedPortrait
    {
        get => _offSelectedPortrait;
        set
        {
            _offSelectedPortrait = value;
            PortraitOff(_offSelectedPortrait);
        }
    }

    public GameObject OnSelectedSkill
    {
        get => _onSelectedSkill;
        set
        {
            _onSelectedSkill = value;
            Image frame = _onSelectedSkill.transform.parent.parent.GetChild(1).GetComponent<Image>();
            frame.color = Color.cyan;
        }
    }

    public GameObject OffSelectedSkill
    {
        get => _offSelectedSkill;
        set
        {
            _offSelectedSkill = value;
            Managers.UI.ClosePopupUI();
            Image frame = _offSelectedSkill.transform.parent.parent.GetChild(1).GetComponent<Image>();
            frame.color = Color.green;
        }
    }
    
    enum Buttons
    {
        TulipButton,
        
        WestSpawnButton,
        NorthSpawnButton,
        EastSpawnButton,
    }

    enum Portraits
    {
        BudButton,
        BloomButton,
        BlossomButton,
        PracticeDummyButton,
        TargetDummyButton,
        TrainingDummyButton,
        SunBlossomButton,
        SunflowerFairyButton,
        SunfloraPixieButton,
        MothLunaButton,
        MothMoonButton,
        MothCelestialButton,
        SoulButton,
        HauntButton,
        SoulMageButton,
    }

    enum SkillButtons
    {
        BudAttackButton,
        BudAttackSpeedButton,
        BudRangeButton,
        BudSeedButton,
        BudDoubleButton,
        
        BloomAttackButton,
        BloomAttackSpeedButton,
        BloomRangeButton,
        BloomAttackSpeed2Button,
        Bloom3ComboButton,
        BloomAirAttackButton,
        
        BlossomPoisonButton,
        BlossomAccuracyButton,
        BlossomAttackButton,
        BlossomAttackSpeedButton,
        BlossomRangeButton,
        BlossomDeathButton,
        
        PracticeDummyHealthButton,
        PracticeDummyDefenceButton,
        PracticeDummyHealth2Button,
        PracticeDummyDefence2Button,
        PracticeDummyAggroButton,
        
        TargetDummyHealthButton,
        TargetDummyHealButton,
        TargetDummyFireResistButton,
        TargetDummyPoisonResistButton,
        TargetDummyReflectionButton,
        
        TrainingDummyAggroButton,
        TrainingDummyHealButton,
        TrainingDummyFaintButton,
        TrainingDummyHealthButton,
        TrainingDummyDefenceButton,
        TrainingDummyFireResistButton,
        TrainingDummyPoisonResistButton,
        TrainingDummyDebuffRemoveButton,
        
        SunBlossomHealthButton,
        SunBlossomSlowButton,
        SunBlossomHealButton,
        SunBlossomSlowAttackButton,
        
        SunflowerFairyAttackButton,
        SunflowerFairyDefenceButton,
        SunflowerFairyDoubleButton,
        SunflowerFairyMpDownButton,
        SunflowerFairyFenceHealButton,
        
        SunfloraPixieCurseButton,
        SunfloraPixieHealButton,
        SunfloraPixieRangeButton,
        SunfloraPixieFaintButton,
        SunfloraPixieAttackSpeedButton,
        SunfloraPixieTripleButton,
        SunfloraPixieDebuffRemoveButton,
        SunfloraPixieAttackButton,
        SunfloraPixieInvincibleButton,
        
        MothLunaAttackButton,
        MothLunaSpeedButton,
        MothLunaAccuracyButton,
        MothLunaFaintButton,
        
        MothMoonRemoveDebuffSheepButton,
        MothMoonHealSheepButton,
        MothMoonRangeButton,
        MothMoonOutputButton,
        MothMoonAttackSpeedButton,
        
        MothCelestialSheepHealthButton,
        MothCelestialGroundAttackButton,
        MothCelestialAccuracyButton,
        MothCelestialFireResistButton,
        MothCelestialPoisonResistButton,
        MothCelestialPoisonButton,
        MothCelestialBreedSheepButton,
        
        SoulAttackButton,
        SoulDefenceButton,
        SoulHealthButton,
        SoulDrainButton,
        
        HauntLongAttackButton,
        HauntAttackSpeedButton,
        HauntAttackButton,
        HauntPoisonResistButton,
        HauntFireResistButton,
        HauntFireButton,
        
        SoulMageAvoidButton,
        SoulMageDefenceAllButton,
        SoulMageFireDamageButton,
        SoulMageShareDamageButton,
        SoulMageTornadoButton,
        SoulMageDebuffResistButton,
        SoulMageNatureAttackButton,
        SoulMageCriticalButton,
    }

    enum SkillButtonPanels
    {
        BudAttackPanel,
        BudAttackSpeedPanel,
        BudRangePanel,
        BudSeedPanel,
        BudDoublePanel,
        BudLine1,
        BudLine2,
        
        BloomAttackPanel,
        BloomAttackSpeedPanel,
        BloomRangePanel,
        BloomAttackSpeed2Panel,
        Bloom3ComboPanel,
        BloomAirAttackPanel,
        BloomLine1,

        BlossomPoisonPanel,
        BlossomAccuracyPanel,
        BlossomAttackPanel,
        BlossomAttackSpeedPanel,
        BlossomRangePanel,
        BlossomDeathPanel,
        BlossomLine1,
        BlossomLine2,
        
        PracticeDummyHealthPanel,
        PracticeDummyDefencePanel,
        PracticeDummyHealth2Panel,
        PracticeDummyDefence2Panel,
        PracticeDummyAggroPanel,
        
        TargetDummyHealthPanel,
        TargetDummyHealPanel,
        TargetDummyFireResistPanel,
        TargetDummyPoisonResistPanel,
        TargetDummyReflectionPanel,
        
        TrainingDummyAggroPanel,
        TrainingDummyHealPanel,
        TrainingDummyFaintPanel,
        TrainingDummyHealthPanel,
        TrainingDummyDefencePanel,
        TrainingDummyFireResistPanel,
        TrainingDummyPoisonResistPanel,
        TrainingDummyDebuffRemovePanel,
        
        SunBlossomHealthPanel,
        SunBlossomSlowPanel,
        SunBlossomHealPanel,
        SunBlossomSlowAttackPanel,
        
        SunflowerFairyAttackPanel,
        SunflowerFairyDefencePanel,
        SunflowerFairyDoublePanel,
        SunflowerFairyMpDownPanel,
        SunflowerFairyFenceHealPanel,

        SunfloraPixieCursePanel,
        SunfloraPixieHealPanel,
        SunfloraPixieRangePanel,
        SunfloraPixieFaintPanel,
        SunfloraPixieAttackSpeedPanel,
        SunfloraPixieTriplePanel,
        SunfloraPixieDebuffRemovePanel,
        SunfloraPixieAttackPanel,
        SunfloraPixieInvinciblePanel,
        
        MothLunaAttackPanel,
        MothLunaSpeedPanel,
        MothLunaAccuracyPanel,
        MothLunaFaintPanel,
        
        MothMoonRemoveDebuffSheepPanel,
        MothMoonHealSheepPanel,
        MothMoonRangePanel,
        MothMoonOutputPanel,
        MothMoonAttackSpeedPanel,
        
        MothCelestialSheepHealthPanel,
        MothCelestialGroundAttackPanel,
        MothCelestialAccuracyPanel,
        MothCelestialFireResistPanel,
        MothCelestialPoisonResistPanel,
        MothCelestialPoisonPanel,
        MothCelestialBreedSheepPanel,
        
        SoulAttackPanel,
        SoulDefencePanel,
        SoulHealthPanel,
        SoulDrainPanel,
        
        HauntLongAttackPanel,
        HauntAttackSpeedPanel,
        HauntAttackPanel,
        HauntPoisonResistPanel,
        HauntFireResistPanel,
        HauntFirePanel,
        HauntLine1,
        
        SoulMageAvoidPanel,
        SoulMageDefenceAllPanel,
        SoulMageFireDamagePanel,
        SoulMageShareDamagePanel,
        SoulMageTornadoPanel,
        SoulMageDebuffResistPanel,
        SoulMageNatureAttackPanel,
        SoulMageCriticalPanel,
    }

    enum SkillPanels
    {
        BudSkillPanel,
        BloomSkillPanel,  
        BlossomSkillPanel,
        PracticeDummySkillPanel,  
        TargetDummySkillPanel,
        TrainingDummySkillPanel,
        SunBlossomSkillPanel, 
        SunflowerFairySkillPanel, 
        SunfloraPixieSkillPanel,  
        MothLunaSkillPanel,
        MothMoonSkillPanel,
        MothCelestialSkillPanel,  
        SoulSkillPanel,
        HauntSkillPanel,  
        SoulMageSkillPanel,

    }
    
    enum Images
    {
        UnitPanel0,
        UnitPanel1,
        UnitPanel2,
        UnitPanel3,
        UnitPanel4,

        UnitFrame,
        SkillPanel,
        
        SpawnButtonPanel,
    }
    
    enum Lines
    {
        BloomLine1,
        BlossomLine1,
        BlossomLine2,
        BudLine1,
        BudLine2,
        HauntLine1,
        HauntLine2,
        MothCelestialLine1,
        MothCelestialLine2,
        MothMoonLine1,
        MothLunaLine1,
        PracticeDummyLine1,
        SoulMageLine1,
        SoulMageLine2,
        SoulLine1,
        SunfloraPixieLine1,
        SunfloraPixieLine2,
        SunflowerFairyLine1,
        TargetDummyLine1,
        TrainingDummyLine1,
        TrainingDummyLine2,
    }

    enum Texts
    {
        CurrentName,
        CurrentPercent,
    }

    protected override void Init()
    {
        base.Init();

        BindObjects();
        SetButtonEvents();
        SetUI();
        
        _dgo = gameObject.GetComponent<DeliverGameObject>();
        _gold = 500;
    }

    protected override void BindObjects()
    {
        BindData<Button>(typeof(Buttons), _dictBtn);
        BindData<Button>(typeof(Portraits), _dictPortrait);
        BindData<Image>(typeof(Images), _dictImg);
        BindData<TextMeshProUGUI>(typeof(Texts), _dictTxt);
        BindData<Image>(typeof(SkillPanels), _dictSkillPanel);
        
        BindData<Button>(typeof(SkillButtons), _dictSkillBtn);
        BindData<Image>(typeof(SkillButtonPanels), _dictSkillBtnPanel);
        BindData<Image>(typeof(Lines), _dictLine);
    }

    private void BindData<T>(Type enumType, Dictionary<string, GameObject> dict) where T : Object
    {
        Bind<T>(enumType);
        
        if (typeof(T) == typeof(Button))
        {
            for (int i = 0; i < _objects[typeof(T)].Length; i++)
            {
                GameObject btn = GetButton(i).gameObject;
                dict.Add(btn.name, btn);
            }
        }
        else if (typeof(T) == typeof(Image))
        {
            for (int i = 0; i < _objects[typeof(T)].Length; i++)
            {
                GameObject img = GetImage(i).gameObject;
                dict.Add(img.name, img);
            }
        }
        else if (typeof(T) == typeof(Text))
        {
            for (int i = 0; i < _objects[typeof(T)].Length; i++)
            {
                GameObject txt = GetText(i).gameObject;
                dict.Add(txt.name, txt);
            }
        }
        
        _objects.Clear();
    }
    
    protected override void SetButtonEvents()
    {
        foreach (var item in _dictPortrait)
        {
            item.Value.GetComponent<Button>().onClick.AddListener(OnPortraitClicked);
        }

        foreach (var item in _dictSkillBtn)
        {
            item.Value.GetComponent<Button>().onClick.AddListener(OnSkillClicked);
        }
        
        _dictBtn["TulipButton"].BindEvent(OnTulipClicked);
        _dictBtn["WestSpawnButton"].BindEvent(OnWestSpawnClicked);
        _dictBtn["NorthSpawnButton"].BindEvent(OnNorthSpawnClicked);
    }

    private void SetTulipButton(GameObject go)
    {
        string level = GetLevelFromUIObject(go, "Button");
        Transform tf = _dictBtn["TulipButton"].transform;
        Button btn = _dictBtn["TulipButton"].GetComponent<Button>();
        btn.interactable = true;

        for (int i = 0; i < tf.childCount; i++)
        {
            tf.GetChild(i).gameObject.SetActive(false);
        }
        
        switch (level)
        {
            case "0":
                _isActive = go.GetComponent<UI_Portrait>();
                
                if (_isActive.IsActive == false)
                {
                    tf.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    tf.GetChild(1).gameObject.SetActive(true);
                }
                break;
            
            case "1":
                tf.GetChild(1).gameObject.SetActive(true);
                break;
            
            case "2":
                tf.GetChild(2).gameObject.SetActive(true);
                btn.interactable = false;
                break;
        }
    }
    
    protected override void SetUI()
    {
        SetSkillPanel();
        
        foreach (var item in _dictPortrait)
        {
            SetObjectSize(item.Value, 1.0f);
            _isActive = item.Value.GetComponent<UI_Portrait>();
            _isActive.IsActive = false;
        }

        SetObjectSize(_dictImg["UnitFrame"], 1.0f);
        _dictImg["UnitFrame"].SetActive(false);
        SetObjectSize(_dictBtn["TulipButton"], 0.95f);
        _dictBtn["TulipButton"].transform.GetChild(0).gameObject.SetActive(false);
        _dictBtn["TulipButton"].transform.GetChild(1).gameObject.SetActive(true);
        _dictBtn["TulipButton"].transform.GetChild(2).gameObject.SetActive(false);

        _dictImg["SpawnButtonPanel"].SetActive(false);
        
        SetButtons();
    }

    private void SetButtons()
    {
        // 0단계 초상화만 패널에 뜨고 나머지는 비활성화
        foreach (var item in GameData.MonsterSheep)
        {
            string level = item.Key.Substring(1, 1);
            
            if (level == "0")
            {
                SetAlpha(_dictPortrait[$"{item.Value}Button"], 0.6f);
            }
            else
            {
                _dictPortrait[$"{item.Value}Button"].SetActive(false);
            }
        }
        
        _dictBtn["TulipButton"].transform.GetChild(0).gameObject.SetActive(false);
    }
    
    private void SetSkillPanel()
    {
        foreach (var item in _dictSkillBtnPanel)
        {
            SetObjectSize(item.Value, 0.22f);    
        }
        
        foreach (var item in _dictSkillBtn)
        {
            SetAlpha(item.Value, 0.6f);
        }

        foreach (var item in _dictSkillPanel)
        {
            item.Value.SetActive(false);
        }
        
        SetLines();
    }
    
    public void SetAlpha(GameObject go, float alpha)
    {
        Image img = go.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    private void SetLines()
    {
        float[] lineSize = new[] 
        {
            0.3f, 0.6f, 0.6f, 0.3f, 0.45f, 0.6f, 0.3f, 0.3f, 0.3f, 0.3f,
            0.3f, 0.3f, 0.6f, 0.6f, 0.6f, 0.3f, 0.3f, 0.3f, 0.6f, 0.75f, 0.25f
        };
        string[] lineName = Enum.GetNames(typeof(Lines));

        for (int i = 0; i < lineSize.Length; i++)
        {
            SetLineSize(_dictLine[lineName[i]], lineSize[i]);
        }
    }
    
    private string GetLevelFromUIObject(GameObject go, string oldValue)
    {
        string monsterName = go.name.Replace(oldValue, "");
        string num = GameData.MonsterSheep.FirstOrDefault(item => item.Value == monsterName).Key;
        string level = num.Substring(1, 1);

        return level;
    }
    
    private void PortraitOn(GameObject portraits)
    {
        string monsterName = portraits.name.Replace("Button", "");
        var parent = portraits.transform.parent;
        RectTransform rectTransform = _dictImg["UnitFrame"].GetComponent<RectTransform>();

        _dictSkillPanel[$"{monsterName}SkillPanel"].SetActive(true);
        
        _dictImg["UnitFrame"].transform.SetParent(parent, false);
        _dictImg["UnitFrame"].SetActive(true);
        rectTransform.sizeDelta = portraits.GetComponent<RectTransform>().sizeDelta;
    }

    private void PortraitOff(GameObject portraits)
    {
        string monsterName = portraits.name.Replace("Button", ""); 
        _dictSkillPanel[$"{monsterName}SkillPanel"].SetActive(false);
    }
    
    #region ButtonFunction
    
    private void OnPortraitClicked()
    {
        SelectedObj = EventSystem.current.currentSelectedGameObject;
        _dictImg["SpawnButtonPanel"].SetActive(true);
        
        if (OnSelectedPortrait != null)
        {
            OffSelectedPortrait = OnSelectedPortrait;
        }
    
        OnSelectedPortrait = SelectedObj;
    }
    
    private void OnSkillClicked()
    {
        // 스킬 활성화, 튤립버튼 fill 적용
        SelectedObj = EventSystem.current.currentSelectedGameObject;

        if (OnSelectedSkill != null)
        {
            OffSelectedSkill = OnSelectedSkill;
        }

        OnSelectedSkill = SelectedObj;

        Managers.UI.ShowPopupUI<UI_UpgradePopup>();
    }

    private void OnTulipClicked(PointerEventData data)
    {
        if (OnSelectedPortrait != null)
        {
            int level = Int32.Parse(GetLevelFromUIObject(OnSelectedPortrait, "Button"));
            _isActive = OnSelectedPortrait.GetComponent<UI_Portrait>();
        
            int[] needGold = { 400, 450, 500 };
            if ( _gold >= needGold[level])
            {
                GameObject newPortrait;
                if (_isActive.IsActive)
                {
                    if (level < 2)
                    {
                        newPortrait = OnSelectedPortrait.transform.parent.GetChild(level + 1).gameObject;
                    }
                    else return;
                }
                else
                {
                    _isActive.IsActive = true;
                    newPortrait = OnSelectedPortrait.transform.parent.GetChild(level).gameObject;
                }
                
                if (OnSelectedPortrait != null)
                {
                    OffSelectedPortrait = OnSelectedPortrait;
                }

                OnSelectedPortrait = newPortrait;
                OnSelectedPortrait.GetComponent<UI_Portrait>().IsActive = true;
            }
            else
            {
                Debug.Log("Need more gold.");
            }
        }
    }

    private void OnWestSpawnClicked(PointerEventData data)
    {
        if (OnSelectedPortrait == null) return;
        _dgo.Spawn = OnSelectedPortrait;
        _dgo.Way = Define.Way.West;
    }

    private void OnNorthSpawnClicked(PointerEventData data)
    {
        // lock
        if (OnSelectedPortrait == null) return;
        _dgo.Spawn = OnSelectedPortrait;
        _dgo.Way = Define.Way.North;
    }
    
    #endregion
}
