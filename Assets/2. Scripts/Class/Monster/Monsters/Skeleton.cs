using System.Collections;
using UnityEngine;

public class Skeleton : MonsterBase
{
    protected override void Init()
    {
        base.Init();

        attackActiveTime = 0.25f;
        attackEndTime = 0.8f;
        dieTime = 1.1f;
        sightRange = 15.0f;
    }

    private void Awake()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (currentTarget != null)
        {
            Move(currentTarget);
        }
        else
        {
            anim.SetBool(isMoveHash, false);
        }
    }

    // 사거리 내의 적이면 정지 후 공격
    protected override IEnumerator Attack(ICombat target)
    {
        if (monsterState == MonsterState.Die)
        {
            yield break;
        }

        monsterState = MonsterState.Attack;
        anim.SetBool(isMoveHash, false);

        // 공격 딜레이 적용
        float wait = GetDelay(CurrentSpeed);
        yield return new WaitForSeconds(wait);

        int damage = UtilityManager.utility.CalculateDamage(this, target);

        // 타겟이 존재하면 공격 아니면 Idle
        if (currentTarget == null)
        {
            monsterState = MonsterState.Idle;
            yield break;
        }

        // 공격 적용
        anim.SetTrigger(attackHash);
        yield return new WaitForSeconds(attackActiveTime);

        currentTarget.GetDamage(this, target, damage);
        target.InvokeDamageDataEvent(damage);

        yield return new WaitForSeconds(attackEndTime);
        monsterState = MonsterState.Idle;
    }

    // 데미지 처리
    public override void GetDamage(ICombat attacker,ICombat target, int damage)
    {
        // 데미지는 음수 불가
        int safeDamage = Mathf.Max(0, damage);

        currentHp -= safeDamage;
        if (currentHp <= 0)
        {
            Die();
        }
    }
}
