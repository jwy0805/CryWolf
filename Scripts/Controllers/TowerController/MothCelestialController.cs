using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothCelestialController : TowerController
{
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.MothCelestialSheepHealth:
                    break;
                case Define.Skill.MothCelestialSheepDefence:
                    break;
                case Define.Skill.MothCelestialAccuracy:
                    break;
                case Define.Skill.MothCelestialPoisonResist:
                    break;
                case Define.Skill.MothCelestialFireResist:
                    break;
                case Define.Skill.MothCelestialPoison:
                    break;
                case Define.Skill.MothCelestialBreedSheep:
                    break;
            }
        }
    }
    
    public override bool Active
    {
        get => _active;
        set => _active = value;
    }

    protected override void Init()
    {
        _skillSubject = GameObject.Find("SkillSubject").GetComponent<SkillSubject>();
        _skillSubject.AddObserver(this);
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        WorldObjectType = Define.WorldObject.Tower;
        Vector3 size = gameObject.GetComponent<Collider>().bounds.size;
        _stat = gameObject.GetComponent<Stat>();

        transform.position = GameData.center + Vector3.up * 2;
        State = Define.State.Idle;

        _stat.Hp = 180;
        _stat.MaxHp = 180;
        _stat.Mp = 0;
        _stat.maxMp = 30;
        _stat.Attack = 30;
        _stat.Defense = 3;
        _stat.AttackRange = 7;
        _stat.AttackSpeed = 0.8f;
        _anim.SetFloat(_attackSpeed, _stat.AttackSpeed);

        _stat.Accuracy = 105;

        tags = new[] { "MonsterAir" };

        SkillInit();
    }
    
    protected override void SetTarget(string[] tags)
    {
        float closestDist = 5000.0f;
        foreach (var tag in tags)
        {
            _tagged = GameObject.FindGameObjectsWithTag(tag);
            foreach (var tagged in _tagged)
            {
                Vector3 targetPos = tagged.transform.position;
                Stat stat = tagged.gameObject.GetComponent<Stat>();
                stat.enabled = true;
                bool targetable = stat.Targetable;
                float dist = (targetPos - transform.position).sqrMagnitude;
                if (dist < closestDist && targetable)
                {
                    closestDist = dist;
                    _lockTarget = tagged;
                }
            }   
        }
    }
}
