using System;
using System.Collections;
using UnityEngine;


public class CharacterBase : MonoBehaviour, ICombat
{
    [SerializeField] protected CharacterSO characterData;
    public CharacterSO CharacterData => characterData;
    [SerializeField] protected SkillBase activeSkill;
    [SerializeField] protected SkillBase passiveSkill;

    public enum CharacterState
    {
        Idle,
        Move,
        Attack,
        Die,
    }

    public CharacterState characterState;
    protected int maxHp;
    protected int currentHp;
    protected int currentDefense;
    protected int currentAttack;
    protected int currentSpeed;
    protected float currentCritRate;
    protected float currentAttackRange;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public int CurrentDefense => currentDefense;
    public int CurrentAttack => currentAttack;
    public int CurrentSpeed => currentSpeed;
    public float CurrentCritRate => currentCritRate;
    public float CurrentAttackRange => currentAttackRange;

    protected Animator anim;
    protected readonly int moveXHash = Animator.StringToHash("MoveX");
    protected readonly int moveYHash = Animator.StringToHash("MoveY");
    protected readonly int isMoveHash = Animator.StringToHash("IsMove");
    protected readonly int attackHash = Animator.StringToHash("Attack");
    protected readonly int usePassiveSkillHash = Animator.StringToHash("UsePassiveSkill");
    protected readonly int useActiveSkillHash = Animator.StringToHash("UseActiveSkill");
    protected readonly int dieHash = Animator.StringToHash("Die");
    protected readonly int isBuffHash = Animator.StringToHash("IsBuff");


    public bool IsAlive => currentHp > 0 && characterState != CharacterState.Die && gameObject.activeInHierarchy;

    // 공격 처리, 사망 자연스럽게 하기 위한 변수
    protected float attackActiveTime;
    protected float attackEndTime;
    protected float dieTime;
    protected bool isDieTriggered;
    public event Action<DamageDataBus> SendDamageData;

    // 공격 속도 제어
    // 스피드 1 = 0.25초의 딜레이 경감을 가짐
    private float delayBySpeed = 0.25f;
    private float minDelay = 0.25f;
    private float maxDelay = 2.5f;

    protected bool isPassiveTriggered;
    public bool IsPassiveTriggered => isPassiveTriggered;
    public Vector3 Position => transform.position;

    protected CharacterCtrl characterCtrl;


    // init애서 스탯 배정은 이후 DB 권한으로 이전할 것
    // 현재 구조는 클라이언트 로컬 개발에서만 이용
    protected virtual void Init()
    {
        maxHp = characterData.BaseHp;
        currentHp = maxHp;
        currentDefense = characterData.BaseDefense;
        currentAttack = characterData.BaseAttack;
        currentSpeed = characterData.BaseSpeed;
        currentCritRate = characterData.BaseCritRate;
        currentAttackRange = characterData.BaseAttackRange;

        characterState = CharacterState.Idle;
        isDieTriggered = false;
        isPassiveTriggered = false;

        anim = GetComponent<Animator>();
        characterCtrl = GetComponentInParent<CharacterCtrl>();
    }

    void OnEnable()
    {
        SendDamageData += ApplyDamageFeedback;

        characterCtrl.ActivateAttack += Attack;
    }

    void OnDisable()
    {
        SendDamageData -= ApplyDamageFeedback;

        characterCtrl.ActivateAttack -= Attack;
    }

    // 타겟을 향해 이동
    protected virtual void Move()
    {

    }

    // 공격
    protected virtual void Attack()
    {

    }

    // 스킬 사용
    public virtual void UseActiveSkill()
    {

    }

    public virtual void UsePassiveSkill()
    {

    }

    // 데미지 입음
    public virtual DamageDataBus GetDamage(ICombat attacker, ICombat target, int damage)
    {
        // 데미지는 음수 불가
        int safeDamage = Mathf.Max(0, damage);

        // hp 차감 후 0 이하면 사망
        currentHp -= safeDamage;
        if (currentHp <= 0)
        {
            Die();
        }

        var damageData = new DamageDataBus(attacker, target, safeDamage, currentHp);
        return damageData;
    }

    public void InvokeDamageDataEvent(DamageDataBus damageData)
    {
        SendDamageData?.Invoke(damageData);
    }

    // 사망 처리
    protected virtual void Die()
    {

    }


    // 스피드에 따른 공격 딜레이 처리
    protected float GetDelay(int speed)
    {
        speed = Mathf.Max(0, speed);
        float delay = maxDelay - (speed * delayBySpeed);
        return Mathf.Max(minDelay, delay);
    }

    // 데미지를 입은 경우 애니메이션, UI 등 처리
    private void ApplyDamageFeedback(DamageDataBus damageData)
    {
        if (damageData.isDied)
        {
            StartCoroutine(ApplyDieAnim());
        }
    }
    public virtual IEnumerator ApplyDieAnim()
    {
        if (characterState == CharacterState.Die && isDieTriggered == false)
        {
            isDieTriggered = true;
            anim.SetTrigger(dieHash);
            yield return new WaitForSeconds(dieTime);
            gameObject.SetActive(false);
        }
    }

    public void ChangeStat(StatKind statKind, int value)
    {
        switch (statKind)
        {
            case StatKind.hp:
                currentHp += value;
                break;
            case StatKind.defense:
                currentDefense += value;
                break;
            case StatKind.attack:
                currentAttack += value;
                break;
            case StatKind.speed:
                currentSpeed += value;
                break;
            case StatKind.critRate:
                currentCritRate += value;
                break;
        }
    }
}
