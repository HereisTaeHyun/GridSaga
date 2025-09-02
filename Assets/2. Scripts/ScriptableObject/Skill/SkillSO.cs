using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/SkillSO")]
public class SkillSO : ScriptableObject
{
    [SerializeField] private int skillId;
    [SerializeField] string skillName;
    [SerializeField] private SkillKind skillKind;
    [SerializeField] private StatKind statKind;
    [SerializeField] private string description;

    public int SkillId => skillId;
    public string SkillName => skillName;
    public SkillKind SkillKind => skillKind;
    public StatKind StatKind => statKind;
    public string Description => description;
}

// Table Skill {
//   skill_id   int         [pk, increment]
//   skill_name varchar(50) [unique, not null]
//   skill_cool_down int [not null]
//   skill_kind skill_kind [not null]
//   stat_kind stat_kind [not null]
//   description text
// }