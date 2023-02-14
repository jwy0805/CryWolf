using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunBlossomController : TowerController
{
    private float _mpTime = 1f;
    private float _lastMpTime = 0f;
    private int _numHeal = 20;
    private int _numHealth = 50;
    private float _numSlow = 0.3f;
    private float _numSlowAttack = 0.3f;
    private bool _heal = false;
    private bool _health = false;
    private bool _slow = false;
    private bool _slowAttack = false;
    
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SunBlossomHeal:
                    _heal = true;
                    break;
                case Define.Skill.SunBlossomHealth:
                    _health = true;
                    break;
                case Define.Skill.SunBlossomSlow:
                    _slow = true;
                    break;
                case Define.Skill.SunBlossomSlowAttack:
                    _slowAttack = true;
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();

        _stat.Hp = 80;
        _stat.MaxHp = 80;
        _stat.Mp = 0;
        _stat.maxMp = 50;
        _stat.Defense = 0;
        _stat.AttackRange = 10;

        SkillInit();
    }

    protected override void UpdateIdle()
    {
        if (Time.time > _lastMpTime + _mpTime)
        {
            _stat.Mp += 4;
            _lastMpTime = Time.time;
        }

        if (_stat.Mp >= _stat.maxMp)
        {
            State = Define.State.Skill;
        }
    }

    private void OnSkillEvent()
    {
        float height = 6f;
        Vector3 pos = transform.position;
        Vector3 pos1 = new Vector3(pos.x, pos.y - height, pos.z);
        Vector3 pos2 = new Vector3(pos.x, pos.y + height, pos.z);
        Collider[] colliders = Physics.OverlapCapsule(pos1, pos2, _stat.AttackRange);
        int length = colliders.Length;
        List<Collider> monsters = new List<Collider>();

        for (int i = 0; i < length; i++)
        {
            if (_health && (colliders[i].CompareTag("Tower") || colliders[i].CompareTag("TowerAir")))
            {
                if (colliders[i].TryGetComponent(out Stat towerStat))
                {
                    StartCoroutine(towerStat.HealthInRounds(_numHealth));
                    if (_heal)
                    {
                        towerStat.Heal(_numHeal);
                    }
                }
            }

            if ((colliders[i].CompareTag("Monster") || colliders[i].CompareTag("MonsterAir")))
            {
                monsters.Add(colliders[i]);
            }   
        }

        int lenMon = monsters.Count;
        int ran = UnityEngine.Random.Range(0, lenMon);
        if (monsters[ran].TryGetComponent(out Stat monsterStat) && _slow)
        {
            StartCoroutine(monsterStat.SlowInRounds(_numSlow));
            if (_slowAttack)
            {
                StartCoroutine(monsterStat.SlowAttackInRounds(_numSlowAttack));
            }
        }
    }

    protected override void OnEndEvent()
    {
        State = Define.State.Idle;
    }
}
