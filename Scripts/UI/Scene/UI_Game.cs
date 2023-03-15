using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UI_Game : UI_Scene
{
    private GameObject _selectedObj;
    private GameObject _selectedBefore;
    private GameObject _onSelectedPortrait;
    private GameObject _offSelectedPortrait;
    private GameObject _onSelectedSkill;
    private GameObject _offSelectedSkill;
    private bool _capacityButton;
    private DeliverGameObject _dgo;
    private UI_Portrait _isActive;
    private string _side;
    private PlayerController _player;
    
    #region UIDictionaray
    
    private Dictionary<string, GameObject> _dictCommonBtn;
    private Dictionary<string, GameObject> _dictCommonImg;
    private Dictionary<string, GameObject> _dictCommonTxt;
    
    private Dictionary<string, GameObject> _dictBtn;
    private Dictionary<string, GameObject> _dictImg;
    private Dictionary<string, GameObject> _dictTxt;
    private Dictionary<string, GameObject> _dictSkillPanel;
    private Dictionary<string, GameObject> _dictSkillBtn;
    private Dictionary<string, GameObject> _dictLine;
    private Dictionary<string, GameObject> _dictPortrait;

    public Dictionary<string, GameObject> DictSkillBtn => _dictSkillBtn;
    public Dictionary<string, GameObject> DictTxt => _dictTxt;
    
    #endregion
    
    #region Properties
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
            // 
            PortraitOn(_onSelectedPortrait);
            SetUpgradeButton(_onSelectedPortrait);
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

    public bool CapacityButton
    {
        get => _capacityButton;
        set
        {
            _capacityButton = value;
            _dictSkillPanel["SheepSkillPanel"].SetActive(_capacityButton);
            if (OnSelectedPortrait != null) PortraitOff(OnSelectedPortrait);
        }
    }
    
    #endregion

    protected override void Init()
    {
        base.Init();

        _side = Util.SheepOrWolf;
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var go in gameObjects)
        {
            if (Enum.IsDefined(typeof(Define.SheepCharacter), go.name) && _side == "Sheep")
            {
                _player = go.GetComponent<PlayerController>();
            }
            else if (Enum.IsDefined(typeof(Define.WolfCharacter), go.name) && _side == "Wolf")
            {
                _player = go.GetComponent<PlayerController>();
            }
        }
    }
    
    protected override void BindObjects()
    {
        BindData<Button>(typeof(CommonButtons), _dictCommonBtn);
        BindData<Image>(typeof(CommonImages), _dictCommonImg);
        BindData<TextMeshProUGUI>(typeof(CommonTexts), _dictCommonTxt);
        
        if (_side == "Sheep")
        {
            BindData<Button>(typeof(SheepButtons), _dictBtn);
            BindData<Button>(typeof(SheepPortraits), _dictPortrait);
            BindData<Image>(typeof(SheepSkillPanels), _dictSkillPanel);
            BindData<Button>(typeof(SheepSkillButtons), _dictSkillBtn);
            BindData<Image>(typeof(SheepLines), _dictLine);
        }
        else
        {
            BindData<Button>(typeof(WolfButtons), _dictBtn);
            BindData<Button>(typeof(WolfPortraits), _dictPortrait);
            BindData<Image>(typeof(WolfSkillPanels), _dictSkillPanel);
            BindData<Button>(typeof(WolfSkillButtons), _dictSkillBtn);
            BindData<Image>(typeof(WolfLines), _dictLine);
        }
        
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
        else if (typeof(T) == typeof(TextMeshProUGUI))
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
        
        _dictBtn["CapacityButton"].BindEvent(OnCapacityClicked);
        _dictBtn["WestSpawnButton"].BindEvent(OnWestSpawnClicked);
        _dictBtn["NorthSpawnButton"].BindEvent(OnNorthSpawnClicked);

        if (_side == "Sheep")
        {
            _dictBtn["TulipButton"].BindEvent(OnUpgradeClicked);
        }
        else
        {
            _dictBtn["DnaButton"].BindEvent(OnUpgradeClicked);
        }
    }
    
    private void SetUpgradeButton(GameObject go)
    {
        string level = GetLevelFromUIObject(go, "Button");
        Transform tf = _side == "Sheep" ? _dictBtn["TulipButton"].transform : _dictBtn["DnaButton"].transform;
        Button btn = _side == "Sheep" ? 
            _dictBtn["TulipButton"].GetComponent<Button>() : _dictBtn["DnaButton"].GetComponent<Button>();
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
    
    private void SetTexts()
    {
        _dictTxt["GoldText"].GetComponent<TextMeshProUGUI>().text = "0";
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
        foreach (var item in GameData.Tower)
        {
            string level = item.Key.Substring(1, 1);
            
            if (level == "0")
            {
                _dictPortrait[$"{item.Value}Button"].GetComponent<UI_Portrait>().IsActive = true;
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
        foreach (var item in DictSkillBtn)
        {
            SetAlpha(item.Value, Enum.IsDefined(typeof(BaseSkillButtons), item.Key) ? 1.0f : 0.6f);
            SetObjectSize(item.Value.transform.parent.parent.gameObject, 0.22f);
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
        float[] lineSizeSheep =
        {
            0.3f, 0.6f, 0.6f, 0.3f, 0.45f, 0.6f, 0.3f, 0.3f, 0.3f, 0.3f,
            0.3f, 0.3f, 0.6f, 0.6f, 0.6f, 0.3f, 0.3f, 0.3f, 0.6f, 0.75f, 0.25f
        };
        float[] lineSizeWolf =
        {
            0.3f, 0.6f, 0.3f, 0.6f, 0.6f, 0.6f, 0.3f, 0.6f, 0.6f, 0.6f,
            0.6f, 0.3f, 0.6f, 0.6f, 0.3f, 0.6f, 0.6f,
        };
        string[] lineName;
        float[] lineSize;
        
        if (_side == "Sheep")
        {
            lineName = Enum.GetNames(typeof(SheepLines));
            lineSize = lineSizeSheep;
        }
        else
        {
            lineName = Enum.GetNames(typeof(WolfLines));
            lineSize = lineSizeWolf;
        }

        for (int i = 0; i < lineSize.Length; i++)
        {
            SetLineSize(_dictLine[lineName[i]], lineSize[i]);
        }
    }
    
    private string GetLevelFromUIObject(GameObject go, string oldValue)
    {
        string towerName = go.name.Replace(oldValue, "");
        string num = GameData.Tower.FirstOrDefault(item => item.Value == towerName).Key;
        string level = num.Substring(1, 1);

        return level;
    }
    
    private void PortraitOn(GameObject portraits)
    {
        CapacityButton = false;
        string unitName = portraits.name.Replace("Button", "");
        var parent = portraits.transform.parent;
        RectTransform rectTransform = _dictImg["UnitFrame"].GetComponent<RectTransform>();

        _dictSkillPanel[$"{unitName}SkillPanel"].SetActive(true);
        
        _dictImg["UnitFrame"].transform.SetParent(parent, false);
        _dictImg["UnitFrame"].SetActive(true);
        rectTransform.sizeDelta = portraits.GetComponent<RectTransform>().sizeDelta;
    }

    private void PortraitOff(GameObject portraits)
    {
        string unitName = portraits.name.Replace("Button", ""); 
        _dictSkillPanel[$"{unitName}SkillPanel"].SetActive(false);
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

    private void OnCapacityClicked(PointerEventData data)
    {
        CapacityButton = true;
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

    private void OnUpgradeClicked(PointerEventData data)
    {
        if (OnSelectedPortrait != null)
        {
            int level = Int32.Parse(GetLevelFromUIObject(OnSelectedPortrait, "Button"));
            string unitName = OnSelectedPortrait.name.Replace("Button", ""); 
            _isActive = OnSelectedPortrait.GetComponent<UI_Portrait>();

            int cost = GameData.UnitUpgradeCost[(Define.UnitId)Enum.Parse(typeof(Define.UnitId), unitName)];
            if (_player.Resource >= cost)
            {
                _player.Resource -= cost;
                
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

    private void OnEastSpawnClicked(PointerEventData data)
    {
        // lock
        if (OnSelectedPortrait == null) return;
        _dgo.Spawn = _onSelectedPortrait;
        _dgo.Way = Define.Way.East;
    }
    
    #endregion
    
    #region Enum

    private enum CommonButtons
    {
        WestSpawnButton,
        NorthSpawnButton,
        EastSpawnButton,
    }
    private enum CommonImages
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
    private enum CommonTexts
    {
        CurrentName,
        CurrentPercent,
        
        ResourceText,
        WestCapacityText,
        NorthCapacityText,
        EastCapacityText,
        MaxCapacityText,
    }
    private enum BaseSkillButtons
    {
        FenceRepairButton,
        StorageLvUpButton,
        GoldIncreaseButton,
        SheepHealthButton,
        SheepIncreaseButton,
        
        MonsterCapacityButton,
        DnaIncreaseButton,
        RestoreCaveButton,
        CardUpgradeButton,
        BlackMagicCardButton,
    }
    
    private enum SheepButtons
    {
        TulipButton,

        GoldButton,
        CapacityButton,
    }
    private enum SheepPortraits
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
    private enum SheepBaseSkillButtons
    {
        FenceRepairButton,
        StorageLvUpButton,
        GoldIncreaseButton,
        SheepHealthButton,
        SheepIncreaseButton,
    }
    private enum SheepSkillButtons
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
        
        FenceRepairButton,
        StorageLvUpButton,
        GoldIncreaseButton,
        SheepHealthButton,
        SheepIncreaseButton,
    }
    private enum SheepSkillPanels
    {
        SheepSkillPanel,
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
    private enum SheepLines
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
    private enum SheepTexts
    {
        
    }
    
    private enum WolfButtons
    {
        DnaButton,

        ResourceButton,
        CapacityButton,
    }
    private enum WolfPortraits
    {
        WolfPupButton,
        WolfButton,
        WerewolfButton,
        LurkerButton,
        CreeperButton,
        HorrorButton,
        SnakeletButton,
        SnakeButton,
        SnakeNagaButton,
        MosquitoBugButton,
        MosquitoPesterButton,
        MosquitoStingerButton,
        ShellButton,
        SpikeButton,
        HermitButton,
    }
    private enum WolfBaseSkillButtons
    {
        MonsterCapacityButton,
        DnaIncreaseButton,
        RestoreCaveButton,
        CardUpgradeButton,
        BlackMagicCardButton,
    }
    private enum WolfSkillButtons
    {
        WolfPupSpeedButton,
        WolfPupHealthButton,
        WolfPupAttackButton,
        WolfPupAttackSpeedButton,
        
        WolfDrainButton,
        WolfDefenceButton,
        WolfAvoidButton,
        WolfCriticalButton,
        WolfFireResistButton,
        WolfPoisonResistButton,
        WolfDnaButton,
        
        WerewolfThunderButton,
        WerewolfDebuffResistButton,
        WerewolfFaintButton,
        WerewolfHealthButton,
        WerewolfEnhanceButton,
        
        LurkerSpeedButton,
        LurkerHealthButton,
        LurkerDefenceButton,
        LurkerHealth2Button,
        
        CreeperSpeedButton,
        CreeperAttackSpeedButton,
        CreeperAttackButton,
        CreeperRollButton,
        CreeperPoisonButton,
        
        HorrorRollPoisonButton,
        HorrorPoisonStackButton,
        HorrorHealthButton,
        HorrorPoisonResistButton,
        HorrorDefenceButton,
        HorrorPoisonBeltButton,
        
        SnakeletSpeedButton,
        SnakeletRangeButton,
        SnakeletAttackSpeedButton,
        SnakeletAttackButton,
        
        SnakeAttackButton,
        SnakeAttackSpeedButton,
        SnakeRangeButton,
        SnakeAccuracyButton,
        SnakeFireButton,
        
        SnakeNagaAttackButton,
        SnakeNagaRangeButton,
        SnakeNagaFireResistButton,
        SnakeNagaCriticalButton,
        SnakeNagaDrainButton,
        SnakeNagaMeteorButton,
        
        MosquitoBugSpeedButton,
        MosquitoBugDefenceButton,
        MosquitoBugAvoidButton,
        MosquitoBugWoolDownButton,
        
        MosquitoPesterAttackButton,
        MosquitoPesterHealthButton,
        MosquitoPesterWoolDown2Button,
        MosquitoPesterWoolRateButton,
        MosquitoPesterWoolStopButton,
        
        MosquitoStingerLongAttackButton,
        MosquitoStingerHealthButton,
        MosquitoStingerAvoidButton,
        MosquitoStingerPoisonButton,
        MosquitoStingerPoisonResistButton,
        MosquitoStingerInfectionButton,
        MosquitoStingerSheepDeathButton,
        
        ShellAttackSpeedButton,
        ShellSpeedButton,
        ShellHealthButton,
        ShellRollButton,
        
        SpikeSelfDefenceButton,
        SpikeLostHealButton,
        SpikeDefenceButton,
        SpikeAttackButton,
        SpikeDoubleBuffButton,
        
        HermitFireResistButton,
        HermitPoisonResistButton,
        HermitDebuffRemoveButton,
        HermitRangeButton,
        HermitAggroButton,
        HermitReflectionButton,
        HermitFaintButton,
        
        MonsterCapacityButton,
        DnaIncreaseButton,
        RestoreCaveButton,
        CardUpgradeButton,
        BlackMagicCardButton,
    }
    private enum WolfSkillPanels
    {
        DnaSkillPanel,
        WolfPupSkillPanel,
        WolfSkillPanel,
        WerewolfSkillPanel,
        LurkerSkillPanel,
        CreeperSkillPanel,
        HorrorSkillPanel,
        SnakeletSkillPanel,
        SnakeSkillPanel,
        SnakeNagaSkillPanel,
        MosquitoBugSkillPanel,
        MosquitoPesterSkillPanel,
        MosquitoStingerSkillPanel,
        ShellSkillPanel,
        SpikeSkillPanel,
        HermitSkillPanel,
    }
    private enum WolfLines
    {
        WolfLine1,
        WolfLine2,
        WerewolfLine1,
        CreeperLine1,
        HorrorLine1,
        HorrorLine2,
        SnakeletLine1,
        SnakeLine1,
        SnakeNagaLine1,
        SnakeNagaLine2,
        MosquitoBugLine1,
        MosquitoPesterLine1,
        MosquitoStingerLine1,
        ShellLine1,
        SpikeLine1,
        HermitLine1,
        HermitLine2,
    }
    private enum WolfTexts
    {
        
    }
    
    #endregion
}
