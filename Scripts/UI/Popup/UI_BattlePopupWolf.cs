using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class UI_BattlePopupWolf : UI_Popup
{
    enum Buttons
    {
        ExitButton,
        RankGameButton,
        ExhibitionGameButton,
    }

    enum Images
    {
        WolfIcon,
    }

    protected override void Init()
    {
        base.Init();
        
        BindObjects();
        SetButtonEvents();
        SetUI();
    }

    private void OnExitClicked(PointerEventData data)
    {
        Managers.UI.CloseAllPopupUI();
    }

    private void OnRankGameClicked(PointerEventData data)
    {
        
    }

    private void OnExhibitionGameClicked(PointerEventData data)
    {
        
    }
    
    protected override void BindObjects()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
    }

    protected override void SetButtonEvents()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitClicked);
        GetButton((int)Buttons.RankGameButton).gameObject.BindEvent(OnRankGameClicked);
        GetButton((int)Buttons.ExhibitionGameButton).gameObject.BindEvent(OnExhibitionGameClicked);
    }

    protected override void SetUI()
    {
        SetObjectSize(GetImage((int)Images.WolfIcon).gameObject, 0.25f);
    }
}
