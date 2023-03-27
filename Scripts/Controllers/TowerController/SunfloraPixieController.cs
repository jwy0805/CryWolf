using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunfloraPixieController : TowerController
{
    private float _mpTime = 1f;
    private float _lastMpTime = 0f;
    private int _numHeal = 20;
    private int _numFenceHeal = 90;
    private int _numHealth = 50;
    private float _numAttack = 0.1f;
    private float _numAttackSpeed = 0.15f;
    private int _numDefence = 8;
    private float _numSlow = 0.1f;
    private float _numSlowAttack = 0.1f;

    private bool _faint = false;
    private bool _curse = false;
    private bool _triple = false;
    private bool _debuffRemove = false;
    private bool _attack = false;
    private bool _attackSpeedSkill = false;
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
                    _attackSpeedSkill = true;
                    break;
                case Define.Skill.SunfloraPixieTriple:
                    _triple = true;
                    break;
                case Define.Skill.SunfloraPixieDebuffRemove:
                    _debuffRemove = true;
                    break;
                case Define.Skill.SunfloraPixieAttack:
                    _attack = true;
                    break;
                case Define.Skill.SunfloraPixieInvincible:
                    _invincible = true;
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
        _stat.Attack = 50;
        _stat.AttackSpeed = 0.7f;
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
                SetTarget(Tags);
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
        if (_stat.Mp >= _stat.maxMp)
        {
            State = Define.State.Skill;
        }
    }

    private void MpUp()
    {
        if (Time.time > _lastMpTime + _mpTime)
        {
            _stat.Mp += 4;
            _lastMpTime = Time.time;
        }
    }
    
    private List<Collider> PickUnits(int num, List<Collider> monsters)
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

    protected override void OnHitEvent()
    {
        Managers.Resource.Instanciate("Effects/SunfloraPixieAttack", gameObject.transform);
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

        _stat.Mp = 0;
        
        for (int i = 0; i < length; i++)
        {
            if (colliders[i].CompareTag("Tower") || colliders[i].CompareTag("TowerAir"))
            {
                towers.Add(colliders[i]);
                
                if (colliders[i].TryGetComponent(out Stat towerStat))
                {
                    towerStat.Heal(_numHeal);
                    towerStat.ApplyingBuff(10, _numHealth, Define.BuffList.HealthIncrease);
                    towerStat.ApplyingBuff(10, _numAttack, Define.BuffList.AttackIncrease);
                    towerStat.ApplyingBuff(10, _numDefence, Define.BuffList.DefenceIncrease);
                    if (_attackSpeedSkill)
                    {
                        towerStat.ApplyingBuff(10, _numAttackSpeed, Define.BuffList.AttackSpeedIncrease);
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

        if (_debuffRemove)
        {
            List<Collider> tower = PickUnits(1, towers);
            if (tower[0].TryGetComponent(out Stat towerStat))
            {
                towerStat.RemoveAllDebuff();
            }
        }

        if (_invincible)
        {
            List<Collider> tower = PickUnits(1, towers);
            if (tower[0].TryGetComponent(out Stat towerStat))
            {
                towerStat.ApplyingBuff(3, 0, Define.BuffList.Invincible);
            }
        }
        
        // return 있음 -> 코드 순서 바꾸지 말것
        List<Collider> monsterDebuff = PickUnits(_triple ? 3 : 2, monsters);
        if (monsterDebuff == null) return;
        int cnt = monsterDebuff.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (monsterDebuff[i].TryGetComponent(out Stat monsterStat))
            {
                monsterStat.ApplyingBuff(10, _numSlow, Define.BuffList.MoveSpeedDecrease);
                monsterStat.ApplyingBuff(10, _numSlowAttack, Define.BuffList.AttackSpeedDecrease);
                if (_curse)
                {
                    monsterStat.ApplyingBuff(5,0, Define.BuffList.Curse);
                }

                if (_faint)
                {
                    monsterStat.OnFaint();
                }
            }
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
