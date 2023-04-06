using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SpikeController : MonsterController
{
    private bool _knockBack;
    private bool _lostHeal = false;
    private bool _attackBuff = false;
    private bool _defenceBuff = false;
    private bool _doubleBuff = false;
    private float _sight;
    private float _rollingSpeed;
    private float _mpTime = 1f;
    private float _lastMpTime = 0f;
    private float _crashTime;
    private float _rollCoolTime = 3f;
    private bool _start = false;
    private Vector3 _dir;
    
    private bool KnockBack
    {
        get => _knockBack;
        set
        {
            _knockBack = value;
            if (_knockBack)
            {
                if (_lostHeal) _stat.Hp += (int)((_stat.MaxHp - _stat.Hp) * 0.2f);
                _destPos = -(_dir.normalized * 8.0f);
                _destPos.y = 6.0f;
                State = Define.State.KnockBack;
            }
        }
    }
    
    protected override string NewSkill
    {
        get => _newSkill;
        set
        {
            _newSkill = value;
            Define.Skill skill = (Define.Skill)Enum.Parse(typeof(Define.Skill), _newSkill);

            switch (skill)
            {
                case Define.Skill.SpikeSelfDefence:
                    _stat.Defense += 10;
                    break;
                case Define.Skill.SpikeLostHeal:
                    _lostHeal = true;
                    break;
                case Define.Skill.SpikeAttack:
                    _attackBuff = true;
                    break;
                case Define.Skill.SpikeDefence:
                    _defenceBuff = true;
                    break;
                case Define.Skill.SpikeDoubleBuff:
                    _doubleBuff = true;
                    break;
            }
        }
    }
    
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 200;
        _stat.MaxHp = 200;
        _stat.maxMp = 50;
        _stat.Mp = 0;
        _stat.Attack = 35;
        _stat.Skill = 40;
        _stat.AttackSpeed = 0.75f;
        _stat.Defense = 7;
        _stat.MoveSpeed = 4.0f;
        _stat.AttackRange = 4.0f;

        _sight = 20.0f;
        
        _lastMpTime = Time.time;
        _crashTime = 0f;
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time > _lastMpTime + _mpTime)
        {
            _stat.Mp += 4;
            _lastMpTime = Time.time;
        }
    }

    protected override void UpdateIdle()
    {
        if (Time.time < _crashTime + _rollCoolTime && _start) return;
        _start = true;
        if (_stat.Mp > _stat.maxMp) State = Define.State.Skill;
        base.UpdateIdle();
    }
    
    protected override void UpdateMoving()
    {
        // Targeting
        if (Time.time > _lastTargetingTime + _targetingTime)
        {
            _lastTargetingTime = Time.time;
            // 이미 Fence 내부에 있으면
            if (GameData.FenceBounds.Contains(transform.position))
            {
                Tags = new[] { "Sheep", "Tower" };
            }
            else
            {
                // Fence 안으로 들어갈 수 있는지?
                Tags = IsReachable(GameData.Center) ? new[] { "Sheep", "Tower" } :
                    // Fence 안으로 들어갈 수 없으면
                    new[] { "Fence", "Tower" };
            }
            SetTarget(Tags);
        }
    
        // Attack
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Collider targetCollider = _lockTarget.GetComponent<Collider>();
            Vector3 position = transform.position;
            if (targetStat.Targetable == false) return;
        
            _destPos = targetCollider.ClosestPoint(position);
            float distance = (_destPos - position).magnitude;
            if (distance < _sight)
            {
                _navMesh.SetDestination(transform.position);
                State = Define.State.Rush;
                return;
            }
        }
    
        // Move
        Vector3 dir= _destPos - transform.position;
    
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            _navMesh.SetDestination(_destPos);
            _navMesh.speed = _stat.MoveSpeed;
        }
    }

    protected override void UpdateRush()
    {
        _rollingSpeed = _stat.MoveSpeed + 2.0f;
        
        if (_stat.Mp > _stat.maxMp) State = Define.State.Skill;
        if (Time.time > _lastTargetingTime + _targetingTime)
        {
            // Fence 안으로 들어갈 수 있는지?
            Tags = IsReachable(GameData.Center) ? new[] { "Sheep", "Tower" } :
                // Fence 안으로 들어갈 수 없으면
                new[] { "Fence", "Tower" };
            SetTarget(Tags);
        }
        
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            if (targetStat.Targetable == false) return;
            _destPos = _lockTarget.transform.position;
        }

        // Move
        _dir = _destPos - transform.position;
        _navMesh.SetDestination(_destPos);
        _navMesh.speed = _rollingSpeed;
    }
    
    protected override void UpdateKnockBack()
    {
        _navMesh.SetDestination(_destPos);
        _dir = _destPos - transform.position;
        if (_dir.magnitude < 0.1f) State = Define.State.Idle;
    }

    protected override void UpdateSkill()
    {
        _destPos = transform.position;
        _navMesh.SetDestination(_destPos);
    }

    private List<Collider> PickUnits(int num)
    {
        float height = 6f;
        Vector3 pos = transform.position;
        Vector3 pos1 = new Vector3(pos.x, pos.y - height, pos.z);
        Vector3 pos2 = new Vector3(pos.x, pos.y + height, pos.z);
        Collider[] colliders = Physics.OverlapCapsule(pos1, pos2, 20, 1 << (int)Define.Layer.Monster);
        List<Collider> colliderList = new List<Collider>(colliders);
        List<Collider> toRemove = new List<Collider>();
        foreach (var collider in colliderList)
        {
            if (collider.gameObject.name == gameObject.name) toRemove.Add(collider);
        }

        foreach (var collider in toRemove)
        {
            colliderList.RemoveAll(x => x == collider);
        }
        colliderList.Sort(new DistanceComparer(transform.position));
        
        return colliderList.Count == 0 ? colliderList : colliderList.GetRange(0, num);
    }
    
    private void OnSkillEvent()
    {
        _stat.Mp = 0;

        List<Collider> monsterList = new List<Collider>(PickUnits(_doubleBuff ? 2 : 1));
        if (monsterList.Count == 0) return;
        foreach (var monster in monsterList)
        {
            Stat stat = monster.GetComponent<Stat>();
            stat.ApplyingBuff(10, 2.0f, Define.BuffList.MoveSpeedIncrease);
            stat.ApplyingBuff(10, 0.1f, Define.BuffList.AttackSpeedIncrease);
            if (_attackBuff) stat.ApplyingBuff(10, 0.25f, Define.BuffList.AttackIncrease);
            if (_defenceBuff) stat.ApplyingBuff(10, 6, Define.BuffList.DefenceIncrease);
        }
    }
    
    protected override void OnEndEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Collider targetCollider = _lockTarget.GetComponent<Collider>();
            Vector3 position = transform.position;
            
            if (targetStat.Hp > 0)
            {
                float distance = (targetCollider.ClosestPoint(position) - position).magnitude;
                State = distance <= _sight ? Define.State.Moving : Define.State.Rush;
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
    
    private void OnCollisionEnter(Collision collision)
    {
        if (Tags.Contains(collision.gameObject.tag) && State == Define.State.Rush)
        {
            if (collision.gameObject.TryGetComponent(out Stat targetStat))
            {
                targetStat.OnSkilled(_stat);
                _stat.Mp += 4;
                _crashTime = Time.time;
                KnockBack = true;
                // 충돌음 재생
            }
        }

        if (KnockBack && State == Define.State.Rush)
        {
            KnockBack = false;
            State = Define.State.Idle;
        }
    }
}
