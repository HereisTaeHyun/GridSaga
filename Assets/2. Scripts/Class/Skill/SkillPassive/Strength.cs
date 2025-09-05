using UnityEngine;

public class Strength : SkillBase
{
    private StatKind statKind = StatKind.attack;
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

    // 피격 당한 캐릭터의 hp가 30퍼센트 이하면 트리거, 상승하면 원복
    public override void SkillTrigger()
    {
        float hpPercent = character.CurrentHp * 100f / character.MaxHp;
        bool isTrigger = hpPercent <= triggerHpPercent;

        if (isTrigger && !character.IsPassiveTriggered)
        {
            character.IsPassiveTriggered = true;
            character.ChangeStat(statKind, value);
        }
        else if (!isTrigger && character.IsPassiveTriggered)
        {
            character.IsPassiveTriggered = false;
            character.ChangeStat(statKind, -value);
        }
    }
}
