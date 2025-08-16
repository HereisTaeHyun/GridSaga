using System.Collections;
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
        Debug.Log($"{this.name} attack");

        var originPos = transform.position;

        // 상대의 바로 앞으로 이동
        var targetPos = target.transform.position;
        var moveToPos = new Vector2(targetPos.x - 1.0f, targetPos.y);
        transform.position = moveToPos;

        // 공격 모션 만큼의 시간이 지나면 원래 위치로
        yield return new WaitForSeconds(0.5f);
        transform.position = originPos;
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
        Debug.Log($"{this.name} Get {damage} damage");

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
        Debug.Log($"{this.name} died");
        DungeonManager.dungeonManager.unitOnStage.Remove(this);
        Destroy(gameObject);
    }
}
