using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    [SerializeField] private int characterId;
    [SerializeField] private string characterName;
    [SerializeField] private CharacterKind characterKind;
    [SerializeField] private int baseHp;
    [SerializeField] private int baseDefense;
    [SerializeField] private int baseAttack;
    [SerializeField] private int baseSpeed;
    [SerializeField] private float baseCritRate;
    [SerializeField] private float baseAttackRange;
    [SerializeField] private int passiveSkillId;
    [SerializeField] private int activeSkillId;

    public int CharacterId => characterId;
    public string CharacterName => characterName;
    public CharacterKind CharacterKind => characterKind;
    public int BaseHp => baseHp;
    public int BaseDefense => baseDefense;
    public int BaseAttack => baseAttack;
    public int BaseSpeed => baseSpeed;
    public float BaseCritRate => baseCritRate;
    public float BaseAttackRange => baseAttackRange;
    public int PassiveSkillId => passiveSkillId;
    public int ActiveSkillId => activeSkillId;
}


// Table Character {
//   char_id   int         [pk, increment]
//   char_name varchar(50) [unique, not null]
//   hp        int         [not null]
//   defense   int         [not null]
//   attack    int         [not null]
//   speed int [not null]       // 0–100
//   crit_rate decimal(5,2) [not null]       // 0–100

//   passive_skill_id int [ref: > Skill.skill_id]
//   active_skill_id  int [ref: > Skill.skill_id]
// }