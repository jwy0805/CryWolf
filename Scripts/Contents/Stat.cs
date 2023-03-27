using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    private List<IEnumerator> _buffList = new List<IEnumerator>();
    private List<IEnumerator> _debuffList = new List<IEnumerator>();
    private List<int> _indexList = new List<int>();
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
                if (_buffList.Contains(_buffDict[buff]))
                {
                    int index = _buffList.IndexOf(_buffDict[buff]);
                    StopCoroutine(_buffList[index]);
                    _buffList.Remove(_buffDict[buff]);
                }
                _buffList.Add(_buffDict[buff]);
                StartCoroutine(_buffDict[buff]);
            }
            else
            {
                if (buffstr == null) return;
                Define.Debuff debuff = (Define.Debuff)Enum.Parse(typeof(Define.Debuff), buffstr);

                if (_debuffList.Contains(_debuffDict[debuff]))
                {
                    int index = _debuffList.IndexOf(_debuffDict[debuff]);
                    StopCoroutine(_debuffList[index]);
                    StopCoroutineEffect(_debuffList[index]);
                    _debuffList.Remove(_debuffDict[debuff]);
                }
                _debuffList.Add(_debuffDict[debuff]);
                StartCoroutine(_debuffList.Last());
            }
        }
    }

    private void StopCoroutineEffect(IEnumerator coroutine)
    {
        
    }
    
    public void ApplyingBuff(float time, float param, Define.BuffList buff)
    {
        _buffTime = time;
        _buffParam = param;
        ApplyBuff = buff;
    }
    
    private Dictionary<Define.Buff, IEnumerator> _buffDict = new Dictionary<Define.Buff, IEnumerator>();    
    private Dictionary<Define.Debuff, IEnumerator> _debuffDict = new Dictionary<Define.Debuff, IEnumerator>();

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
        
        RegisterBuff();
    }

    public virtual void OnAttakced(Stat attacker)
    {
        if (_buffList.Contains(_buffDict[Define.Buff.Invincible])) return;
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
        if (_buffList.Contains(_buffDict[Define.Buff.Invincible])) return;
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

    private void RegisterBuff()
    {
        // Define.Buff 와 순서 맞출것
        IEnumerator[] buffArr =
        {
            AttackBuff(), AttackSpeedBuff(), HealthBuff(), DefenceBuff(), MoveSpeedBuff(), Invincible()
        };
        IEnumerator[] debuffArr =
        {
            AttackDebuff(), AttackSpeedDebuff(), DefenceDebuff(), MoveSpeedDebuff(), Curse(), Addicted(), 
            DeadlyAddicted(), AggroFunc(),
        };
        Array buffEnum = Enum.GetValues(typeof(Define.Buff));
        Array debuffEnum = Enum.GetValues(typeof(Define.Debuff));

        for (int i = 0; i < buffArr.Length; i++)
        {
            _buffDict.Add((Define.Buff)buffEnum.GetValue(i), buffArr[i]);
        }

        for (int i = 0; i < debuffArr.Length; i++)
        {
            _debuffDict.Add((Define.Debuff)debuffEnum.GetValue(i), debuffArr[i]);
        }
    }

    private void SetParams(float time, float param)
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
    }
    
    public void RemoveAllDebuff()
    {
        // for (int i = 0; i < _debuffList.Count; i++)
        // {
        //     Define.Debuff debuff = _debuffList[i];
        //     StopCoroutine(_debuffDict[debuff]);
        // }
        //
        // _debuffList.Clear();
    }
    
    public void RemoveDebuff(Define.Debuff debuff)
    {
        // if (_debuffList.Contains(debuff))
        // {
        //     StopCoroutine(_debuffDict[debuff]);
        //     _debuffList.Remove(debuff);   
        // }
    }

    private IEnumerator AttackBuff()
    {
        float time = Time.time;
        Debug.Log($"start {time}");
        int p = (int)(Attack * _buffParam);
        Attack += p;
        yield return new WaitForSeconds(_buffTime);
        Attack -= p;
        _debuffList.Remove(_buffDict[Define.Buff.AttackIncrease]);
        Debug.Log($"end {time}");
    }

    private IEnumerator AttackSpeedBuff()
    {
        Debug.Log(gameObject.name);
        float p = AttackSpeed * _buffParam;
        AttackSpeed += p;
        yield return new WaitForSeconds(_buffTime);
        AttackSpeed -= p;
        _debuffList.Remove(_buffDict[Define.Buff.AttackSpeedIncrease]);
    }

    private IEnumerator HealthBuff()
    {
        int p = (int)(MaxHp * _buffParam);
        Hp += p;
        MaxHp += p;
        yield return new WaitForSeconds(_buffTime);
        MaxHp -= p;
        if (Hp > MaxHp) Hp = MaxHp;
        _debuffList.Remove(_buffDict[Define.Buff.HealthIncrease]);
    }

    private IEnumerator DefenceBuff()
    {
        int p = (int)_buffParam;
        Defense += p;
        yield return new WaitForSeconds(_buffTime);
        Defense -= p;
        _debuffList.Remove(_buffDict[Define.Buff.DefenceIncrease]);
    }

    private IEnumerator MoveSpeedBuff()
    {
        float p = MoveSpeed * _buffParam;
        MoveSpeed += p;
        yield return new WaitForSeconds(_buffTime);
        MoveSpeed -= p;
        _debuffList.Remove(_buffDict[Define.Buff.MoveSpeedIncrease]);
    }

    private IEnumerator Invincible()
    {
        GameObject effect = Managers.Resource.Instanciate("Effects/HolyAura", gameObject.transform);
        yield return new WaitForSeconds(_buffTime);
        Managers.Resource.Destroy(effect);
        _debuffList.Remove(_buffDict[Define.Buff.Invincible]);
    }

    private IEnumerator AttackDebuff()
    {
        int p = (int)(Attack * _buffParam);
        Attack -= p;
        yield return new WaitForSeconds(_buffTime);
        Attack += p;
        _debuffList.Remove(_debuffDict[Define.Debuff.AttackDecrease]);
    }

    private IEnumerator AttackSpeedDebuff()
    {
        float p = AttackSpeed * _buffParam;
        AttackSpeed -= p;
        yield return new WaitForSeconds(_buffTime);
        AttackSpeed += p;
        _debuffList.Remove(_debuffDict[Define.Debuff.AttackSpeedDecrease]);
    }

    private IEnumerator DefenceDebuff()
    {
        int p = (int)_buffParam;
        Defense -= p;
        yield return new WaitForSeconds(_buffTime);
        Defense += p;
        _debuffList.Remove(_debuffDict[Define.Debuff.DefenceDecrease]);
    }

    private IEnumerator MoveSpeedDebuff()
    {
        float p = MoveSpeed * _buffParam;
        MoveSpeed -= p;
        yield return new WaitForSeconds(5f);
        MoveSpeed += p;
        _debuffList.Remove(_debuffDict[Define.Debuff.MoveSpeedDecrease]);
    }

    private IEnumerator Curse()
    {
        yield return new WaitForSeconds(_buffTime);
        Hp /= 2;
        _debuffList.Remove(_debuffDict[Define.Debuff.Curse]);
    }

    private IEnumerator Addicted()
    {
        float time = Time.time;
        Debug.Log($"start {time}");
        float intervalTime = 1.0f;
        float startTime = Time.time;
        int poison = (int)(MaxHp * _buffParam);
        while (startTime + _buffTime >= Time.time)
        {
            Hp -= poison;
            yield return new WaitForSeconds(intervalTime);
        }
        _debuffList.Remove(_debuffDict[Define.Debuff.Addicted]);
        Debug.Log($"end {time}");
    }

    private IEnumerator DeadlyAddicted()
    {
        float intervalTime = 1.0f;
        float time = Time.time;
        int poison = (int)(MaxHp * _buffParam);
        Hp -= poison;
        for (int i = 0; i < (int)(_buffTime + 0.1); i++)
        {
            if (Time.time > time + intervalTime)
            {
                time = Time.time;
                Hp -= poison;
            }
        }
        yield return new WaitForSeconds(_buffTime);
        _debuffList.Remove(_debuffDict[Define.Debuff.DeadlyAddicted]);
    }

    private IEnumerator AggroFunc()
    {
        Aggro = true;
        yield return new WaitForSeconds(_buffTime);
        Aggro = false;
        _debuffList.Remove(_debuffDict[Define.Debuff.Aggro]);
    }
}
