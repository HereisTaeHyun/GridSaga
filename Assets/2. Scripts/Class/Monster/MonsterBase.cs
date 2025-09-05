using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterBase : MonoBehaviour, ICombat
{
    [SerializeField] protected CharacterSO monsterData;
    public CharacterSO MonsterData => monsterData;
    [SerializeField] protected SkillBase activeSkill;
    [SerializeField] protected SkillBase passiveSkill;

    public enum MonsterState
    {
        Idle,
        Move,
        Attack,
        Die,
    }

    public MonsterState monsterState;
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


    public bool IsAlive => currentHp > 0 && monsterState != MonsterState.Die && gameObject.activeInHierarchy;


    // 공격 처리, 사망 자연스럽게 하기 위한 변수
    protected float attackActiveTime;
    protected float attackEndTime;
    protected float dieTime;
    protected bool isDieTriggered;

    // 체력 변화 시 발생할 이벤트
    // 순서대로 MaxHp, currentHp 순서
    public event Action<int, int> HpChanged;

    // 공격 속도 제어
    // 스피드 1 = 0.25초의 딜레이 경감을 가짐
    protected bool canAttack;

    protected CharacterBase currentTarget;
    protected bool isPassiveTriggered;
    public bool IsPassiveTriggered
    {
        get { return isPassiveTriggered; }
        set { isPassiveTriggered = value; }
    }
    public Vector3 Position => transform.position;

    // 플레이어야 시야에 있는지 처리하기 위한 변수
    protected float sightRange;
    protected bool isPlayerInSight;
    protected LayerMask attackableLayer;
    protected LayerMask obstacleLayer;
    public LayerMask AttackableLayer => attackableLayer;
    private RaycastHit2D[] rayHits = new RaycastHit2D[10];
    private Collider2D scanResult;
    private WaitForSeconds scanWait;
    protected float scanInterval = 0.25f;

    // 움직임 체크 변수
     protected Rigidbody2D rb2D;
    protected Vector2 lastDir;

    // 공격 범위 체크 변수
    protected float attackDegree;
    protected float attackThreshold;



    // init애서 스탯 배정은 이후 DB 권한으로 이전할 것
    // 현재 구조는 클라이언트 로컬 개발에서만 이용
    protected virtual void Init()
    {
        maxHp = monsterData.BaseHp;
        currentHp = maxHp;
        currentDefense = monsterData.BaseDefense;
        currentAttack = monsterData.BaseAttack;
        currentSpeed = monsterData.BaseSpeed;
        currentCritRate = monsterData.BaseCritRate;
        currentAttackRange = monsterData.BaseAttackRange;

        monsterState = MonsterState.Idle;
        isDieTriggered = false;
        isPassiveTriggered = false;
        canAttack = true;

        isPlayerInSight = false;
        attackableLayer = LayerMask.GetMask("Character");
        obstacleLayer = LayerMask.GetMask("Wall");
        scanWait = new WaitForSeconds(scanInterval);

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        StartCoroutine(ScanPlayer());
    }

    void OnEnable()
    {
        HpChanged += ApplyDamageFeedback;
    }

    void OnDisable()
    {
        HpChanged -= ApplyDamageFeedback;
    }

    // 사거리 이내이고 시야 내의 캐릭터를 설정
    protected IEnumerator ScanPlayer()
    {
        yield return null;
        while (IsAlive)
        {
            FindTarget();
            yield return scanWait;
        }
    }

    // 사거리 이내이고 시야 내의 캐릭터를 설정
    protected virtual void FindTarget()
    {
        scanResult = Physics2D.OverlapCircle(transform.position, sightRange, attackableLayer);
        if (scanResult == null)
        {
            currentTarget = null;
            anim.SetBool(isMoveHash, false);
            return;
        }

        var selected = scanResult.GetComponentInChildren<CharacterBase>();
        isPlayerInSight = SeeingPlayer(selected);
        if (selected != null && isPlayerInSight)
        {
            currentTarget = selected;
        }
        else if (!isPlayerInSight)
        {
            currentTarget = null;
            monsterState = MonsterState.Idle;
        }
    }
    
    // ray를 쏘아 첫 대상이 캐릭터인지 감지
    protected bool SeeingPlayer(CharacterBase character)
    {
        Vector2 direction = character.transform.position - transform.position;
        Vector2 directionNorm = UtilityManager.utility.DirSet(direction);

        // 콜라이더 기준이 발 위치니 그에 맞추기
        Vector2 origin = FootPoint(transform);
        Vector2 target = FootPoint(character.transform);
        float distance = Vector2.Distance(origin, target);
        int count = Physics2D.RaycastNonAlloc(origin, directionNorm, rayHits, distance, attackableLayer | obstacleLayer);
        
        // ray에 닿은 존재가 있으며 첫 충돌이 Character라면 true
        if (count > 0)
        {
            var hit = rayHits[0];
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                return true;
            }
        }
        return false;
    }

    protected Vector2 FootPoint(Transform transform)
    {
        var collider = transform.GetComponent<Collider2D>();
        var bound = collider.bounds;
        return new Vector2(bound.center.x, bound.center.y - 0.02f);
    }

    // 타겟을 향해 이동
    protected virtual void Move(ICombat target)
    {
        if (!isPlayerInSight) return;

        Vector2 currentPos = transform.position;
        Vector2 targetPos = target.Position;
        float distance = Vector2.Distance(transform.position, target.Position);

        Vector2 dir = UtilityManager.utility.DirSet(targetPos - currentPos);
        anim.SetFloat(moveXHash, dir.x);
        anim.SetFloat(moveYHash, dir.y);

        lastDir.x = dir.x;
        lastDir.y = dir.y;

        if (distance > currentAttackRange)
        {
            anim.SetBool(isMoveHash, true);
            transform.position = Vector2.MoveTowards(transform.position, target.Position, currentSpeed * Time.deltaTime);
        }
        else if (distance <= currentAttackRange && monsterState != MonsterState.Attack)
        {
            anim.SetBool(isMoveHash, false);
            StartCoroutine(Attack());
        }
    }

    // 공격
    protected virtual IEnumerator Attack()
    {
        Debug.Log("Base attack start");
        monsterState = MonsterState.Attack;
        Debug.Log("Base attack end");
        monsterState = MonsterState.Idle;
        yield break;
    }

    // 스킬 사용
    public virtual void UseActiveSkill()
    {

    }

    public virtual void UsePassiveSkill()
    {

    }

    // 데미지 처리
    public virtual void GetDamage(ICombat target, int damage)
    {
        // 데미지는 음수 불가
        int safeDamage = Mathf.Max(0, damage);

        currentHp = (int)Mathf.Max(0f, currentHp - safeDamage);
        HpChanged?.Invoke(maxHp, currentHp);
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
    protected virtual IEnumerator Die()
    {
        if (isDieTriggered == false)
        {
            monsterState = MonsterState.Die;
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
