using System.Collections;
using UnityEngine;

public class Warrior : CharacterBase
{

    protected override void Init()
    {
        base.Init();

        attackEndTime = 0.75f;
        dieTime = 1.5f;
        attackRange = 2.0f;
    }

    private void Awake()
    {
        Init();
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (currentTarget == null)
        {
            currentTarget = SetTarget();
        }

        if (currentTarget != null)
        {
            Move(currentTarget);
        }
        else
        {
            anim.SetBool(isMoveHash, false);  
        } 
    }

    protected override void Move(CharacterBase target)
    {
        if (Vector2.Distance(transform.position, target.transform.position) > attackRange)
        {
            anim.SetBool(isMoveHash, true);
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, currentSpeed * Time.deltaTime);
        }
        else if (Vector2.Distance(transform.position, target.transform.position) <= attackRange && characterState != CharacterState.Attack)
        {
            anim.SetBool(isMoveHash, false);
            StartCoroutine(Attack(target));
        }
    }

    protected override IEnumerator Attack(CharacterBase target)
    {
        if (characterState == CharacterState.Die)
        {
            yield break;
        }

        characterState = CharacterState.Attack;

        // 공격 딜레이 적용
        float wait = GetDelay(CurrentSpeed);
        yield return new WaitForSeconds(wait);

        // 공격 적용
        anim.SetTrigger(attackHash);
        GameManager.gameManager.ApplyDamage(this, target);
        yield return new WaitForSeconds(attackEndTime);
        characterState = CharacterState.Idle;
    }

    protected override void UseActiveSkill()
    {
        Debug.Log($"{this.name} Active Skill Activated");
        anim.SetTrigger(useActiveSkillHash);
    }

    protected override void UsePassiveSkill()
    {
        Debug.Log($"{this.name} Passive Skill Activated");
        anim.SetTrigger(usePassiveSkillHash);
    }

    public override void GetDamage(int damage)
    {
        // 데미지는 음수 불가
        int safeDamage = Mathf.Max(0, damage);

        // hp 차감 후 0 이하면 사망
        currentHp -= safeDamage;
        if (currentHp <= 0)
        {
            StartCoroutine(Die());
        }
    }

    protected override IEnumerator Die()
    {
        characterState = CharacterState.Die;
        anim.SetTrigger(dieHash);
        yield return new WaitForSeconds(dieTime);
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
