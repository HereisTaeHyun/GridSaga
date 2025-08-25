using UnityEngine;

public class SkillBase : MonoBehaviour
{
    [SerializeField] SkillSO skillData;
    protected int skillId;
    protected string skillName;
    protected SkillKind skillKind;
    protected string description;

    public int SkillId => skillId;
    public string SkillName => skillName;
    public SkillKind SkillKind => skillKind;
    public string Description => description;

    protected CharacterBase character;

    protected virtual void Init()
    {
        skillId = skillData.SkillId;
        skillName = skillData.SkillName;
        skillKind = skillData.SkillKind;
        description = skillData.Description;

        character = GetComponent<CharacterBase>();
    }

    protected virtual void SkillTrigger(DamageDataBus damageData)
    {

    }
}
