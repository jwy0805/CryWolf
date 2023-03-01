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
    [SerializeField] protected int _level;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _mp;
    [SerializeField] protected int _maxMp;
    [SerializeField] protected int _attack;
    [SerializeField] protected int _defense;
    [SerializeField] protected int _fireResist;
    [SerializeField] protected int _poisonResist;
    [SerializeField] protected int _skill;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _attackSpeed;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _criticalChance;
    [SerializeField] protected float _criticalMultiplier;
    [SerializeField] protected float _accuracy;
    [SerializeField] protected float _evasion;
    [SerializeField] protected bool _targetable;
    [SerializeField] protected bool _aggro = false;

    protected bool _reflection;
    protected bool _reflectionSkill;
    protected float _reflectionRate;

    private float _time = 0f;
    private float _param = 0f;
    
    private static readonly int AnimAttackSpeed = Animator.StringToHash("AttackSpeed");

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
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

    private List<Define.Buff> _buffList = new List<Define.Buff>();
    private List<Define.Debuff> _debuffList = new List<Define.Debuff>();
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
        if (_buffList.Contains(Define.Buff.Invincible)) return;
        int damage = 0;
        var random = new System.Random();
        int randVal = random.Next(100);
        
        if (randVal >= (int)attacker.Accuracy - (int)Evasion) return;
        if (randVal < (int)attacker.CriticalChance)
        {
            damage = (int)(Mathf.Max(0, attacker.Attack - Defense) * CriticalMultiplier); 
        }
        else
        {
            damage = Mathf.Max(0, attacker.Attack - Defense);
        }

        Mp += 2;
        Hp -= damage;
        
        if (Reflection)
        {
            OnReflection(attacker, damage);
        }
        
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    public virtual void OnSkilled(Stat attacker)
    {
        if (_buffList.Contains(Define.Buff.Invincible)) return;
        int damage = 0;
        var random = new System.Random();
        int randVal = random.Next(100);
        
        if (randVal < (int)attacker.CriticalChance)
        {
            damage = (int)(Mathf.Max(0, attacker.Attack - Defense) * CriticalMultiplier); 
        }
        else
        {
            damage = Mathf.Max(0, attacker.Skill - Defense);
        }

        Mp += 2;
        Hp -= damage;
        
        if (ReflectionSkill) OnReflection(attacker, damage);

        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
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
        if (attacker.Hp <= 0)
        {
            attacker.Hp = 0;
            attacker.OnDead();
        }
    }
    
    public virtual void Heal(float heal)
    {
        Hp += (int)heal;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }

    private void RegisterBuff()
    {
        // Define.Buff 와 순서 맞출것
        IEnumerator[] buffArr =
        {
            AttackBuff(Define.Buff.Attack), AttackSpeedBuff(Define.Buff.AttackSpeed),
            HealthBuff(Define.Buff.Health), DefenceBuff(Define.Buff.Defence), MoveSpeedBuff(Define.Buff.MoveSpeed),
            Invincible(Define.Buff.Invincible)
        };
        IEnumerator[] debuffArr =
        {
            AttackDebuff(Define.Debuff.Attack), AttackSpeedDebuff(Define.Debuff.AttackSpeed),
            DefenceDebuff(Define.Debuff.Defence), MoveSpeedDebuff(Define.Debuff.MoveSpeed),
            Curse(Define.Debuff.Curse), Addicted(Define.Debuff.Addicted), AggroFunc(Define.Debuff.Aggro),
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

    public void SetBuffParams(float time, float param, Define.Buff buff)
    {
        SetParams(time, param);
        if (_buffList.Contains(buff))
        {
            StopCoroutine(_buffDict[buff]);
            _buffList.Remove(buff);
        }

        StartCoroutine(_buffDict[buff]);
    }
    
    public void SetDebuffParams(float time, float param, Define.Debuff debuff)
    {
        SetParams(time, param);
        if (_debuffList.Contains(debuff))
        {
            StartCoroutine(_debuffDict[debuff]);
            _debuffList.Remove(debuff);
        }

        StartCoroutine(_debuffDict[debuff]);
    }
    
    public void SetParams(float time, float param)
    {
        _time = time;
        _param = param;
    }
    
    public void RemoveAllDebuff()
    {
        for (int i = 0; i < _debuffList.Count; i++)
        {
            Define.Debuff debuff = _debuffList[i];
            StopCoroutine(_debuffDict[debuff]);
        }
        
        _debuffList.Clear();
    }

    public void RemoveDebuff(Define.Debuff debuff)
    {
        if (_debuffList.Contains(debuff))
        {
            StopCoroutine(_debuffDict[debuff]);
            _debuffList.Remove(debuff);   
        }
    }

    public IEnumerator AttackBuff(Define.Buff buff)
    {
        _buffList.Add(buff);
        int p = (int)(Attack * _param);
        Attack += p;
        yield return new WaitForSeconds(_time);
        _buffList.Remove(buff);
        Attack -= p;
    }
    
    public IEnumerator AttackSpeedBuff(Define.Buff buff)
    {
        Debug.Log(gameObject.name);
        _buffList.Add(buff);
        float p = AttackSpeed * _param;
        AttackSpeed += p;
        yield return new WaitForSeconds(_time);
        _buffList.Remove(buff);
        AttackSpeed -= p;
    }

    public IEnumerator HealthBuff(Define.Buff buff)
    {
        _buffList.Add(buff);
        int p = (int)(MaxHp * _param);
        Hp += p;
        MaxHp += p;
        yield return new WaitForSeconds(_time);
        _buffList.Remove(buff);
        MaxHp -= p;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }

    public IEnumerator DefenceBuff(Define.Buff buff)
    {
        _buffList.Add(buff);
        int p = (int)_param;
        Defense += p;
        yield return new WaitForSeconds(_time);
        _buffList.Remove(buff);
        Defense -= p;
    }
    
    public IEnumerator MoveSpeedBuff(Define.Buff buff)
    {
        _buffList.Add(buff);
        float p = MoveSpeed * _param;
        MoveSpeed += p;
        yield return new WaitForSeconds(_time);
        _buffList.Remove(buff);
        MoveSpeed -= p;
    }

    public IEnumerator Invincible(Define.Buff buff)
    {
        _buffList.Add(buff);
        GameObject effect = Managers.Resource.Instanciate("Effects/HolyAura", gameObject.transform);
        yield return new WaitForSeconds(_time);
        Managers.Resource.Destroy(effect);
        _buffList.Remove(buff);
    }
    
    public IEnumerator AttackDebuff(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        int p = (int)(Attack * _param);
        Attack -= p;
        yield return new WaitForSeconds(_time);
        _debuffList.Remove(debuff);
        Attack += p;
    }
    
    public IEnumerator AttackSpeedDebuff(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        float p = AttackSpeed * _param;
        AttackSpeed -= p;
        yield return new WaitForSeconds(_time);
        _debuffList.Remove(debuff);
        AttackSpeed += p;
    }
    
    public IEnumerator DefenceDebuff(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        int p = (int)_param;
        Defense -= p;
        yield return new WaitForSeconds(_time);
        _debuffList.Remove(debuff);
        Defense += p;
    }
    
    public IEnumerator MoveSpeedDebuff(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        float p = MoveSpeed * _param;
        MoveSpeed -= p;
        yield return new WaitForSeconds(_time);
        _debuffList.Remove(debuff);
        MoveSpeed += p;
    }

    public IEnumerator Curse(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        yield return new WaitForSeconds(_time);
        Hp /= 2;
        _debuffList.Remove(debuff);
    }

    public IEnumerator Addicted(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        float intervalTime = 1.0f;
        float time = Time.time;
        int posion = (int)(MaxHp * _param);
        for (int i = 0; i < (int)(_time + 0.1); i++)
        {
            if (Time.time > time + intervalTime)
            {
                time = Time.time;
                Hp -= posion;
            }
        }
        yield return new WaitForSeconds(_time);
        _debuffList.Remove(debuff);
    }

    public IEnumerator AggroFunc(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        Aggro = true;
        yield return new WaitForSeconds(_time);
        Aggro = false;
        _debuffList.Remove(debuff);
    }
}
