using UnityEngine;

public class GroundImpact : SkillBase
{
    protected override void Init()
    {
        base.Init();
    }
    void Awake()
    {
        Init();
    }

    public override void SkillTrigger()
    {
        // 공격 범위 내 적들에게 공격 적용
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, character.CurrentAttackRange, character.AttackableLayer);
        foreach (var collider in colliders)
        {
            var target = collider.GetComponent<ICombat>();

            int damage = UtilityManager.utility.CalculateDamage(character, target);
            target.GetDamage(target, damage);
        }
    }
}
