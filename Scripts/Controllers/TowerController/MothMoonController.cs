using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothMoonController : TowerController
{
    private bool _healSheep = false;
    private int _heal;
    private bool _output = false;
    private bool _removeDebuff = false;
    private int _removeProb;
    private Bounds _fenceBounds = new Bounds(GameData.center, GameData.FenceSize[1]);

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
                case Define.Skill.MothMoonRemoveDebuffSheep:
                    _removeDebuff = true;
                    _removeProb = 25;
                    break;
                case Define.Skill.MothMoonHealSheep:
                    _healSheep = true;
                    _heal = 15;
                    break;
                case Define.Skill.MothMoonRange:
                    _stat.AttackRange += 3;
                    break;
                case Define.Skill.MothMoonOutput:
                    _output = true;
                    break;
                case Define.Skill.MothMoonAttackSpeed:
                    _stat.AttackSpeed = 0.95f;
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

        _stat.Accuracy = 105;

        tags = new[] { "MonsterAir" };
        
        SkillInit(); 
    }

    protected override void UpdateAttack()
    {
        if (_stat.Mp >= _stat.maxMp)
        {
            if (_healSheep || _removeDebuff)
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

    protected override void OnHitEvent()
    {
        Managers.Resource.Instanciate("Effects/MothMoonAttack", gameObject.transform);
    }

    private void OnSkillEvent()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, GameData.FenceSize[_level] / 2,
                                                    Quaternion.identity, _mask);
        List<Collider> sheeps = new List<Collider>();

        int cLength = colliders.Length;
        for (int i = 0; i < cLength; i++)
        {
            if (colliders[i].CompareTag("Sheep"))
            {
                sheeps.Add(colliders[i]); 
            }    
        }

        int sLength = sheeps.Count;
        for (int i = 0; i < sLength; i++)
        {
            if (!sheeps[i].TryGetComponent(out Stat sheepStat)) continue;
            if (_healSheep) sheepStat.Heal(_heal);
            if (_removeDebuff)
            {
                int ranVal = UnityEngine.Random.Range(0, 100);
                if (ranVal < _removeProb) sheepStat.RemoveAllDebuff(); 
            }
            if (_output)
            {
                
            }
        }

        _stat.Mp = 0;
    }
}
