using System;
using System.Collections;
using UnityEngine;

public class Warrior : CharacterBase
{
    protected override void Init()
    {
        base.Init();

        attackActiveTime = 0.25f;
        attackEndTime = 0.8f;

        activeSkillActiveTime = 0.25f;
        activeSkillEndTime = 0.8f;
        activeSkillCoolDown = 5.0f;

        dieTime = 1.5f;

        attackDegree = 90.0f;
        attackThreshold = Mathf.Cos(attackDegree * Mathf.Deg2Rad / 2.0f);
    }

    private void Awake()
    {
        Init();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        HpChanged += ListenEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        HpChanged -= ListenEvent;
    }

    void FixedUpdate()
    {
        if (characterState == CharacterState.Die) return;

        Move();
    }

    protected override IEnumerator Attack()
    {
        if (characterState == CharacterState.Die)
        {
            yield break;
        }

        characterState = CharacterState.Attack;
        canAttack = false;

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
        characterState = CharacterState.Idle;
        canAttack = true;
    }

    // 공격 각도 계산 함수
    private bool IsInAttackSectorDeg(ICombat target)
    {
        attackThreshold = Mathf.Cos(0.5f * attackDegree * Mathf.Deg2Rad);
        Vector2 directionToTarget = (target.Position - transform.position).normalized;
        float cos = Vector2.Dot(directionToTarget, lastLookDir);
        if (cos >= attackThreshold)
        {
            return true;
        }
        return false;
    }

    // Warrior의 스킬은 범위 내 적 밀어내기
    protected override IEnumerator ActiveSkill()
    {
        StartCoroutine(ActiveSkillCoolDown());
        anim.SetTrigger(useActiveSkillHash);

        yield return new WaitForSeconds(activeSkillActiveTime);

        // 공격 범위 내 적들에게 공격 적용 및 밀려나게 함
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentAttackRange, attackableLayer);
        foreach (var collider in colliders)
        {
            var target = collider.GetComponent<ICombat>();

            int damage = UtilityManager.utility.CalculateDamage(this, target);
            target.GetDamage(target, damage);
        }

        yield return new WaitForSeconds(activeSkillEndTime);
    }

    // hp 변화에 따른 스킬이면 이벤트에 등록하기 위해 필요
    private void ListenEvent(int currentHp, int Maxhp)
    {
        UsePassiveSkill();
    }

    public override void UsePassiveSkill()
    {
        bool prevPassiveActive = IsPassiveTriggered;
        passiveSkill.SkillTrigger();
        bool currPassiveActive = IsPassiveTriggered;

        if (currPassiveActive == prevPassiveActive) return;

        if (currPassiveActive)
        {
            anim.SetBool(isBuffHash, true);
            anim.SetTrigger(usePassiveSkillHash);
        }
        else if (!currPassiveActive)
        {
            anim.SetBool(isBuffHash, false);
        }
    }
}
