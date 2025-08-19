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
        gap = 2.0f;
    }

    private void Awake()
    {
        Init();
    }

    public override IEnumerator Attack(CharacterBase attacker, CharacterBase target)
    {
        if (characterState == CharacterState.Die)
        {
            yield break;
        }

        characterState = CharacterState.Attack;
        var originPos = transform.position;

        float dir = formation ? +1.0f : -1.0f;

        // 상대의 바로 앞으로 이동
        var targetPos = (Vector2)target.transform.position;
        var moveToPos = targetPos + new Vector2(dir * gap, 0f);
        transform.position = moveToPos;

        // 공격 적용
        anim.SetTrigger(attackHash);
        yield return new WaitForSeconds(attackActiveTime);
        GameManager.gameManager.ApplyDamage(attacker, target);
        yield return new WaitForSeconds(returnAfterAttackTime);

        // 공격 모션 만큼의 시간이 지나면 원래 위치로
        transform.position = originPos;
        characterState = CharacterState.Idle;
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
