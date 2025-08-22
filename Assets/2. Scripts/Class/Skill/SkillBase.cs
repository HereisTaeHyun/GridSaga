using UnityEngine;

public class SkillBase : MonoBehaviour
{
    [SerializeField] SkillSO skillData;
    protected bool isTriggered;
    protected int skillId;
    protected string skillName;
    protected SkillKind skillKind;
    protected string description;

    public bool IsTriggered => isTriggered;
    public int SkillId => skillId;
    public string SkillName => skillName;
    public SkillKind SkillKind => skillKind;
    public string Description => description;

    protected virtual void Init()
    {
        isTriggered = false;

        skillId = skillData.SkillId;
        skillName = skillData.SkillName;
        skillKind = skillData.SkillKind;

    }

    protected virtual void SkillTrigger()
    {
        
    }
}
