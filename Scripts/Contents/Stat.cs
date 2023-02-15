using System.Collections;
using System.Collections.Generic;
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
    
    protected bool _reflection;
    protected bool _reflectionSkill;
    protected float _reflectionRate;

    private bool _healthInRound = false;
    private bool _slowInRound = false;
    private bool _slowAttackInRound = false;
    private bool _attackInRound = false;
    private bool _attackSpeedInRound = false;
    private bool _defenceInRound = false;
    private bool _attackDebuffInRound = false;
    private bool _defenceDebuffInRound = false;
    
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
        
        if (ReflectionSkill)
        {
            OnReflection(attacker, damage);
        }
        
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
                Managers.Game.Despawn(gameObject, 0);
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

    public virtual void Heal(int heal)
    {
        Hp += heal;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }

    
    
    public IEnumerator HealthInRounds(int health)
    {
        if (_healthInRound) yield break;
        _healthInRound = true;

        float roundTime = GameData.RoundTime;
        float remainTime = Time.time % roundTime;

        Hp += health;
        MaxHp += health;
        
        yield return new WaitForSeconds(remainTime);
        _healthInRound = false;
        MaxHp -= health;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }

    public IEnumerator AttackInRounds(float param)
    {
        if (_attackInRound) yield break;
        _attackInRound = true;
        
        float roundTime = GameData.RoundTime;
        float remainTime = Time.time % roundTime;
        int attack = (int)(Attack * param);
        Attack += attack;

        yield return new WaitForSeconds(remainTime);
        _attackInRound = false;
        Attack -= attack;
    }

    public IEnumerator AttackSpeedInRounds(float param)
    {
        if (_attackSpeedInRound) yield break;
        _attackSpeedInRound = true;
        
        float roundTime = GameData.RoundTime;
        float remainTime = Time.time % roundTime;
        int attackSpeed = (int)(AttackSpeed * param);
        AttackSpeed += attackSpeed;

        yield return new WaitForSeconds(remainTime);
        _attackSpeedInRound = false;
        AttackSpeed -= attackSpeed;
    }
    
    public IEnumerator DefenceInRound(int param)
    {
        if (_defenceInRound) yield break;
        _defenceInRound = true;
        
        float roundTime = GameData.RoundTime;
        float remainTime = Time.time % roundTime;
        Defense += param;

        yield return new WaitForSeconds(remainTime);
        _defenceInRound = false;
        Defense -= param;
    }

    public IEnumerator AttackDebuffInRounds(float param)
    {
        if(_attackDebuffInRound) yield break;
        _attackDebuffInRound = true;
        
        float roundTime = GameData.RoundTime;
        float remainTime = Time.time % roundTime;
        int attack = (int)(Attack * param);
        Attack -= attack;

        yield return new WaitForSeconds(remainTime);
        _attackDebuffInRound = false;
        Attack += attack;
    }

    public IEnumerator DefenceDebuffInRounds(int param)
    {
        if (_defenceDebuffInRound) yield break;
        _defenceDebuffInRound = true;
        
        float roundTime = GameData.RoundTime;
        float remainTime = Time.time % roundTime;
        Defense -= param;

        yield return new WaitForSeconds(remainTime);
        _defenceInRound = false;
    }

    public IEnumerator SlowInRounds(float param)
    {
        if (_slowInRound) yield break;
        _slowInRound = true;

        float roundTime = GameData.RoundTime;
        float remainTime = Time.time % roundTime;
        float moveSpeed = MoveSpeed * param;
        MoveSpeed -= moveSpeed;
        
        yield return new WaitForSeconds(remainTime);
        _slowInRound = false;
        MoveSpeed += moveSpeed;
    }

    public IEnumerator SlowAttackInRounds(float param)
    {
        if (_slowAttackInRound) yield break;
        _slowAttackInRound = true;
        
        float roundTime = GameData.RoundTime;
        float remainTime = Time.time % roundTime;
        float attackSpeed = AttackSpeed;

        AttackSpeed *= (1f - param);
        
        yield return new WaitForSeconds(remainTime);
        _slowAttackInRound = false;
        AttackSpeed = attackSpeed;
    }
}
