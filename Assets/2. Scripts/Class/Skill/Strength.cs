using UnityEngine;

public class Strength : SkillBase
{
    [SerializeField] private StatKind statKind;

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
        character.HpChanged += ListenEvent;
    }

    void OnDisable()
    {

    }

    private void ListenEvent(int currentHp, int Maxhp)
    {
        SkillTrigger();
    }

    // 피격 당한 캐릭터의 hp가 30퍼센트 이하면 트리거
    protected override void SkillTrigger()
    {
        if ((character.CurrentHp * 100.0f / character.MaxHp) <= triggerHpPercent)
        {
            character.UsePassiveSkill();
            character.ChangeStat(statKind, value);
        }
    }
}
