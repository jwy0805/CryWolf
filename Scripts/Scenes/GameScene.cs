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

        switch (Util.SheepOrWolf)
        {
            case "Sheep":
                Managers.UI.ShowSceneUI<UI_GameSheep>();
                break;
            case "Wolf":
                Managers.UI.ShowSceneUI<UI_GameWolf>();
                break;
            default:
                return;
        }
        
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
                player = Managers.Game.Spawn(Define.WorldObject.Player, "PlayerCharacter");
                break;
            case "Wolf":
                player = Managers.Game.Spawn(Define.WorldObject.Player, "PoisonBomb");
                break;
            default:
                player = Managers.Game.Spawn(Define.WorldObject.Player, "PlayerCharacter");
                break;
        }
        
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
