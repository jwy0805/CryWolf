using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunflowerFairyController : TowerController
{
    private float _mpTime = 1f;
    private float _lastMpTime = 0f;
    private int _numHeal = 20;
    private int _numFenceHeal = 90;
    private int _numHealth = 50;
    private float _numAttack = 0.1f;
    private int _numDefence = 8;
    private float _numSlow = 0.1f;
    private float _numSlowAttack = 0.1f;

    private bool _attack = false;
    private bool _defence = false;
    private bool _double = false;
    private bool _fenceHeal = false;

    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SunflowerFairyAttack:
                    _attack = true;
                    break;
                case Define.Skill.SunflowerFairyDefence:
                    _defence = true;
                    break;
                case Define.Skill.SunflowerFairyDouble:
                    _double = true;
                    break;
                case Define.Skill.SunflowerFairyMpDown:
                    _stat.maxMp = 40;
                    break;
                case Define.Skill.SunflowerFairyFenceHeal:
                    _fenceHeal = true;
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();

        _stat.Hp = 150;
        _stat.MaxHp = 150;
        _stat.Mp = 0;
        _stat.maxMp = 50;
        _stat.Defense = 0;
        _stat.AttackRange = 14;

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

    private List<Collider> GetMonsterOrTower(int num, List<Collider> monsters)
    {
        if (monsters.Count == 0) return null;
        
        int cnt = monsters.Count;
        List<Collider> randomMonsters = new List<Collider>();
        num = (monsters.Count > num) ? num : monsters.Count;
        
        for (int i = 0; i < num; i++)
        {
            int ran = UnityEngine.Random.Range(0, cnt);
            randomMonsters.Add(monsters[ran]);
            monsters.RemoveAt(ran);
            cnt -= 1;
        }

        return randomMonsters;
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
            if (colliders[i].CompareTag("Tower") || colliders[i].CompareTag("TowerAir"))
            {
                if (colliders[i].TryGetComponent(out Stat towerStat))
                {
                    towerStat.Heal(_numHeal);
                    StartCoroutine(towerStat.HealthInRounds(_numHealth));
                    if (_attack)
                    {
                        StartCoroutine(towerStat.AttackInRounds(_numAttack));
                    }

                    if (_defence)
                    {
                        StartCoroutine(towerStat.DefenceInRound(_numDefence));
                    }
                }
            }

            if (colliders[i].CompareTag("Monster") || colliders[i].CompareTag("MonsterAir"))
            {
                monsters.Add(colliders[i]);
            }
            
            if (colliders[i].CompareTag("Fence"))
            {
                if (_fenceHeal && colliders[i].TryGetComponent(out Stat fenceStat))
                {
                    fenceStat.Heal(_numFenceHeal);
                }
            }
        }

        List<Collider> monsterDebuff = new List<Collider>();
        if (_double)
        {
            monsterDebuff = GetMonsterOrTower(2, monsters);
            if (monsterDebuff == null) return;
            int cnt = monsterDebuff.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (monsterDebuff[i].TryGetComponent(out Stat monsterStat))
                {
                    StartCoroutine(monsterStat.SlowInRounds(_numSlow));
                }
            }
            
            monsterDebuff = GetMonsterOrTower(2, monsters);
            if (monsterDebuff == null) return;
            cnt = monsterDebuff.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (monsterDebuff[i].TryGetComponent(out Stat monsterStat))
                {
                    StartCoroutine(monsterStat.SlowInRounds(_numSlow));
                }
            }
        }
        else
        {
            monsterDebuff = GetMonsterOrTower(1, monsters);
            if (monsterDebuff == null) return;
            int cnt = monsterDebuff.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (monsterDebuff[i].TryGetComponent(out Stat monsterStat))
                {
                    StartCoroutine(monsterStat.SlowInRounds(_numSlow));
                }
            }
            
            monsterDebuff = GetMonsterOrTower(1, monsters);
            if (monsterDebuff == null) return;
            cnt = monsterDebuff.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (monsterDebuff[i].TryGetComponent(out Stat monsterStat))
                {
                    StartCoroutine(monsterStat.SlowInRounds(_numSlow));
                }
            }
        }

        
    }

    protected override void OnEndEvent()
    {
        State = Define.State.Idle;
    }
}
