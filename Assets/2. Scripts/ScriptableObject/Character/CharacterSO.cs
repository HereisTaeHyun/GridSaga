using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    [SerializeField] private int characterId;
    [SerializeField] private string characterName;
    [SerializeField] private int baseHp;
    [SerializeField] private int baseDefense;
    [SerializeField] private int baseAttack;
    [SerializeField] private float attackCoolTime;
    [SerializeField] private int speed;
    [SerializeField] private float critRate;
    [SerializeField] private int passiveSkillId;
    [SerializeField] private int activeSkillId;

    public int CharacterId => characterId;
    public string CharacterName => characterName;
    public int BaseHp => baseHp;
    public int BaseDefense => baseDefense;
    public int BaseAttack => baseAttack;
    public float AttackCoolTime => attackCoolTime;
    public int Speed => speed;
    public float CritRate => critRate;
    public int PassiveSkillId => passiveSkillId;
    public int ActiveSkillId => activeSkillId;
}


// Table Character {
//   char_id   int         [pk, increment]
//   char_name varchar(50) [unique, not null]
//   hp        int         [not null]
//   defense   int         [not null]
//   attack    int         [not null]
//   attack_cool_time decimal(5,2)            [not null]
//   speed int [not null]       // 0–100
//   crit_rate decimal(5,2) [not null]       // 0–100

//   passive_skill_id int [ref: > Skill.skill_id]
//   active_skill_id  int [ref: > Skill.skill_id]
// }