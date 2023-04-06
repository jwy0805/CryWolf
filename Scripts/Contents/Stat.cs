using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stat : MonoBehaviour
{
    private int _level;
    private int _resource;
    private int _hp;
    private int _maxHp;
    private int _mp;
    private int _maxMp;
    private int _attack;
    private int _defense;
    private int _fireResist;
    private int _poisonResist;
    private int _skill;
    private float _moveSpeed;
    private float _attackSpeed;
    private float _attackRange;
    private float _criticalChance;
    private float _criticalMultiplier;
    private float _accuracy;
    private float _evasion;
    private bool _targetable;
    private bool _aggro = false;

    private bool _reflection;
    private bool _reflectionSkill;
    private float _reflectionRate;

    private float _buffTime = 10f;
    private float _buffParam = 0f;
    private GameObject _caller;
    private Define.BuffList _applyBuff;

    private static readonly int AnimAttackSpeed = Animator.StringToHash("AttackSpeed");

    public int Level { get { return _level; } set { _level = value; } }
    public int Resource { get { return _resource; } set { _resource = value; } }
    public int Hp
    {
        get => _hp;
        set
        {
            _hp = value;
            if (_hp <= 0)
            {
                OnDead();
            }
        }
    }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Mp { get { return _mp; } set { _mp = value; } }
    public int maxMp { get { return _maxMp; } set { _maxMp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public int FireResist { get { return _fireResist; } set { _fireResist = value; } }
    public int PoisonResist { get { return _poisonResist; } set { _poisonResist = value; } }
    public int Skill { get { return _skill; } set { _skill = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float AttackSpeed 
    {
        get => _attackSpeed;
        set
        {
            _attackSpeed = value;
            Animator anim = gameObject.GetComponent<Animator>();
            anim.SetFloat(AnimAttackSpeed, _attackSpeed);
        } 
    }
    public float AttackRange { get { return _attackRange; } set { _attackRange = value; } }
    public float CriticalChance { get { return _criticalChance; } set { _criticalChance = value; } }
    public float CriticalMultiplier { get { return _criticalMultiplier; } set { _criticalMultiplier = value; } }
    public float Accuracy { get { return _accuracy; } set { _accuracy = value; } }
    public float Evasion { get { return _evasion; } set { _evasion = value; } }
    public bool Targetable { get { return _targetable; } set { _targetable = value; } }
    public bool Reflection { get { return _reflection; } set { _reflection = value; } }
    public bool ReflectionSkill { get { return _reflectionSkill; } set { _reflectionSkill = value; } }
    public float ReflectionRate { get { return _reflectionRate; } set { _reflectionRate = value; } }
    public bool Aggro { get { return _aggro;} set { _aggro = value; } }

    void Start()
    {
        Targetable = true;
        Accuracy = 100;
        Evasion = 0;
        FireResist = 0;
        PoisonResist = 0;
        CriticalChance = 0;
        CriticalMultiplier = 1f;
        Reflection = false;
        ReflectionSkill = false;
        ReflectionRate = 0f;
    }
    
    public virtual void OnAttakced(Stat attacker)
    {
        if (_buffDict.ContainsKey(Define.Buff.Invincible)) return;
        int damage;
        var random = new System.Random();
        int randVal = random.Next(100);
        
        if (randVal >= (int)attacker.Accuracy - (int)Evasion) return;
        if (randVal < (int)attacker.CriticalChance)
            damage = (int)(Mathf.Max(0, attacker.Attack - Defense) * CriticalMultiplier); 
        else damage = Mathf.Max(0, attacker.Attack - Defense);

        Mp += 2;
        Hp -= damage;
        
        if (Reflection) OnReflection(attacker, damage);
    }

    public virtual void OnSkilled(Stat attacker)
    {
        if (_buffDict.ContainsKey(Define.Buff.Invincible)) return;
        int damage;
        var random = new System.Random();
        int randVal = random.Next(100);
        
        if (randVal < (int)attacker.CriticalChance)
            damage = (int)(Mathf.Max(0, attacker.Skill - Defense) * CriticalMultiplier); 
        else damage = Mathf.Max(0, attacker.Skill - Defense);

        Mp += 2;
        Hp -= damage;
        
        if (ReflectionSkill) OnReflection(attacker, damage);
    }

    public virtual void OnDead()
    {
        Targetable = false;
        BaseController baseController = gameObject.GetComponent<BaseController>();
        Define.WorldObject type = baseController.WorldObjectType;
        RemoveAllDebuff();
        switch (type)
        {
            case Define.WorldObject.Fence:
                Managers.Game.Despawn(gameObject);
                GameData.CurrentFenceCnt -= 1;
                break;
            default:
                baseController.State = Define.State.Die;
                break;
        }
    }

    public virtual void OnFaint()
    {
        BaseController baseController = gameObject.GetComponent<BaseController>();
        baseController.State = Define.State.Faint;
    }
    
    public virtual void OnReflection(Stat attacker, int damage)
    {
        attacker.Hp -= (int)Mathf.Max(0, damage * ReflectionRate);
    }
    
    public virtual void Heal(float heal)
    {
        Hp += (int)heal;
        if (Hp > MaxHp) Hp = MaxHp;
    }

    #region BuffSystem

    private Dictionary<Define.Buff, IEnumerator> _buffDict = new Dictionary<Define.Buff, IEnumerator>();
    private Dictionary<Define.Debuff, IEnumerator> _debuffDict = new Dictionary<Define.Debuff, IEnumerator>();
    private Dictionary<int, Define.Debuff> _debuffHashDict = new Dictionary<int, Define.Debuff>();
    private Dictionary<int, IEnumerator> _debuffNestedDict = new Dictionary<int, IEnumerator>(); 
    private Dictionary<Define.Buff, float> _buffParamDict = new Dictionary<Define.Buff, float>();
    private Dictionary<Define.Debuff, float> _debuffParamDict = new Dictionary<Define.Debuff, float>();
    public Dictionary<Define.Debuff, IEnumerator> DebuffDict => _debuffDict;
    private Define.BuffList ApplyBuff
    {
        get => _applyBuff;
        set
        {
            _applyBuff = value;
            string buffstr = Enum.GetName(typeof(Define.BuffList), _applyBuff);
            string[] buffArr = Enum.GetNames(typeof(Define.Buff));
            
            if (buffArr.Contains(buffstr))
            {
                if (buffstr == null) return;
                Define.Buff buff = (Define.Buff)Enum.Parse(typeof(Define.Buff), buffstr);
                if (_buffDict.ContainsKey(buff)) RenewBuff(buff);
                StartBuff(buff);
            }
            else
            {
                if (buffstr == null) return;
                Define.Debuff debuff = (Define.Debuff)Enum.Parse(typeof(Define.Debuff), buffstr);
                if (_debuffDict.ContainsKey(debuff)) RenewDebuff(debuff);
                StartDebuff(debuff);
            }
        }
    }

    public void ApplyingBuff(float time, float param, Define.BuffList buff, GameObject caller = null)
    {
        switch (gameObject.name)
        {
            case "Werewolf":
                bool debuffResist = gameObject.GetComponent<WerewolfController>().DebuffResist;
                if (debuffResist)
                {
                    _buffTime = time * 0.5f;
                    _buffParam = param * 0.5f;
                }
                break;
            default:
                _buffTime = time;
                _buffParam = param;
                break;
        }
        if (caller != null) _caller = caller;
        ApplyBuff = buff;
    }
    
    private void StartBuff(Define.Buff buff)
    {
        IEnumerator coroutine = SelectBuff(buff);
        _buffDict.Add(buff, coroutine);
        StartCoroutine(coroutine);
    }

    private void StartDebuff(Define.Debuff debuff)
    {
        IEnumerator coroutine = SelectDebuff(debuff);
        switch (debuff)
        {
            case Define.Debuff.Curse:
            case Define.Debuff.DeadlyAddicted:
                _debuffHashDict.Add(coroutine.GetHashCode(), debuff);
                _debuffNestedDict.Add(coroutine.GetHashCode(), coroutine);
                break;
        }
        _debuffDict.Add(debuff, coroutine);
        StartCoroutine(coroutine);
    }
    
    private IEnumerator SelectBuff(Define.Buff buff)
    {
        IEnumerator coroutine;
        switch (buff)
        {
            case Define.Buff.AttackIncrease:
                coroutine = AttackBuff();
                break;
            case Define.Buff.AttackSpeedIncrease:
                coroutine = AttackSpeedBuff();
                break;
            case Define.Buff.MoveSpeedIncrease:
                coroutine = MoveSpeedBuff();
                break;
            case Define.Buff.DefenceIncrease:
                coroutine = DefenceBuff();
                break;
            case Define.Buff.HealthIncrease:
                coroutine = HealthBuff();
                break;
            case Define.Buff.Invincible:
                coroutine = Invincible();
                break;
            default:
                coroutine = null;
                break;
        }

        return coroutine;
    }
    
    private IEnumerator SelectDebuff(Define.Debuff debuff)
    {
        IEnumerator coroutine;
        switch (debuff)
        {
            case Define.Debuff.AttackDecrease:
                coroutine = AttackDebuff();
                break;
            case Define.Debuff.AttackSpeedDecrease:
                coroutine = AttackSpeedDebuff();
                break;
            case Define.Debuff.MoveSpeedDecrease:
                coroutine = MoveSpeedDebuff();
                break;
            case Define.Debuff.DefenceDecrease:
                coroutine = DefenceDebuff();
                break;
            case Define.Debuff.Curse:
                coroutine = Curse();
                break;
            case Define.Debuff.Addicted:
                coroutine = Addicted();
                break;
            case Define.Debuff.DeadlyAddicted:
                coroutine = DeadlyAddicted();
                break;
            case Define.Debuff.Aggro:
                coroutine = AggroFunc();
                break;
            default:
                coroutine = null;
                break;
        }
        
        return coroutine;
    }
    
    private void RenewBuff(Define.Buff buff)
    {
        StopCoroutine(_buffDict[buff]);
        switch (buff)
        {
            case Define.Buff.AttackIncrease:
                Attack -= (int)_buffParamDict[buff];
                break;
            case Define.Buff.AttackSpeedIncrease:
                AttackSpeed -= _buffParamDict[buff];
                break;
            case Define.Buff.MoveSpeedIncrease:
                MoveSpeed -= _buffParamDict[buff];
                break;
            case Define.Buff.DefenceIncrease:
                Defense -= (int)_buffParamDict[buff];
                break;
            case Define.Buff.HealthIncrease:
                MaxHp -= (int)_buffParamDict[buff];
                if (Hp > MaxHp) Hp = MaxHp;
                break;
            case Define.Buff.Invincible:
                break;
        }
        _buffDict.Remove(buff);
        _buffParamDict.Remove(buff);
    }
    
    private void RenewDebuff(Define.Debuff debuff)
    {
        switch (debuff)
        {
            case Define.Debuff.AttackDecrease:
                StopCoroutine(_debuffDict[debuff]);
                Attack += (int)_debuffParamDict[debuff];
                break;
            case Define.Debuff.AttackSpeedDecrease:
                StopCoroutine(_debuffDict[debuff]);
                AttackSpeed += _debuffParamDict[debuff];
                break;
            case Define.Debuff.MoveSpeedDecrease:
                StopCoroutine(_debuffDict[debuff]);
                MoveSpeed += _debuffParamDict[debuff];
                break;
            case Define.Debuff.DefenceDecrease:
                StopCoroutine(_debuffDict[debuff]);
                Defense += (int)_debuffParamDict[debuff];
                break;
            case Define.Debuff.Addicted:
                StopCoroutine(_debuffDict[debuff]);
                break;
            case Define.Debuff.Aggro:
                StopCoroutine(_debuffDict[debuff]);
                Aggro = false;
                break;
        }
        _debuffDict.Remove(debuff);
        _debuffParamDict.Remove(debuff);
    }
    
    public void RemoveDebuff(Define.Debuff debuff)
    {
        StopCoroutine(_debuffDict[debuff]);
        switch (debuff)
        {
            case Define.Debuff.AttackDecrease:
                Attack += (int)_debuffParamDict[debuff];
                break;
            case Define.Debuff.AttackSpeedDecrease:
                AttackSpeed += _debuffParamDict[debuff];
                break;
            case Define.Debuff.MoveSpeedDecrease:
                MoveSpeed += _debuffParamDict[debuff];
                break;
            case Define.Debuff.DefenceDecrease:
                Defense += (int)_debuffParamDict[debuff];
                break;
            case Define.Debuff.Aggro:
                Aggro = false;
                break;
            case Define.Debuff.Curse:
            case Define.Debuff.DeadlyAddicted:
                if (_debuffHashDict != null)
                {
                    Dictionary<int, Define.Debuff> toRemove = new Dictionary<int, Define.Debuff>();
                    foreach (var hash in _debuffHashDict.Keys)
                    {
                        if (_debuffHashDict[hash] == debuff)
                        {
                            StopCoroutine(_debuffNestedDict[hash]);
                            toRemove.Add(hash, _debuffHashDict[hash]);
                        }
                    }
                    foreach (var hash in toRemove.Keys)
                    {
                        _debuffHashDict.Remove(hash);
                        _debuffNestedDict.Remove(hash);
                    }
                }
                break;
        }
        _debuffDict.Remove(debuff);
        _debuffParamDict.Remove(debuff);
    }
    
    public void RemoveAllDebuff()
    {
        if (_debuffDict != null)
        {
            List<Define.Debuff> toRemove = new List<Define.Debuff>();
            foreach (var debuff in _debuffDict.Keys)
            {
                toRemove.Add(debuff);
            }
            foreach (var debuff in toRemove)
            {
                RemoveDebuff(debuff);
            }
        }

        if (_debuffHashDict != null)
        {
            List<int> toRemove = new List<int>();
            foreach (var hash in _debuffHashDict.Keys)
            {
                toRemove.Add(hash);
            }
            foreach (var hash in toRemove)
            {
                RemoveDebuff(_debuffHashDict[hash]);
            }
        }
    }
    
    private IEnumerator AttackBuff()
    {
        float time = Time.time;
        Define.Buff buff = Define.Buff.AttackIncrease;
        float p = Attack * _buffParam;
        _buffParamDict.Add(buff, p);
        Attack += (int)p;
        yield return new WaitForSeconds(_buffTime);
        Attack -= (int)p;
        _buffDict.Remove(buff);
        _buffParamDict.Remove(buff);
    }

    private IEnumerator AttackSpeedBuff()
    {
        Define.Buff buff = Define.Buff.AttackSpeedIncrease;
        float p = AttackSpeed * _buffParam;
        _buffParamDict.Add(buff, p);
        AttackSpeed += p;
        yield return new WaitForSeconds(_buffTime);
        AttackSpeed -= p;
        _buffDict.Remove(buff);
        _buffParamDict.Remove(buff);
    }

    private IEnumerator HealthBuff()
    {
        Define.Buff buff = Define.Buff.HealthIncrease;
        float p = MaxHp * _buffParam;
        _buffParamDict.Add(buff, p);
        Hp += (int)p;
        MaxHp += (int)p;
        yield return new WaitForSeconds(_buffTime);
        MaxHp -= (int)p;
        if (Hp > MaxHp) Hp = MaxHp;
        _buffDict.Remove(buff);
        _buffParamDict.Remove(buff);
    }

    private IEnumerator DefenceBuff()
    {
        Define.Buff buff = Define.Buff.DefenceIncrease;
        float p = _buffParam;
        _buffParamDict.Add(buff, p);
        Defense += (int)p;
        yield return new WaitForSeconds(_buffTime);
        Defense -= (int)p;
        _buffDict.Remove(buff);
        _buffParamDict.Remove(buff);
    }

    private IEnumerator MoveSpeedBuff()
    {
        Define.Buff buff = Define.Buff.MoveSpeedIncrease;
        float p = MoveSpeed * _buffParam;
        _buffParamDict.Add(buff, p);
        MoveSpeed += p;
        yield return new WaitForSeconds(_buffTime);
        MoveSpeed -= p;
        _buffDict.Remove(buff);
        _buffParamDict.Remove(buff);
    }

    private IEnumerator Invincible()
    {
        GameObject effect = Managers.Resource.Instanciate("Effects/HolyAura", gameObject.transform);
        Define.Buff buff = Define.Buff.Invincible;
        yield return new WaitForSeconds(_buffTime);
        Managers.Resource.Destroy(effect);
        _buffDict.Remove(buff);
    }

    private IEnumerator AttackDebuff()
    {
        Define.Debuff debuff = Define.Debuff.AttackDecrease;
        float p = Attack * _buffParam;
        _debuffParamDict.Add(debuff, p);
        Attack -= (int)p;
        yield return new WaitForSeconds(_buffTime);
        Attack += (int)p;
        _debuffDict.Remove(debuff);
        _debuffParamDict.Remove(debuff);
    }

    private IEnumerator AttackSpeedDebuff()
    {
        Define.Debuff debuff = Define.Debuff.AttackSpeedDecrease;
        float p = AttackSpeed * _buffParam;
        _debuffParamDict.Add(debuff, p);
        AttackSpeed -= p;
        yield return new WaitForSeconds(_buffTime);
        AttackSpeed += p;
        _debuffDict.Remove(debuff);
        _debuffParamDict.Remove(debuff);
    }

    private IEnumerator DefenceDebuff()
    {
        Define.Debuff debuff = Define.Debuff.DefenceDecrease;
        float p = _buffParam;
        _debuffParamDict.Add(debuff, p);
        Defense -= (int)p;
        yield return new WaitForSeconds(_buffTime);
        Defense += (int)p;
        _debuffDict.Remove(debuff);
        _debuffParamDict.Remove(debuff);
    }

    private IEnumerator MoveSpeedDebuff()
    {
        Define.Debuff debuff = Define.Debuff.MoveSpeedDecrease;
        float p = MoveSpeed * _buffParam;
        _debuffParamDict.Add(debuff, p);
        MoveSpeed -= p;
        yield return new WaitForSeconds(5f);
        MoveSpeed += p;
        _debuffDict.Remove(debuff);
        _debuffParamDict.Remove(debuff);
    }

    private IEnumerator Curse()
    {
        yield return new WaitForSeconds(_buffTime);
        Hp /= 2;
    }

    private IEnumerator Addicted()
    {
        Define.Debuff debuff = Define.Debuff.Addicted;
        float intervalTime = 1.0f;
        float startTime = Time.time;
        int poison = (int)(MaxHp * _buffParam);
        while (startTime + _buffTime >= Time.time)
        {
            Hp -= poison;
            yield return new WaitForSeconds(intervalTime);
        }
        _debuffDict.Remove(debuff);
    }

    private IEnumerator DeadlyAddicted()
    {
        float intervalTime = 1.0f;
        float startTime = Time.time;
        int poison = (int)(MaxHp * _buffParam);
        while (startTime + _buffTime >= Time.time)
        {
            Hp -= poison;
            yield return new WaitForSeconds(intervalTime);
        }
    }

    private IEnumerator AggroFunc()
    {
        Define.Debuff debuff = Define.Debuff.Aggro;
        float intervalTime = 0.5f;
        float startTime = Time.time;
        BaseController controller = GetComponent<BaseController>();
        Aggro = true;
        while (startTime + _buffTime >= Time.time)
        {
            if (_caller != null) controller._lockTarget = _caller;
            yield return new WaitForSeconds(intervalTime);
        }
        Aggro = false;
        _debuffDict.Remove(debuff);
    }
    
    #endregion
}
