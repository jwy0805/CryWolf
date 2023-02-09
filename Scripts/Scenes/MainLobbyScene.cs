using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLobbyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Lobby;
        Managers.UI.ShowSceneUI<UI_MainLobbySheep>();
    }

    public override void Clear()
    {
        
    }
}
