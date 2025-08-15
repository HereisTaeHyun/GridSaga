using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterData;
    public CharacterSO CharacterData => characterData;
    [SerializeField] protected GameObject ActiveSkill;
    [SerializeField] protected GameObject PassiveSkill;

    protected virtual void Init()
    {

    }

    protected virtual void Attack()
    {

    }

    protected virtual void UseActiveSkill()
    {
        
    }

    protected virtual void UsePassiveSkill()
    {
        
    }

    protected virtual void GetDamage()
    {

    }
    
    protected virtual void Die()
    {
        
    }
}
