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

    public override void Attack()
    {
        Debug.Log($"{this.name} attack");
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
