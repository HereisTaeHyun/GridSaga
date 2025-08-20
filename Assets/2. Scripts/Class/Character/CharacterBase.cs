using System;
using System.Collections;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterData;
    public CharacterSO CharacterData => characterData;
    [SerializeField] protected GameObject ActiveSkill;
    [SerializeField] protected GameObject PassiveSkill;

    public enum CharacterState
    {
        Idle,
        Move,
        Attack,
        Die,
    }

    public CharacterState characterState;
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

    protected Animator anim;
    protected readonly int factionHash = Animator.StringToHash("Faction");
    protected readonly int isMoveHash = Animator.StringToHash("IsMove");
    protected readonly int attackHash = Animator.StringToHash("Attack");
    protected readonly int usePassiveSkillHash = Animator.StringToHash("UsePassiveSkill");
    protected readonly int useActiveSkillHash = Animator.StringToHash("UseActiveSkill");
    protected readonly int dieHash = Animator.StringToHash("Die");
    protected readonly int isBuffHash = Animator.StringToHash("IsBuff");


    public bool IsAlive => currentHp > 0 && characterState != CharacterState.Die && gameObject.activeInHierarchy;
    public virtual bool CanAttack => IsAlive && characterState == CharacterState.Idle;
    public virtual bool CanBeTarget => IsAlive && characterState != CharacterState.Attack;

    // 공격 처리 자연스럽게 하기 위한 변수
    protected float attackActiveTime;
    protected float returnAfterAttackTime;
    protected float dieTime;
    protected float attackRange;

    // 공격 속도 제어
    // 스피드 1 = 0.25초의 딜레이 경감을 가짐
    private float delayBySpeed = 0.25f;
    private float minDelay = 0.25f;
    private float maxDelay = 2.5f;

    [SerializeField] private Faction allyFaction;


    // init애서 스탯 배정은 이후 DB 권한으로 이전할 것
    // 현재 구조는 클라이언트 로컬 개발에서만 이용
    protected virtual void Init()
    {
        currentHp = characterData.BaseHp;
        currentDefense = characterData.BaseDefense;
        currentAttack = characterData.BaseAttack;
        currentSpeed = characterData.BaseSpeed;
        currentCritRate = characterData.BaseCritRate;

        characterState = CharacterState.Idle;

        anim = GetComponent<Animator>();
    }

    // 타겟을 향해 이동
    protected virtual void Move(CharacterBase target)
    {

    }

    // 공격
    public virtual IEnumerator Attack(CharacterBase target)
    {
        Debug.Log("Base attack start");
        characterState = CharacterState.Attack;

        float wait = GetDelay(CurrentSpeed);
        yield return new WaitForSeconds(wait);

        Debug.Log("Base attack end");
        characterState = CharacterState.Idle;
    }

    // 스킬 사용
    protected virtual void UseActiveSkill()
    {

    }

    protected virtual void UsePassiveSkill()
    {

    }

    // 데미지 입음
    public virtual void GetDamage(int damage)
    {

    }

    // 사망 처리
    protected virtual IEnumerator Die()
    {
        yield break;
    }

    // 팩션 어디인지 처리
    public void SetFaction(Faction faction)
    {
        allyFaction = faction;
    }

    protected void SetTarget(CharacterBase target)
    {

    }

    
    // 스피드에 따른 딜레이 처리
    protected float GetDelay(int speed)
    {
        speed = Mathf.Max(0, speed);
        float delay = maxDelay - (speed * delayBySpeed);
        return Mathf.Max(minDelay, delay);
    }
}
