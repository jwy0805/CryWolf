using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class BaseController : MonoBehaviour, ISkillObserver
{
    [SerializeField] protected Vector3 _destPos;
    [SerializeField] protected Define.State _state = Define.State.Idle;
    [SerializeField] public GameObject _lockTarget;
    private string[] _tags = new [] {"Tower", "Fence"};
    protected Animator _anim;
    protected Rigidbody _rigidbody;

    protected SkillSubject _skillSubject;
    protected List<string> _skillList = new List<string>();
    protected string _newSkill;
    protected readonly int _attackSpeed = Animator.StringToHash("AttackSpeed");

    public string[] Tags { get => _tags; set => _tags = value; }
    protected virtual string NewSkill { get; set; }
    public Define.Way Way { get; set; }
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;
    
    public virtual Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;

            _anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Die:
                    _anim.CrossFade("DIE", 0.1f);
                    break;
                case Define.State.Idle:
                    _anim.CrossFade("IDLE", 0.1f);
                    break;
                case Define.State.Moving:
                    _anim.CrossFade("RUN", 0.1f);
                    break;
                case Define.State.Rush:
                    _anim.CrossFade("RUSH", 0.1f);
                    break;
                case Define.State.Attack:
                    _anim.CrossFade("ATTACK", 0.1f, -1, 0.0f);
                    break;
                case Define.State.Skill:
                    _anim.CrossFade("SKILL", 0.1f, -1, 0.0f);
                    break;
                case Define.State.Skill2:
                    _anim.CrossFade("SKILL2", 0.1f, -1, 0.0f);
                    break;
                case Define.State.Jump:
                    _anim.CrossFade("JUMP", 0.1f);
                    break;
                case Define.State.KnockBackCreeper:
                    _anim.CrossFade("RUSH", 0.1f);
                    break;
                case Define.State.Faint:
                    _anim.CrossFade("FAINT", 0.1f);
                    break;
            }
        }
    }

    private void Start()
    {
        Init();
    }

    protected virtual void Update()
    {
        switch (State)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Rush:
                UpdateRush();
                break;
            case Define.State.Attack:
                UpdateAttack();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
            case Define.State.Skill2:
                UpdateSkill2();
                break;
            case Define.State.KnockBackCreeper:
                UpdateKnockBackCreeper();
                break;
            case Define.State.Faint:
                break;
        }
    }

    protected virtual void Init()
    {
        _skillSubject = GameObject.Find("Subject").GetComponent<SkillSubject>();
        _skillSubject.AddObserver(this);
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
    
    protected virtual void UpdateDie() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateRush() { }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateSkill2() { }
    protected virtual void UpdateKnockBackCreeper() { }
    protected virtual void UpdateFaint() { }

    protected IEnumerator Despawn(GameObject go, float animPlayTime)
    {
        yield return new WaitForSeconds(animPlayTime);
        Managers.Game.Despawn(go);
    }

    public virtual void OnSkillUpgraded(string skillName)
    {
        // if (gameObject.activeSelf == false) return;
        bool contains = skillName.Contains(gameObject.name);
        if (contains)
        {
            NewSkill = skillName;
            _skillList.Add(NewSkill);
        }
    }

    protected virtual void SkillInit()
    {
        List<string> skillUpgradedList = GameData.SkillUpgradedList;
        if (skillUpgradedList.Count == 0) return;
        foreach (var skill in skillUpgradedList)
        {
            bool contains = skill.Contains(gameObject.name);
            if (contains)
            {
                _skillList.Add(skill);
            }
        }
        
        if (_skillList != null)
        {
            foreach (string skill in _skillList)
            {
                NewSkill = skill;
            }
        }
    }
}
