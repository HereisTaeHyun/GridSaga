using System.Collections;
using UnityEngine;

public class Warrior : CharacterBase
{

    protected override void Init()
    {
        base.Init();

        attackActiveTime = 0.25f;
        returnAfterAttackTime = 0.75f;
        dieTime = 1.5f;
        attackRange = 2.0f;
    }

    private void Awake()
    {
        Init();
    }

    void FixedUpdate()
    {
        
    }

    public override IEnumerator Attack(CharacterBase attacker, CharacterBase target)
    {
        if (characterState == CharacterState.Die)
        {
            yield break;
        }

        // 공격 딜레이 적용
        float wait = GetDelay(attacker.CurrentSpeed);
        yield return new WaitForSeconds(wait);

        // 공격 적용
        anim.SetTrigger(attackHash);
        yield return new WaitForSeconds(attackActiveTime);
        GameManager.gameManager.ApplyDamage(attacker, target);
        yield return new WaitForSeconds(returnAfterAttackTime);
    }

    protected override void Move(CharacterBase target)
    {
        if (Vector2.Distance(transform.position, target.transform.position) > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, currentSpeed * Time.deltaTime);
        }
    }

    protected override void UseActiveSkill()
    {
        Debug.Log($"{this.name} Active Skill Activated");
        anim.SetTrigger(useActiveSkillHash);
    }

    protected override void UsePassiveSkill()
    {
        Debug.Log($"{this.name} Passive Skill Activated");
        anim.SetTrigger(usePassiveSkillHash);
    }

    public override void GetDamage(int damage)
    {
        // 데미지는 음수 불가
        int safeDamage = Mathf.Max(0, damage);

        // hp 차감 후 0 이하면 사망
        currentHp -= safeDamage;
        if (currentHp > 0)
        {
            anim.SetTrigger(takeDamageHash);
        }
        else if (currentHp <= 0)
        {
            StartCoroutine(Die());
        }
    }

    protected override IEnumerator Die()
    {
        characterState = CharacterState.Die;
        anim.SetTrigger(dieHash);
        yield return new WaitForSeconds(dieTime);
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
