using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MainLobbyWolf : UI_Scene
{
    enum Buttons
    {
        SheepButton,
        RankButton,
        SingleButton,
        MultiButton,
        ChatButton,
        ClanButton,
        NewsButton,
    }

    enum Texts
    {
        GoldText,
        GemText,
        CloverText,
        CloverTimeText,
        
        RankScoreText,
        RankingText,
        RankNameText,
    }

    enum Images
    {
        GoldImage,
        GemImage,
        CloverImage,
        CloverTimeImage,
        RankStar,
        RankTextIcon,
        SingleButtonImage,
        MultiButtonImage,
        ChatImageIcon,
        ClanIcon,
        NoticeIcon,
        CharacterButtonIcon,
        GameButtonIcon,
        ShopButtonIcon,
    }

    protected override void Init()
    {
        base.Init();

        BindObjects();
        SetButtonEvents();
        SetUI();
    }

    private void OnSheepClicked(PointerEventData data)
    {
        Managers.UI.ShowSceneUI<UI_MainLobbySheep>();
        Managers.Resource.Destroy(gameObject);
    }

    private void OnSingleClicked(PointerEventData data)
    {
    }

    private void OnMultiClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_Dim>();
        Managers.UI.ShowPopupUI<UI_BattlePopupWolf>();
    }
    
    protected override void BindObjects()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
    }

    protected override void SetButtonEvents()
    {
        GetButton((int)Buttons.SheepButton).gameObject.BindEvent(OnSheepClicked);
        GetButton((int)Buttons.SingleButton).gameObject.BindEvent(OnSingleClicked);
        GetButton((int)Buttons.MultiButton).gameObject.BindEvent(OnMultiClicked);
    }
    
    protected override void SetUI()
    {
        SetObjectSize(GetButton((int)Buttons.SheepButton).gameObject, 1.0f);
        SetObjectSize(GetImage((int)Images.GoldImage).gameObject, 1.2f);
        SetObjectSize(GetImage((int)Images.GemImage).gameObject, 1.2f);
        SetObjectSize(GetImage((int)Images.CloverImage).gameObject, 0.9f);
        SetObjectSize(GetImage((int)Images.CloverTimeImage).gameObject, 0.9f);

        SetObjectSize(GetImage((int)Images.RankStar).gameObject, 0.9f);
        SetObjectSize(GetImage((int)Images.RankTextIcon).gameObject, 0.25f);
        SetObjectSize(GetImage((int)Images.SingleButtonImage).gameObject, 0.7f);
        SetObjectSize(GetImage((int)Images.MultiButtonImage).gameObject, 0.7f);
        
        SetObjectSize(GetImage((int)Images.ChatImageIcon).gameObject, 0.9f);
        SetObjectSize(GetImage((int)Images.ClanIcon).gameObject, 0.6f);
        SetObjectSize(GetImage((int)Images.NoticeIcon).gameObject, 0.6f);
        
        SetObjectSize(GetImage((int)Images.CharacterButtonIcon).gameObject, 0.8f);
        SetObjectSize(GetImage((int)Images.GameButtonIcon).gameObject, 0.8f);
        SetObjectSize(GetImage((int)Images.ShopButtonIcon).gameObject, 0.8f);
    }
}
