using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;

public class SunfloraPixieController : TowerController
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

    private bool _faint = false;
    private bool _curse = false;
    private bool _triple = false;
    private bool _debuffRemove = false;
    private bool _attack = false;
    private bool _attackSpeed = false;
    private bool _invincible = false;
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SunfloraPixieFaint:
                    _faint = true;
                    break;
                case Define.Skill.SunfloraPixieHeal:
                    _numHeal = 70;
                    break;
                case Define.Skill.SunfloraPixieRange:
                    _stat.AttackRange += 2;
                    break;
                case Define.Skill.SunfloraPixieCurse:
                    _curse = true;
                    break;
                case Define.Skill.SunfloraPixieAttackSpeed:
                    _attackSpeed = true;
                    break;
                case Define.Skill.SunfloraPixieTriple:
                    break;
                case Define.Skill.SunfloraPixieDebuffRemove:
                    break;
                case Define.Skill.SunfloraPixieAttack:
                    _attack = true;
                    break;
                case Define.Skill.SunfloraPixieInvincible:
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();

        _stat.Hp = 320;
        _stat.MaxHp = 320;
        _stat.Mp = 0;
        _stat.maxMp = 40;
        _stat.Defense = 0;
        _stat.AttackRange = 16;

        SkillInit();
    }

    protected override void UpdateIdle()
    {
        if (_attack)
        {
            if (_lockTarget != null)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _stat.AttackRange)
                {
                    State = Define.State.Attack;
                }
            }
        
            if (Time.time > _lastTargetingTime + _targetingTime)
            {
                _lastTargetingTime = Time.time;
                SetTarget(tags);
            }
        }
        else
        {
            MpUp();
        
            if (_stat.Mp >= _stat.maxMp)
            {
                State = Define.State.Skill;
            }
        }
    }

    protected override void UpdateAttack()
    {
        base.UpdateAttack();
        MpUp();
    }

    private void MpUp()
    {
        if (Time.time > _lastMpTime + _mpTime)
        {
            _stat.Mp += 4;
            _lastMpTime = Time.time;
        }
    }
    
    private List<Collider> GetMonsterOrTower(int num, List<Collider> monsters)
    {
        int cnt = monsters.Count;
        List<Collider> randomMonsters = new List<Collider>();

        for (int i = 0; i < num; i++)
        {
            int ran = UnityEngine.Random.Range(0, cnt);
            randomMonsters.Add(monsters[ran]);
            monsters.RemoveAt(ran);
            cnt -= 1;
        }

        return randomMonsters;
    }

    protected override void OnHitEvent()
    {
        
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
        List<Collider> towers = new List<Collider>();
        
        for (int i = 0; i < length; i++)
        {
            if (colliders[i].CompareTag("Tower") || colliders[i].CompareTag("TowerAir"))
            {
                towers.Add(colliders[i]);
                
                if (colliders[i].TryGetComponent(out Stat towerStat))
                {
                    towerStat.Heal(_numHeal);
                    StartCoroutine(towerStat.HealthInRounds(_numHealth));
                    StartCoroutine(towerStat.AttackInRounds(_numAttack));
                    StartCoroutine(towerStat.DefenceInRound(_numDefence));
                    if (_attackSpeed)
                    {
                        
                    }
                }
            }

            if (colliders[i].CompareTag("Monster") || colliders[i].CompareTag("MonsterAir"))
            {
                monsters.Add(colliders[i]);
            }
            
            if (colliders[i].CompareTag("Fence"))
            {
                if (colliders[i].TryGetComponent(out Stat fenceStat))
                {
                    fenceStat.Heal(_numFenceHeal);
                }
            }
        }

        List<Collider> monsterDebuff = new List<Collider>();
        if (_triple)
        {
            monsterDebuff = GetMonsterOrTower(3, monsters);
            if (monsterDebuff == null) return;
            int cnt = monsterDebuff.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (monsterDebuff[i].TryGetComponent(out Stat monsterStat))
                {
                    StartCoroutine(monsterStat.SlowInRounds(_numSlow));
                }
            }
            
            monsterDebuff = GetMonsterOrTower(3, monsters);
            if (monsterDebuff == null) return;
            cnt = monsterDebuff.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (monsterDebuff[i].TryGetComponent(out Stat monsterStat))
                {
                    StartCoroutine(monsterStat.SlowInRounds(_numSlow));
                }
            }

            if (_curse)
            {
                monsterDebuff = GetMonsterOrTower(3, monsters);
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
        else
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

        int debuffCnt = monsterDebuff.Count;
        for (int i = 0; i < debuffCnt; i++)
        {
            
        }
    }

    protected override void OnEndEvent()
    {
        if (_attack)
        {
            if (_lockTarget != null)
            {
                Stat targetStat = _lockTarget.GetComponent<Stat>();

                if (targetStat.Hp > 0)
                {
                    float distance = (_lockTarget.transform.position - transform.position).magnitude;
                    if (distance <= _stat.AttackRange)
                    {
                        State = Define.State.Attack;
                    }
                    else
                    {
                        _lockTarget = null;
                        State = Define.State.Idle;
                    }
                }
                else
                {
                    _lockTarget = null;
                    State = Define.State.Idle;
                }
            }
            else
            {
                State = Define.State.Idle;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}
