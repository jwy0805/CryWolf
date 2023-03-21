using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothCelestialController : TowerController
{
    private bool _sheepHealth = false;
    private bool _poison = false;
    private bool _breedSheep = false;
    private int _removeProb;
    private int _breedProb;
    private int _heal;
    private int _health;

    private int _mask = 1 << (int)Define.Layer.Sheep;
    private int _level = 1;
    
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
                    _sheepHealth = true;
                    break;
                case Define.Skill.MothCelestialGroundAttack:
                    Tags = new[] { "MonsterAir", "Monster" };
                    break;
                case Define.Skill.MothCelestialAccuracy:
                    _stat.Accuracy += 10;
                    break;
                case Define.Skill.MothCelestialPoisonResist:
                    _stat.PoisonResist = 15;
                    break;
                case Define.Skill.MothCelestialFireResist:
                    _stat.FireResist = 15;
                    break;
                case Define.Skill.MothCelestialPoison:
                    _poison = true;
                    break;
                case Define.Skill.MothCelestialBreedSheep:
                    _breedSheep = true;
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
        _skillSubject = GameObject.Find("Subject").GetComponent<SkillSubject>();
        _skillSubject.AddObserver(this);
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        WorldObjectType = Define.WorldObject.Tower;
        Vector3 size = gameObject.GetComponent<Collider>().bounds.size;
        _stat = gameObject.GetComponent<Stat>();

        transform.position = GameData.Center + Vector3.up * 2;
        State = Define.State.Idle;

        _stat.Hp = 180;
        _stat.MaxHp = 180;
        _stat.Mp = 0;
        _stat.maxMp = 50;
        _stat.Attack = 30;
        _stat.Defense = 3;
        _stat.AttackRange = 15;
        _stat.AttackSpeed = 0.8f;
        _stat.Accuracy = 105;

        _removeProb = 25;
        _breedProb = 3;
        _heal = 15;
        Tags = new[] { "MonsterAir" };

        SkillInit();
    }

    protected override void UpdateAttack()
    {
        if (_stat.Mp >= _stat.maxMp)
        {
            if (_sheepHealth || _breedSheep)
            {
                State = Define.State.Skill;
            }
            else
            {
                _stat.Mp = 0;
            }
        }
        
        base.UpdateAttack();
    }

    protected override void SetTarget(string[] tags)
    {
        float closestDist = 5000.0f;
        int length = tags.Length;
        for (int i = 0; i < length; i++)
        {
            _tagged = GameObject.FindGameObjectsWithTag(tags[i]);
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

            // 공중 우선 공격 
            if (_lockTarget != null && closestDist <= _stat.AttackRange) return;
        }
    }

    protected override void OnHitEvent()
    {
        if (_poison)
        {
            Managers.Resource.Instanciate("Effects/MothCelestialPoison", gameObject.transform);
        }
        else
        {
            Managers.Resource.Instanciate("Effects/MothMoonAttack", gameObject.transform);
        }
    }

    private void OnSkillEvent()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, GameData.FenceSize[_level] / 2,
            Quaternion.identity, _mask);
        List<Collider> sheeps = new List<Collider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Sheep"))
            {
                sheeps.Add(colliders[i]); 
            }    
        }

        int sLength = sheeps.Count;
        int ranHealth = UnityEngine.Random.Range(0, sLength);
        for (int i = 0; i < sLength; i++)
        {
            int randBreed = UnityEngine.Random.Range(0, 100);
            if (randBreed < _breedProb)
            {
                Managers.Game.Spawn(Define.WorldObject.Sheep, "Sheep");
            }

            // MothMoon output 스킬 계승
            // 이 위에 스킬 만들것(sheepStat을 이용한 스킬만 아래 구현)
            if (!sheeps[i].TryGetComponent(out Stat sheepStat)) continue;
            sheepStat.Heal(_heal);
            int ranVal = UnityEngine.Random.Range(0, 100);
            if (ranVal < _removeProb) sheepStat.RemoveAllDebuff();
            if (i == ranHealth)
            {
                sheepStat.Hp += 100;
                sheepStat.MaxHp += 100;
            }
        }
    }
}
