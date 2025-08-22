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

    protected virtual void Init()
    {
        skillId = skillData.SkillId;
        skillName = skillData.SkillName;
        skillKind = skillData.SkillKind;
        description = skillData.Description;

    }

    // 스킬 조건과 트리거 될 경우 할 행동
    protected virtual void SkillTrigger()
    {

    }

    // 플레이어의 스탯 등 상태에 영행을 주면 이 오버로드 사용
    // 현재는 public이지만 이후 이벤트 구독으로 바꾸면서 protected로 바꿀 것!
    public virtual void SkillTrigger(CharacterBase character)
    {

    }
}
