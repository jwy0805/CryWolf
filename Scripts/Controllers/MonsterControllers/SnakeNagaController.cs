using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNagaController : MonsterController
{
    private bool _drain = false;
    private bool _meteor = true;
    
    public bool Drain => _drain;

    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SnakeNagaAttack:
                    _stat.AttackSpeed += 0.15f;
                    break;
                case Define.Skill.SnakeNagaRange:
                    _stat.Attack += 10;
                    break;
                case Define.Skill.SnakeNagaCritical:
                    _stat.AttackRange += 2;
                    break;
                case Define.Skill.SnakeNagaFireResist:
                    _stat.Accuracy += 10;
                    break;
                case Define.Skill.SnakeNagaDrain:
                    _drain = true;
                    break;
                case Define.Skill.SnakeNagaMeteor:
                    _meteor = true;
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 120;
        _stat.MaxHp = 120;
        _stat.Mp = 0;
        _stat.maxMp = 20;
        _stat.Attack = 100;
        _stat.Defense = 0;
        _stat.Skill = 250;
        _stat.MoveSpeed = 5.0f;
        _stat.AttackRange = 11.0f;
    }

    protected override void UpdateAttack()
    {
        if (_stat.Mp >= 20)
        {
            _stat.Mp = 0;
            if (_meteor) State = Define.State.Skill;
        }
        else
        {
            base.UpdateAttack();
        }
    }

    private Vector3 MeteorPos()
    {
        // 메테오 위치 선정
        int towerMask = 1 << (int)Define.Layer.Tower;
        int sheepMask = 1 << (int)Define.Layer.Sheep;
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        Vector3 meteorPos = GameData.Center;
        int cnt = 200;
        for (int i = 0; i < towers.Length; i++)
        {
            GameObject tower = towers[i];
            var pos = tower.transform.position;
            Collider[] towerColliders = Physics.OverlapSphere(pos, 1.5f, towerMask);
            Collider[] sheepColliders = Physics.OverlapSphere(pos, 1.5f, sheepMask);
            int towerCnt = towerColliders.Length;
            int sheepCnt = sheepColliders.Length;
            int objectCnt = towerCnt + sheepCnt;
            if (objectCnt < cnt)
            {
                cnt = objectCnt;
                meteorPos = tower.transform.position;
            }
        }

        return meteorPos;
    }
    
    protected override void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Managers.Resource.Instanciate("Effects/BigFire", gameObject.transform);
        }
    }

    private void OnSkillEvent()
    {
        GameObject meteor = Managers.Resource.Instanciate("Effects/Meteor", gameObject.transform);
        meteor.transform.position = MeteorPos();
    }
}
