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

    public override void SkillTrigger(CharacterBase character)
    {
        if (character.MaxHp / character.CurrentHp <= triggerHpPercent)
        {
            character.ChangeStat(statKind, value);
        }
    }
}
