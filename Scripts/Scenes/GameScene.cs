using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        
        SceneType = Define.Scene.Game;
        
        Util.SheepOrWolf = "Sheep";
        if (Util.SheepOrWolf == "Sheep")
        {
            Managers.UI.ShowSceneUI<UI_GameSheep>();
        }
        else
        {
            Managers.UI.ShowSceneUI<UI_GameWolf>();
        }

        Util.SheepOrWolf = null;

        InitObjects();
    }
    
    public override void Clear()
    {
        
    }

    private void InitObjects()
    {
        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "PlayerCharacter");
        
        GameObject virtualCamera = GameObject.Find("FollowCam");
        CinemachineVirtualCamera followCam = virtualCamera.GetComponent<CinemachineVirtualCamera>();
        followCam.Follow = player.transform;
        followCam.LookAt = player.transform;
        
        GameObject spawner = new GameObject { name = "Spawner" };
        spawner.GetOrAddComponent<Spawner>();
        spawner.GetOrAddComponent<DeliverGameObject>();

        GameObject skillSubject = new GameObject { name = "SkillSubject" };
        skillSubject.GetOrAddComponent<SkillSubject>();
    }
}
