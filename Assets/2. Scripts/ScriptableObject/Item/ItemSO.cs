using UnityEngine;

[CreateAssetMenu(menuName = "ItemSO")]
public class ItemSO : ScriptableObject
{
    [SerializeField] private int itemId;
    [SerializeField] private string itemName;
    [SerializeField] private ItemKind itemKind;
    [SerializeField] private StatKind statKind;
    [SerializeField] private EffectKind effectKind;
    [SerializeField] private int value;
    [SerializeField] private int itemSkillId;

    public int ItemId => itemId;
    public string ItemName => itemName;
    public ItemKind ItemKind => itemKind;
    public StatKind StatKind => statKind;
    public EffectKind EffectKind => effectKind;
    public int Value => value;
    public int ItemSkillId => itemSkillId;
}


// Table Items {
//   item_id   int         [pk, increment]
//   item_name varchar(50) [unique, not null]
//   item_kind item_kimd [not null]
//   stat_type   tinyint 
//   effect_kind effect_kind
//   value          decimal(6,2) [not null]
//   item_skill_id int [ref: > Skill.skill_id]
// }