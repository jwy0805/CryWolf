using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfController : MonsterController
{
    private bool _skill = true;
    private int _count = 0;
    
    protected override void Init()
    {
        base.Init();
        
        _stat.Hp = 400;
        _stat.MaxHp = 400;
        _stat.Mp = 0;
        _stat.maxMp = 0;
        _stat.Attack = 100;
        _stat.Skill = 300;
        _stat.Defense = 5;
        _stat.MoveSpeed = 7.0f;
        _stat.AttackRange = 2.5f;
    }
    
    protected override void UpdateAttack()
    {
        // 스킬 업그레이드 완료
        if (_skill)
        {
            State = _count % 2 == 0 ? Define.State.Skill : Define.State.Skill2;
        }
        else
        {
            base.UpdateAttack();
        }
    }
    
    protected override void UpdateDie()
    {
        _count = 0;
        base.UpdateDie();
    }
    
    protected override void UpdateSkill()
    {
        base.UpdateAttack();
    }

    private void OnSkillEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            GameObject go = Managers.Resource.Instanciate("Effects/Lightning Strike");

            targetStat.OnSkilled(_stat);
            go.transform.position = _lockTarget.transform.position;
            _count++;
        }
    }
}
