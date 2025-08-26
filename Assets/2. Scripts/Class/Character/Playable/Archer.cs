using System.Collections;
using UnityEngine;

public class Archer : CharacterBase
{
    protected override void Init()
    {
        base.Init();

        attackActiveTime = 0.75f;
        attackEndTime = 0.8f;
        dieTime = 1.1f;
    }

    private void Awake()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = SetTarget();
        }

        if (currentTarget != null)
        {
            Move(currentTarget);
        }
        else
        {
            anim.SetBool(isMoveHash, false);
        }
    }

    // 타겟을 향해 움직임
    protected override void Move(CharacterBase target)
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = target.transform.position;
        float distance = Vector2.Distance(transform.position, target.transform.position);

        Vector2 dir = UtilityManager.utility.DirSet(targetPos - currentPos);
        anim.SetFloat(moveXHash, dir.x);
        anim.SetFloat(moveYHash, dir.y);

        if (distance > currentAttackRange)
        {
            anim.SetBool(isMoveHash, true);
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, currentSpeed * Time.deltaTime);
        }
        else if (distance <= currentAttackRange && characterState != CharacterState.Attack)
        {
            StartCoroutine(Attack(target));
        }
    }

    // 사거리 내의 적이면 정지 후 공격
    protected override IEnumerator Attack(CharacterBase target)
    {
        if (characterState == CharacterState.Die)
        {
            yield break;
        }

        characterState = CharacterState.Attack;
        anim.SetBool(isMoveHash, false);

        // 공격 딜레이 적용
        float wait = GetDelay(CurrentSpeed);
        yield return new WaitForSeconds(wait);

        int damage = GameManager.gameManager.CalculateDamage(this, target);

        // 타겟이 존재하면 공격 아니면 Idle
        if (currentTarget == null)
        {
            characterState = CharacterState.Idle;
            yield break;
        }

        var damageData = currentTarget.GetDamage(this, target, damage);

        // 공격 적용
        anim.SetTrigger(attackHash);
        yield return new WaitForSeconds(attackActiveTime);

        target.InvokeDamageDataEvent(damageData);

        yield return new WaitForSeconds(attackEndTime);
        characterState = CharacterState.Idle;
    }

    // 스킬 사용
    public override void UseActiveSkill()
    {
        Debug.Log($"{this.name} Active Skill Activated");
        anim.SetTrigger(useActiveSkillHash);
    }

    public override void UsePassiveSkill()
    {
        Debug.Log($"{this.name} Passive Skill Activated");
        isPassiveTriggered = true;
        anim.SetBool(isBuffHash, true);
        anim.SetTrigger(usePassiveSkillHash);   
    }

    // 데미지 처리
    public override DamageDataBus GetDamage(CharacterBase attacker, CharacterBase target, int damage)
    {
        // 데미지는 음수 불가
        int safeDamage = Mathf.Max(0, damage);

        currentHp -= safeDamage;
        if (currentHp <= 0)
        {
            Die();
        }

        var damageData = new DamageDataBus(attacker, target, safeDamage, currentHp);
        return damageData;
    }
    protected override void Die()
    {
        characterState = CharacterState.Die;
        allyFaction.RemoveDiedUnit(this);
    }
}
