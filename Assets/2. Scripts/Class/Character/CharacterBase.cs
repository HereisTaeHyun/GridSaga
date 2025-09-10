using System;
using System.Collections;
using UnityEngine;

public class CharacterBase : MonoBehaviour, ICombat
{
    [SerializeField] protected CharacterSO characterData;
    public CharacterSO CharacterData => characterData;
    [SerializeField] protected SkillBase passiveSkill;

    public enum CharacterState
    {
        Idle,
        Move,
        Attack,
        Die,
    }

    protected CharacterState characterState;
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
    protected readonly int lookXHash = Animator.StringToHash("LookX");
    protected readonly int lookYHash = Animator.StringToHash("LookY");
    protected readonly int isMoveHash = Animator.StringToHash("IsMove");
    protected readonly int isMoveToBackHash = Animator.StringToHash("IsMoveToBack");
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
    protected float activeSkillActiveTime;
    protected float activeSkillEndTime;
    protected float activeSkillCoolDown;

    // 체력 변화 시 발생할 이벤트
    // 순서대로 MaxHp, currentHp 순서
    public event Action<int, int> HpChanged;

    protected LayerMask attackableLayer;
    protected LayerMask obstacleLayer;

    public LayerMask AttackableLayer => attackableLayer;

    // 공격 제어
    protected bool canAttack;
    protected bool canUseActiveSkill;

    protected bool isPassiveTriggered;
    public bool IsPassiveTriggered
    {
        get { return isPassiveTriggered; }
        set { isPassiveTriggered = value; }
    }
    public Vector3 Position => transform.position;

    protected CharacterCtrl characterCtrl;
    protected Rigidbody2D rb2D;
    protected bool isMove;
    protected bool isMoveToBack;
    protected Vector2 lastLookDir;
    
    // 공격 범위 체크 변수
    protected float attackDegree;
    protected float attackThreshold;


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
        canAttack = true;
        canUseActiveSkill = true;

        attackableLayer = LayerMask.GetMask("Monster");
        obstacleLayer = LayerMask.GetMask("Wall");

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        characterCtrl = GetComponentInParent<CharacterCtrl>();

    }

    protected virtual void OnEnable()
    {
        HpChanged += ApplyDamageFeedback;
    }

    protected virtual void OnDisable()
    {
        HpChanged -= ApplyDamageFeedback;
    }

    // 이동
    protected virtual void Move()
    {
        isMove = characterCtrl.Move.magnitude > 0.0001f;
        anim.SetBool(isMoveHash, isMove);

        characterState = isMove ? CharacterState.Move : CharacterState.Idle;

        // 바라보는 방향 설정
        Vector2 aimDir = characterCtrl.AimDir;
        lastLookDir.x = aimDir.x;
        lastLookDir.y = aimDir.y;
        anim.SetFloat(lookXHash, lastLookDir.x);
        anim.SetFloat(lookYHash, lastLookDir.y);

        // 움직임 각도 보정
        Vector2 rawDegree = characterCtrl.Move;
        Vector2 correctDegree;
        if (rawDegree.x != 0f && rawDegree.y != 0f)
            correctDegree = new Vector2(rawDegree.x, rawDegree.y * 0.5f).normalized;
        else
            correctDegree = rawDegree;

        // 움직임 설정
        Vector2 newVelocity = new Vector2(currentSpeed * correctDegree.x, currentSpeed * correctDegree.y);
        rb2D.linearVelocity = newVelocity;

        // 뒤로 움직이는지 설정
        float dot = Vector2.Dot(characterCtrl.Move, lastLookDir.normalized);
        float dotThreshold = -0.35f;
        isMoveToBack = dot < dotThreshold;
        anim.SetBool(isMoveToBackHash, isMoveToBack);
    }

    // 공격 활성화 함수, 얘로 검사 안거치면 여러 Attack 코루틴 시행됨
    public virtual void ActiveAttack()
    {
        if (canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    // 공격
    protected virtual IEnumerator Attack()
    {
        yield break;
    }

    // 스킬 사용
    public virtual void UseActiveSkill()
    {
        if (canUseActiveSkill)
        {
            StartCoroutine(ActiveSkill());
        }
    }

    protected virtual IEnumerator ActiveSkill()
    {
        yield break;
    }

    protected virtual IEnumerator ActiveSkillCoolDown()
    {
        canUseActiveSkill = false;
        yield return new WaitForSeconds(activeSkillCoolDown);
        canUseActiveSkill = true;
    }

    public virtual void UsePassiveSkill()
    {

    }

    // 데미지 입음
    public virtual void GetDamage(ICombat target, int damage)
    {
        // 데미지는 음수 불가
        int safeDamage = Mathf.Max(0, damage);

        // hp 차감 후 0 이하면 사망
        currentHp = (int)Mathf.Max(0f, currentHp - safeDamage);
        HpChanged?.Invoke(currentHp, maxHp);

        if (currentHp <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }


    // 데미지를 입은 경우 애니메이션, UI 등 처리
    private void ApplyDamageFeedback(int maxHp, int currentHp)
    {

    }
    public virtual IEnumerator Die()
    {
        if (isDieTriggered == false)
        {
            characterState = CharacterState.Die;
            isDieTriggered = true;

            rb2D.linearVelocity = Vector2.zero;
            rb2D.simulated = false;

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
