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

    protected override void Move()
    {
        isMove = characterCtrl.Move.magnitude > 0.0001f;
        anim.SetBool(isMoveHash, isMove);

        Vector2 dir = UtilityManager.utility.DirSet(characterCtrl.Move);

        if (isMove)
        {
            characterState = CharacterState.Move;
            lastDir.x = dir.x;
            lastDir.y = dir.y;
        }
        else
        {
            characterState = CharacterState.Idle;
        }

        anim.SetFloat(moveXHash, lastDir.x);
        anim.SetFloat(moveYHash, lastDir.y);

        Vector2 newVelocity = new Vector2(currentSpeed * characterCtrl.Move.x, currentSpeed * characterCtrl.Move.y);
        rb2D.linearVelocity = newVelocity;
    }

    protected override IEnumerator Attack()
    {
        if (characterState == CharacterState.Die || characterState == CharacterState.Move)
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
        float cos = Vector2.Dot(directionToTarget, lastDir);
        if (cos >= attackThreshold)
        {
            return true;
        }
        return false;
    }

    public override void UseActiveSkill()
    {
        anim.SetTrigger(useActiveSkillHash);
        activeSkill.SkillTrigger();
    }

    // hp 변화에 따른 스킬이기에 이벤트에 등록하기 위해 필요
    private void ListenEvent(int currentHp, int Maxhp)
    {
        UsePassiveSkill();
    }

    public override void UsePassiveSkill()
    {
        passiveSkill.SkillTrigger();
        if (isPassiveTriggered)
        {
            anim.SetBool(isBuffHash, true);
            anim.SetTrigger(usePassiveSkillHash);
        }
        else if (!isPassiveTriggered)
        {
            anim.SetBool(isBuffHash, false);
        }
    }
}
