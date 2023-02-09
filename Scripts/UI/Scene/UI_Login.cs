using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UI_Login : UI_Scene
{
    enum Buttons
    {
        LoginButton,
        SignUpButton,
        ForgotPasswordButton,
        AppleButton,
        GoogleButton,
        FacebookButton,
    }

    enum Texts
    {
        
    }

    enum Images
    {
        Background,
        AppleImage,
        GoogleImage,
        FacebookImage,
    }

    protected override void Init()
    {
        base.Init();
        
        BindObjects();
        SetButtonEvents();
        SetUI();
    }

    private void OnLoginClicked(PointerEventData data)
    {
        SceneManager.LoadScene("Scenes/MainLobby");
        Managers.Clear();
    }

    protected override void SetBackgroundSize(RectTransform rectTransform)
    {
        Rect rect = rectTransform.rect;
        float canvasWidth = rect.width;
        float canvasHeight = rect.height;
        float backgroundHeight = canvasWidth * 1.2f;
        float nightSkyHeight = canvasHeight - backgroundHeight;
        
        RectTransform rtBackground = GameObject.Find("Background").GetComponent<RectTransform>();
        rtBackground.sizeDelta = new Vector2(canvasWidth, backgroundHeight);

        RectTransform rtNightSky = GameObject.Find("NightSky").GetComponent<RectTransform>();
        rtNightSky.sizeDelta = new Vector2(canvasWidth, nightSkyHeight);
    }

    protected override void BindObjects()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }

    protected override void SetButtonEvents()
    {
        GetButton((int)Buttons.LoginButton).gameObject.BindEvent(OnLoginClicked);
    }
    
    protected override void SetUI()
    {
        SetBackgroundSize(gameObject.GetComponent<RectTransform>());
        
        SetObjectSize(GetImage((int)Images.AppleImage).gameObject, 1.0f);
        SetObjectSize(GetImage((int)Images.GoogleImage).gameObject, 1.0f);
        SetObjectSize(GetImage((int)Images.FacebookImage).gameObject, 1.0f);
    }
}
