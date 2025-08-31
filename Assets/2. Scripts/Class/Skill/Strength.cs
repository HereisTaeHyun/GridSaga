using UnityEngine;

public class Strength : SkillBase
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

    }

    void OnDisable()
    {

    }

    // 피격 당한 상대의 hp가 30퍼센트 이하면 트리거
    protected override void SkillTrigger()
    {
        if ((character.CurrentHp * 100f / character.MaxHp) <= triggerHpPercent)
        {
            character.UsePassiveSkill();
            character.ChangeStat(statKind, value);
        }
    }
}
