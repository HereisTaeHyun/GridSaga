using UnityEngine;

public class SkillBase : MonoBehaviour
{
    [SerializeField] SkillSO skillData;
    protected int skillId;
    protected string skillName;
    protected SkillKind skillKind;
    protected StatKind statKind;
    protected string description;

    public int SkillId => skillId;
    public string SkillName => skillName;
    public SkillKind SkillKind => skillKind;
    public StatKind StatKind => statKind;
    public string Description => description;

    protected ICombat character;

    protected virtual void Init()
    {
        skillId = skillData.SkillId;
        skillName = skillData.SkillName;
        skillKind = skillData.SkillKind;
        description = skillData.Description;
        statKind = skillData.StatKind;

        character = GetComponent<ICombat>();
    }

    protected virtual void SkillTrigger()
    {

    }
}
