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
        // switch (Util.SheepOrWolf)
        // {
        //     case "Sheep":
        //         Managers.UI.ShowSceneUI<UI_GameSheep>();
        //         break;
        //     case "Wolf":
        //         Managers.UI.ShowSceneUI<UI_GameWolf>();
        //         break;
        //     default:
        //         return;
        // }
        Managers.UI.ShowSceneUI<UI_Game>();
        
        InitObjects(Util.SheepOrWolf);
    }
    
    public override void Clear()
    {
        
    }

    private void InitObjects(string side)
    {
        GameObject player;
        
        switch (side)
        {
            case "Sheep":
                player = Managers.Game.Spawn(Define.WorldObject.PlayerSheep, "PlayerCharacter");
                break;
            case "Wolf":
                player = Managers.Game.Spawn(Define.WorldObject.PlayerWolf, "PoisonBomb");
                break;
            default:
                player = Managers.Game.Spawn(Define.WorldObject.PlayerSheep, "PlayerCharacter");
                break;
        }
        
        GameObject virtualCamera = GameObject.Find("FollowCam");
        CinemachineVirtualCamera followCam = virtualCamera.GetComponent<CinemachineVirtualCamera>();
        followCam.Follow = player.transform;
        followCam.LookAt = player.transform;
        
        GameObject spawner = new GameObject { name = "Spawner" };
        spawner.GetOrAddComponent<Spawner>();
        spawner.GetOrAddComponent<DeliverGameObject>();

        GameObject Subject = new GameObject { name = "Subject" };
        Subject.GetOrAddComponent<SkillSubject>();
    }
}
