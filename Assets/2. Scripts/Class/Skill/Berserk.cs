using UnityEngine;

public class Berserk : SkillBase
{
    [SerializeField] StatKind statKind;

    private int triggerHpPercent = 30;
    private int value = 2;

    protected override void Init()
    {
        base.Init();
    }
    void Awake()
    {
        Init();
    }

    void OnEnable()
    {
        character.SendDamageData += SkillTrigger;
    }

    void OnDisable()
    {
        character.SendDamageData -= SkillTrigger;
    }

    // 피격 당한 상대의 hp가 30퍼센트 이하면 트리거
    protected override void SkillTrigger(DamageDataBus damageData)
    {
        if ((damageData.currentHp * 100f / character.MaxHp) <= triggerHpPercent && !character.IsPassiveTriggered)
        {
            character.UsePassiveSkill();
            damageData.target.ChangeStat(statKind, value);
        }
    }
}
