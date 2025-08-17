using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TestCharacter : CharacterBase
{
    protected override void Init()
    {
        base.Init();
    }

    void Awake()
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

        float gap = 1.0f;
        float dir = formation ? +1.0f : -1.0f;

        // 상대의 바로 앞으로 이동
        var targetPos = (Vector2)target.transform.position;
        var moveToPos = targetPos + new Vector2(dir * gap, 0f);
        transform.position = moveToPos;

        // 공격 모션 만큼의 시간이 지나면 원래 위치로
        yield return new WaitForSeconds(0.5f);
        transform.position = originPos;
        characterState = CharacterState.Idle;
    }

    protected override void UseActiveSkill()
    {
        Debug.Log($"{this.name} Active Skill Activated");
    }

    protected override void UsePassiveSkill()
    {
        Debug.Log($"{this.name} Passive Skill Activated");
    }

    public override void GetDamage(int damage)
    {
        // 데미지는 음수 불가
        int safeDamage = Mathf.Max(0, damage);

        // hp 차감 후 0 이하면 사망
        currentHp -= safeDamage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
        characterState = CharacterState.Die;
        gameObject.SetActive(false);
    }
}
