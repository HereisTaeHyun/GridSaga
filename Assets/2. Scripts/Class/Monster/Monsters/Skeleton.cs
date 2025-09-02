using System.Collections;
using UnityEngine;

public class Skeleton : MonsterBase
{
    protected override void Init()
    {
        base.Init();

        attackActiveTime = 0.5f;
        attackEndTime = 0.8f;
        dieTime = 1.5f;
        sightRange = 15.0f;

        attackDegree = 90.0f;
        attackThreshold = Mathf.Cos(attackDegree * Mathf.Deg2Rad / 2.0f);
    }

    private void Awake()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (monsterState == MonsterState.Die)
        {
            return;
        }
        Debug.Log($"{this} : {currentTarget}");
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            anim.SetBool(isMoveHash, false);
            return;
        }
        Move(currentTarget);
    }

    protected override IEnumerator Attack()
    {
        if (monsterState == MonsterState.Die || monsterState == MonsterState.Move)
        {
            yield break;
        }

        monsterState = MonsterState.Attack;

        // 공격 적용
        anim.SetTrigger(attackHash);
        yield return new WaitForSeconds(attackActiveTime);

        // 공격 범위 내 적들에게 공격 적용
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentAttackRange, attackableLayer);
        foreach (var collider in colliders)
        {
            var target = collider.GetComponent<ICombat>();

            if (IsInAttackSectorDeg(target))
            {
                int damage = UtilityManager.utility.CalculateDamage(this, target);
                target.GetDamage(target, damage);   
            }
        }

        yield return new WaitForSeconds(attackEndTime);
        monsterState = MonsterState.Idle;
    }

    // 공격 각도 계산 함수
    private bool IsInAttackSectorDeg(ICombat target)
    {
        attackThreshold = Mathf.Cos(0.5f * attackDegree * Mathf.Deg2Rad);
        Vector2 directionToTarget = (target.Position - transform.position).normalized;
        float cos = Vector2.Dot(directionToTarget, lastDir);
        if (cos >= attackThreshold)
        {
            return true;
        }
        return false;
    }
}
