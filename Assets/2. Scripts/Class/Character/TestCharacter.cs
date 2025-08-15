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
    }
    
    protected override void Die()
    {
        Debug.Log($"{this.name} died");
    }
}
