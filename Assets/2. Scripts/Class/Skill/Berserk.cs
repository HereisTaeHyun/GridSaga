using UnityEngine;

public class Berserk : SkillBase
{
    [SerializeField] StatKind statKind;

    protected override void Init()
    {
        base.Init();
    }
        void Awake()
    {
        Init();
    }

    protected override void SkillTrigger()
    {

    }
}
