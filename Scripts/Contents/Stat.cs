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
    [SerializeField] protected int _resource;
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

    private float _buffTime = 10f;
    private float _buffParam = 0f;
    private Define.BuffList _applyBuff;

    private static readonly int AnimAttackSpeed = Animator.StringToHash("AttackSpeed");

    public int Level { get { return _level; } set { _level = value; } }
    public int Resource { get { return _resource; } set { _resource = value; } }
    public int Hp { get => _hp; set { _hp = value; } }
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

    
    public Define.BuffList ApplyBuff
    {
        get => _applyBuff;
        set
        {
            _applyBuff = value;
            string buffstr = Enum.GetName(typeof(Define.BuffList), _applyBuff);
            string[] buffArr = Enum.GetNames(typeof(Define.Buff));
            string[] debuffArr = Enum.GetNames(typeof(Define.Debuff));
            
            if (buffArr.Contains(buffstr))
            {
                if (buffstr == null) return;
                Define.Buff buff = (Define.Buff)Enum.Parse(typeof(Define.Buff), buffstr);
                if (_buffList.Contains(buff))
                {
                    StopCoroutine(_buffDict[buff]);
                    _buffList.Remove(buff);
                }

                StartCoroutine(_buffDict[buff]);
            }
            else
            {
                if (buffstr == null) return;
                Define.Debuff debuff = (Define.Debuff)Enum.Parse(typeof(Define.Debuff), buffstr);
                bool start;
                
                if (_debuffList.Contains(debuff))
                {
                    start = false;
                    _debuffList.Remove(debuff);
                    SelectDebuff(debuff, start);
                }

                start = true;
                SelectDebuff(debuff, start);
            }
        }
    }
    
    private void SelectBuff(Define.Buff buff, bool start)
    {
        switch (buff)
        {
            case Define.Buff.AttackIncrease:
                if (start) StartCoroutine(AttackBuff(buff));
                else StopCoroutine(AttackBuff(buff));
                break;
            case Define.Buff.DefenceIncrease:
                if (start) StartCoroutine(DefenceBuff(buff));
                else StopCoroutine(DefenceBuff(buff));
                break;
            case Define.Buff.HealthIncrease:
                if (start) StartCoroutine(HealthBuff(buff));
                else StopCoroutine(HealthBuff(buff));
                break;
            case Define.Buff.AttackSpeedIncrease:
                if (start) StartCoroutine(AttackSpeedBuff(buff));
                else StopCoroutine(AttackSpeedBuff(buff));
                break;
            case Define.Buff.MoveSpeedIncrease:
                if (start) StartCoroutine(MoveSpeedBuff(buff));
                else StopCoroutine(MoveSpeedBuff(buff));
                break;
            case Define.Buff.Invincible:
                if (start) StartCoroutine(Invincible(buff));
                else StopCoroutine(Invincible(buff));
                break;
        }
    }

    private void SelectDebuff(Define.Debuff debuff, bool start)
    {
        switch (debuff)
        {
            case Define.Debuff.AttackDecrease:
                if (start) StartCoroutine(AttackDebuff(debuff));
                else StopCoroutine(AttackDebuff(debuff));
                break;
            case Define.Debuff.DefenceDecrease:
                if (start) StartCoroutine(DefenceDebuff(debuff));
                else StopCoroutine(DefenceDebuff(debuff));
                break;
            case Define.Debuff.AttackSpeedDecrease:
                if (start) StartCoroutine(AttackSpeedDebuff(debuff));
                else StopCoroutine(AttackSpeedDebuff(debuff));
                break;
            case Define.Debuff.MoveSpeedDecrease:
                if (start) StartCoroutine(MoveSpeedDebuff(debuff));
                else StopCoroutine(MoveSpeedDebuff(debuff));
                break;
            case Define.Debuff.Curse:
                if (start) StartCoroutine(Curse(debuff));
                else StopCoroutine(Curse(debuff));
                break;
            case Define.Debuff.Addicted:
                if (start) StartCoroutine(Addicted(debuff));
                else StopCoroutine(Addicted(debuff));
                break;
            case Define.Debuff.DeadlyAddicted:
                if (start) StartCoroutine(DeadlyAddicted(debuff));
                else StopCoroutine(DeadlyAddicted(debuff));
                break;
        }
    }
    
    private void RemoveBuff()
    {
            
    }
    
    public void ApplyingBuff(float time, float param, Define.BuffList buff)
    {
        _buffTime = time;
        _buffParam = param;
        ApplyBuff = buff;
    }
    
    
    private List<Define.Buff> _buffList = new List<Define.Buff>();
    private List<Define.Debuff> _debuffList = new List<Define.Debuff>();
    private Dictionary<Define.Buff, IEnumerator> _buffDict = new Dictionary<Define.Buff, IEnumerator>();    
    private Dictionary<Define.Debuff, IEnumerator> _debuffDict = new Dictionary<Define.Debuff, IEnumerator>();

    void Start()
    {
        Targetable = true;
        Accuracy = 100;
        Attack = 0;
        Defense = 0;
        MoveSpeed = 0;
        Evasion = 0;
        FireResist = 0;
        PoisonResist = 0;
        CriticalChance = 0;
        CriticalMultiplier = 1f;
        Reflection = false;
        ReflectionSkill = false;
        ReflectionRate = 0f;

        Resource = 0;
        
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
            damage = (int)(Mathf.Max(0, attacker.Skill - Defense) * CriticalMultiplier); 
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
            AttackBuff(Define.Buff.AttackIncrease), AttackSpeedBuff(Define.Buff.AttackSpeedIncrease),
            HealthBuff(Define.Buff.HealthIncrease), DefenceBuff(Define.Buff.DefenceIncrease), 
            MoveSpeedBuff(Define.Buff.MoveSpeedIncrease), Invincible(Define.Buff.Invincible)
        };
        IEnumerator[] debuffArr =
        {
            AttackDebuff(Define.Debuff.AttackDecrease), AttackSpeedDebuff(Define.Debuff.AttackSpeedDecrease),
            DefenceDebuff(Define.Debuff.DefenceDecrease), MoveSpeedDebuff(Define.Debuff.MoveSpeedDecrease),
            Curse(Define.Debuff.Curse), Addicted(Define.Debuff.Addicted), DeadlyAddicted(Define.Debuff.DeadlyAddicted),
            AggroFunc(Define.Debuff.Aggro),
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
            Debug.Log("asd");
            StopCoroutine(_debuffDict[debuff]);
            _debuffList.Remove(debuff);
        }
        
        StartCoroutine(_debuffDict[debuff]);
    }
    
    private IEnumerator Addicted(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        float time = Time.time;
        Debug.Log($"S{time}");
        yield return new WaitForSeconds(_buffTime);
        _debuffList.Remove(debuff);
        Debug.Log($"E{time}");
        
        
        // _debuffList.Add(debuff);
        // Debug.Log(string.Join(" ", _debuffList));
        // float intervalTime = 1.0f;
        // float startTime = Time.time;
        // float time = startTime - intervalTime;
        // int poison = (int)(MaxHp * _buffParam);
        // while (true)
        // {
        //     if (Time.time >= startTime + _buffTime)
        //         break;
        //     
        //     if (Time.time >= time + intervalTime)
        //     {
        //         time = Time.time;
        //         Debug.Log("at");
        //         Hp -= poison;
        //     }            
        //     yield return null;
        // }
        // _debuffList.Remove(debuff);
    }

    public void Test()
    {
        StartCoroutine(CoTest());
    }
    public IEnumerator CoTest()
    {
        float time = Time.time;
        Debug.Log($"Start {time}");
        yield return new WaitForSeconds(3f);
        Debug.Log($"End {time}");
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

    private IEnumerator AttackBuff(Define.Buff buff)
    {
        _buffList.Add(buff);
        int p = (int)(Attack * _buffParam);
        Attack += p;
        yield return new WaitForSeconds(_buffTime);
        _buffList.Remove(buff);
        Attack -= p;
    }

    private IEnumerator AttackSpeedBuff(Define.Buff buff)
    {
        Debug.Log(gameObject.name);
        _buffList.Add(buff);
        float p = AttackSpeed * _buffParam;
        AttackSpeed += p;
        yield return new WaitForSeconds(_buffTime);
        _buffList.Remove(buff);
        AttackSpeed -= p;
    }

    private IEnumerator HealthBuff(Define.Buff buff)
    {
        _buffList.Add(buff);
        int p = (int)(MaxHp * _buffParam);
        Hp += p;
        MaxHp += p;
        yield return new WaitForSeconds(_buffTime);
        _buffList.Remove(buff);
        MaxHp -= p;
        if (Hp > MaxHp) Hp = MaxHp;
    }

    private IEnumerator DefenceBuff(Define.Buff buff)
    {
        _buffList.Add(buff);
        int p = (int)_buffParam;
        Defense += p;
        yield return new WaitForSeconds(_buffTime);
        _buffList.Remove(buff);
        Defense -= p;
    }

    private IEnumerator MoveSpeedBuff(Define.Buff buff)
    {
        _buffList.Add(buff);
        float p = MoveSpeed * _buffParam;
        MoveSpeed += p;
        yield return new WaitForSeconds(_buffTime);
        _buffList.Remove(buff);
        MoveSpeed -= p;
    }

    private IEnumerator Invincible(Define.Buff buff)
    {
        _buffList.Add(buff);
        GameObject effect = Managers.Resource.Instanciate("Effects/HolyAura", gameObject.transform);
        yield return new WaitForSeconds(_buffTime);
        Managers.Resource.Destroy(effect);
        _buffList.Remove(buff);
    }

    private IEnumerator AttackDebuff(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        int p = (int)(Attack * _buffParam);
        Attack -= p;
        yield return new WaitForSeconds(_buffTime);
        _debuffList.Remove(debuff);
        Attack += p;
    }

    private IEnumerator AttackSpeedDebuff(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        float p = AttackSpeed * _buffParam;
        AttackSpeed -= p;
        yield return new WaitForSeconds(_buffTime);
        _debuffList.Remove(debuff);
        AttackSpeed += p;
    }

    private IEnumerator DefenceDebuff(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        int p = (int)_buffParam;
        Defense -= p;
        yield return new WaitForSeconds(_buffTime);
        _debuffList.Remove(debuff);
        Defense += p;
    }

    private IEnumerator MoveSpeedDebuff(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        float p = MoveSpeed * _buffParam;
        MoveSpeed -= p;
        yield return new WaitForSeconds(5f);
        _debuffList.Remove(debuff);
        MoveSpeed += p;
        Debug.Log("we");
    }

    private IEnumerator Curse(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        yield return new WaitForSeconds(_buffTime);
        Hp /= 2;
        _debuffList.Remove(debuff);
    }

    private IEnumerator DeadlyAddicted(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
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
        _debuffList.Remove(debuff);
    }

    private IEnumerator AggroFunc(Define.Debuff debuff)
    {
        _debuffList.Add(debuff);
        Aggro = true;
        yield return new WaitForSeconds(_buffTime);
        Aggro = false;
        _debuffList.Remove(debuff);
    }
}
