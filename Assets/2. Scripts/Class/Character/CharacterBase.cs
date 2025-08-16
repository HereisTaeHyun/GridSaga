using System.Collections;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterData;
    public CharacterSO CharacterData => characterData;
    [SerializeField] protected GameObject ActiveSkill;
    [SerializeField] protected GameObject PassiveSkill;

    private enum CharacterState
    {
        Idle,
        Move,
        Attack,
    }
    protected int currentHp;
    protected int currentDefense;
    protected int currentAttack;
    protected int currentSpeed;
    protected float currentCritRate;

    public int CurrentHp => currentHp; 
    public int CurrentDefense => currentDefense; 
    public int CurrentAttack => currentAttack; 
    public int CurrentSpeed => currentSpeed; 
    public float CurrentCritRate => currentCritRate; 

    // init애서 스턋 배정은 이후 DB 권한으로 이전할 것
    // 현재 구조는 클라이언트 로컬 개발에서만 이용
    protected virtual void Init()
    {
        currentHp = characterData.BaseHp;
        currentDefense = characterData.BaseDefense;
        currentAttack = characterData.BaseAttack;
        currentSpeed = characterData.BaseSpeed;
        currentCritRate = characterData.BaseCritRate;
    }


    // public virtual void Attack(CharacterBase target)
    // {

    // }

    public virtual IEnumerator Attack(CharacterBase target)
    {
        Debug.Log("Base attack start");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Base attack end");
    }

    protected virtual void UseActiveSkill()
    {

    }

    protected virtual void UsePassiveSkill()
    {
        
    }

    public virtual void GetDamage(int damage)
    {

    }
    
    protected virtual void Die()
    {
        
    }
}
