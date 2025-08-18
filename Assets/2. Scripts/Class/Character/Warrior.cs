using System.Collections;
using UnityEngine;

public class Warrior : CharacterBase
{
    // 공격 처리 자연스럽게 하기 위한 변수
    private float attackTime = 0.75f;
    private float gap = 2.0f;

    protected override void Init()
    {
        base.Init();
    }

    private void Awake()
    {
        Init();
    }

    public override IEnumerator Attack(CharacterBase target)
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

        // 공격애니메이션 재생
        anim.SetTrigger(attackHash);

        // 공격 모션 만큼의 시간이 지나면 원래 위치로
        yield return new WaitForSeconds(attackTime);
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
            Die();
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
        anim.SetTrigger(dieHash);
        characterState = CharacterState.Die;
        gameObject.SetActive(false);
    }
}
